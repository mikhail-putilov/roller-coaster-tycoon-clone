using UnityEngine;

public class MeshBuilder : MonoBehaviour {
    public int xSize;
    public int ySize;
    Mesh mesh;
    Vector3[] verts;

    void Start() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Plane";

        BuildPlane();
    }


    void BuildPlane () {
        //генерируем вершины
        verts = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++) 
            for (int x = 0; x <= xSize; x++, i++)
                verts[i] = new Vector3(x, y);
   
        //назначаем вершины на меш
        mesh.vertices = verts;

        //генерируем треугольники
        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            for (int x = 0; x < xSize; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        //назначаем треугольники на меш
        mesh.triangles = triangles;

        //пересчитываем нормали
        mesh.RecalculateNormals();
    }

    void Coloring() {
        // генерируем массив цветов
        Color[] colors = new Color[verts.Length];
        for (int i = 0; i < verts.Length; i++)
            colors[i] = (i > 3000) ? Color.red : Color.green;

        //назначаем цвета на меш
        mesh.colors = colors;
    }
}
