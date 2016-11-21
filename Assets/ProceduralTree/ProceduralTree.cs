using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ProceduralTree : MonoBehaviour
{
    // Тучка параметров
    public int Seed; // Другое число - другое дерево
    [Range(1024, 65000)]
    public int MaxNumVertices = 65000; // Максимальное число вершин - чем больше, тем более ветвей может быть
    [Range(3, 32)]
    public int NumberOfSides = 16; // Число сторон, чем больше - тем выше качество
    [Range(0.25f, 4f)]
    public float BaseRadius = 2f; // Радиус основания
    [Range(0.75f, 0.95f)]
    public float RadiusStep = 0.9f; // Насколько быстро радиус уменьшается
    [Range(0.01f, 0.2f)]
    public float MinimumRadius = 0.02f; // Насколько маленьким радиус может быть
    [Range(0f, 1f)]
    public float BranchRoundness = 0.8f; // Округлость ветвей
    [Range(0.1f, 2f)]
    public float SegmentLength = 0.5f; // Длина ветвей (точнее их сегментов)
    [Range(0f, 40f)]
    public float Twisting = 20f; // Скручиваем ветви
    [Range(0f, 0.25f)]
    public float BranchProbability = 0.1f; // Вероятность появления ветви
    
        
    float checksum; // Обновлять или не обновлять - вот в чём вопрос
    [SerializeField, HideInInspector]
    float checksumSerialized;

    List<Vector3> vertexList; // Храним вершины
    List<Vector2> uvList; // Это чтобы текстурка правильно ложилась
    List<int> triangleList; // Куда без треугольников?

    float[] ringShape; // "Кольца" дерева

    [HideInInspector, System.NonSerialized]
    public MeshRenderer Renderer; // компонента меша, API юнити
    MeshFilter filter; // ещё компонента меша, тоже API
    
    // Инициализируемся, вытаскиваем компоненты
    void OnEnable()
    {
        // экономим память (не экономится чёт)
        hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;

        if (filter != null && Renderer != null) return;

        gameObject.isStatic = true;

        filter = gameObject.GetComponent<MeshFilter>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
        if (filter.sharedMesh != null) checksum = checksumSerialized;
        Renderer = gameObject.GetComponent<MeshRenderer>();
        if (Renderer == null) Renderer = gameObject.AddComponent<MeshRenderer>();
    }
    
    // Генерируем дерево
    public void GenerateTree()
    {
        gameObject.isStatic = false;

        // сохраняем текущее состояние
        var originalRotation = transform.localRotation;
        var originalState = Random.state;

        if (vertexList == null ) { // Создаём листы для хранения всякого
            vertexList = new List<Vector3>();
            uvList = new List<Vector2>();
            triangleList = new List<int>();
        } else { // Ну или чистим их
            vertexList.Clear();
            uvList.Clear();
            triangleList.Clear();
        }

        SetTreeRingShape(); // Создаём кольцо
        Random.InitState(Seed); // Это мы "задаём" рандом

        // Запускаем рекурсию, строим дерево
        Branch(new Quaternion(), Vector3.zero, -1, BaseRadius, 0f);

        // восстанавливаем состояние
        transform.localRotation = originalRotation;
        Random.state = originalState;

        SetTreeMesh(); // Строим меш по сгенерированному дереву
    }

    // Строим меш по сгенерированному дереву
    private void SetTreeMesh()
    {
        // берём или создаём меш
        var mesh = filter.sharedMesh;
        if (mesh == null) 
            mesh = filter.sharedMesh = new Mesh();
        else 
            mesh.Clear();

        // устанавливаем наши вертексы, UV-отображения и треугольники
        mesh.vertices = vertexList.ToArray();
        mesh.uv = uvList.ToArray();
        mesh.triangles = triangleList.ToArray();

        // просим меш обновится
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    
    // Собственно, как мы строим дерево
    void Branch(Quaternion quaternion, Vector3 position, int lastRingVertexIndex, float radius, float texCoordV)
    {
        // немного стафа
        var offset = Vector3.zero;
        var texCoord = new Vector2(0f, texCoordV);
        var textureStepU = 1f / NumberOfSides;
        var angInc = 2f * Mathf.PI * textureStepU;
        var ang = 0f;

        // накручиваем кольцо
        for (var n = 0; n <= NumberOfSides; n++, ang += angInc) 
        {
            var r = ringShape[n] * radius;
            offset.x = r * Mathf.Cos(ang); // считаем смещения по x
            offset.z = r * Mathf.Sin(ang); // и z
            vertexList.Add(position + quaternion * offset); // добавляем посчитанную позицию
            uvList.Add(texCoord); // обновляем отображение
            texCoord.x += textureStepU;
        }

        if (lastRingVertexIndex >= 0) // после первого кольца
        {
            // нам нужно соединить два последних кольца квадратами
            for ( var currentRingVertexIndex = vertexList.Count - NumberOfSides - 1; currentRingVertexIndex < vertexList.Count - 1; currentRingVertexIndex++, lastRingVertexIndex++) 
            {
                triangleList.Add(lastRingVertexIndex + 1); // из треугольников
                triangleList.Add(lastRingVertexIndex);
                triangleList.Add(currentRingVertexIndex);
                triangleList.Add(currentRingVertexIndex); // по два на каждый квадрат
                triangleList.Add(currentRingVertexIndex + 1);
                triangleList.Add(lastRingVertexIndex + 1);
            }
        }

        // Если закончили с текущей веткой
        radius *= RadiusStep;
        if (radius < MinimumRadius // если она меньше минимального радиуса
            || vertexList.Count + NumberOfSides >= MaxNumVertices ) // или закончились вершины
        {
            vertexList.Add(position); // осталось добавить центральную вершину
            uvList.Add(texCoord + Vector2.one); // закольцовываем текстуру
            for (var n = vertexList.Count - NumberOfSides - 2; n < vertexList.Count - 2; n++) // и соединить её с последним кольцом
            {
                triangleList.Add(n); // треугольниками, конечно
                triangleList.Add(vertexList.Count - 1);
                triangleList.Add(n + 1);
            }
            return; 
        }

        // ну или продолжаем текущую ветку
        texCoordV += 0.0625f * (SegmentLength + SegmentLength / radius);
        position += quaternion * new Vector3(0f, SegmentLength, 0f);
        transform.rotation = quaternion; 
        var x = (Random.value - 0.5f) * Twisting;
        var z = (Random.value - 0.5f) * Twisting;
        transform.Rotate(x, 0f, z);
        lastRingVertexIndex = vertexList.Count - NumberOfSides - 1;
        Branch(transform.rotation, position, lastRingVertexIndex, radius, texCoordV); // переходя к следующему сегменту

        // если закончили ветку - выходим
        if (vertexList.Count + NumberOfSides >= MaxNumVertices || Random.value > BranchProbability) return;

        // И да, иногда ветвимся в новые ветки
        transform.rotation = quaternion;
        x = Random.value * 70f - 35f;
        x += x > 0 ? 10f : -10f;
        z = Random.value * 70f - 35f;
        z += z > 0 ? 10f : -10f;
        transform.Rotate(x, 0f, z);
        Branch(transform.rotation, position, lastRingVertexIndex, radius, texCoordV);
    }
    
    private void SetTreeRingShape()
    {
        ringShape = new float[NumberOfSides + 1];
        var k = (1f - BranchRoundness) * 0.5f;
        // немного рандома при создании кольца
        for ( var n = 0; n < NumberOfSides; n++) ringShape[n] = 1f - (Random.value - 0.5f) * k;
        ringShape[NumberOfSides] = ringShape[0];
    }
    
    // Проверить параметры и, если надо, обновить дерево
    public void ForceUpdate()
    {
        // вычисляем чексумму
        var newChecksum = (Seed & 0xFFFF) + NumberOfSides + SegmentLength + BaseRadius + MaxNumVertices +
            RadiusStep + MinimumRadius + Twisting + BranchProbability + BranchRoundness;

        // и выходим, если она совпадает с предыдущей
        if (newChecksum == checksum && filter.sharedMesh != null) return;

        // обновляем в противном случае
        checksumSerialized = checksum = newChecksum; // чексумму
        GenerateTree(); // и дерево
    }

    public void Update() {
        ForceUpdate();
    }
    
    // чтобы память не текла в эдиторе
#if UNITY_EDITOR
    void OnDisable()
    {
        if (filter.sharedMesh == null) return; // если нет меша - нечего удалять
        if (PrefabUtility.GetPrefabType(this) == PrefabType.PrefabInstance) {   
            var parentPrefab = PrefabUtility.GetPrefabParent(this);
            var list = (ProceduralTree[])FindObjectsOfType(typeof(ProceduralTree));
            foreach (var go in list)
                if (go != this && PrefabUtility.GetPrefabParent(go) == parentPrefab)
                    return; // если кто-то зачем-то использует наш меш, то удалять его не стоит
        }
        DestroyImmediate(filter.sharedMesh, true); // ну и удаляем меш, если можно
    }
#endif
}