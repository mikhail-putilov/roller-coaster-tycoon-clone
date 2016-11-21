using UnityEngine;
using System.Collections;

public class BuildInTerrainHeights : MonoBehaviour {

    public bool randomNoise = true;
    [Range(0, 500)]
    public float noiseOffset = 0f;
    public Texture2D preMadeScaleMap;  //градиент из фотошопа
    [Header("Perlin")]
    [Range(1, 20)]
    public float scale = 10.0F;
    [Range(5, 40)]
    public int clifsHeightFactor = 10; //параметр определяющий максимальную высоту, меньше -- выше
    [Header("Flat Area")]
    public bool isSquare = false; //круглая или квадратная
    [Range(0.01f, 1)]
    public float R = 0.5f; //радиус плоской зоны
    public Gradient g;  //сам градиено по которому будет сглаживание холмиков
    [Range(0, 1)]
    public float x0 = 0.49f;    //центр нашей плоской зоны
    [Range(0, 1)]
    public float y0 = 0.52f;
    public bool updateEachFrame = true; // для нагладности

    Texture2D noiseMap; //карта с шумом 
    Texture2D gradientMap; //градиент для плавных переходов
    Terrain terr;
    float[,] heights; //массив с вершинами



    void Start() {
        noiseMap = new Texture2D(511, 511); //513 -- волшебная цифра из настроек террейна...
        gradientMap = new Texture2D(511, 511); //минус два чтобы внешние "стенки" были закрытые

        terr = GetComponent<Terrain>();
        heights = new float[terr.terrainData.heightmapWidth, terr.terrainData.heightmapHeight];

        Roll();
    }



    //для кнопки
    public void Roll() {
        PerlinToTexture();
        GenerateGradient();
        applyGradient();
        HeightsFromTexture();
    }



    //генерируем градиент
    void GenerateGradient() {
        for (float x = 0; x < gradientMap.width; x++)
            for (float y = 0; y < gradientMap.height; y++) {
                Vector2 r = new Vector2(x / gradientMap.width - x0, y / gradientMap.height - y0); //точка на окружности
                if (isSquare) //для квадратной зоны
                    gradientMap.SetPixel((int)x, (int)y, g.Evaluate(Mathf.Max((1 / R) * Mathf.Abs(r.x), (1 / R) * Mathf.Abs(r.y))));
                else //для круглой
                    gradientMap.SetPixel((int)x, (int)y, g.Evaluate((1 / R) * r.magnitude)); //Evaluate принимает 0..1 возвращает цвет из юнитевского градиента 
            }
    }



    //запись перлина в промежуточную текстуру чтобы потом сгладить
    void PerlinToTexture() {
        if (randomNoise == true)
            noiseOffset = Random.Range(0, 500);

        for (float x = 0; x < noiseMap.width; x++)
            for (float y = 0; y < noiseMap.height; y++) {
                noiseMap.SetPixel((int)x, (int)y, new Color(Mathf.PerlinNoise((x + noiseOffset) / noiseMap.width * scale, (y + noiseOffset) / noiseMap.height * scale), 0, 0)); //в красную компаненту rgb
            }
        noiseMap.Apply();
    }



    //применяем готовый градиент к текстурке с шумом
    void applyPreMadeGradient() {
        for (float x = 0; x < preMadeScaleMap.width; x++)
            for (float y = 0; y < preMadeScaleMap.height; y++) {
                noiseMap.SetPixel((int)x, (int)y, new Color(noiseMap.GetPixel((int)x, (int)y).r * preMadeScaleMap.GetPixel((int)x, (int)y).g, 0, 0)); //перемножаем красную компаненту с серой из градиента
            }
        noiseMap.Apply();
    }


    //применяем процедурный градиент к текстуре
    void applyGradient() {
        for (float x = 0; x < preMadeScaleMap.width; x++)
            for (float y = 0; y < preMadeScaleMap.height; y++) {
                noiseMap.SetPixel((int)x, (int)y, new Color(noiseMap.GetPixel((int)x, (int)y).r * gradientMap.GetPixel((int)x, (int)y).g, 0, 0)); //перемножаем красную компаненту с серой из градиента
            }
        noiseMap.Apply();
    }



    //запись высот в терейн из текстуры
    void HeightsFromTexture() {
        for (int i = 1; i < noiseMap.width; i++)
            for (int j = 1; j < noiseMap.height; j++) {
                heights[i, j] = noiseMap.GetPixel(i, j).r / clifsHeightFactor;
            }
        terr.terrainData.SetHeights(0, 0, heights);
    }



    //запись высот из юнитевского перлина напрямую в террейн
    void HeightsFromPerlin() {
        for (int x = 0; x < terr.terrainData.heightmapWidth; x++)
            for (int y = 0; y < terr.terrainData.heightmapHeight; y++) {
                heights[x, y] = Mathf.PerlinNoise(((float)x / terr.terrainData.heightmapWidth) * scale, ((float)y / terr.terrainData.heightmapHeight) * scale) / clifsHeightFactor;
            }
        terr.terrainData.SetHeights(0, 0, heights);
    }


    void Update() {
        if (updateEachFrame) {
            Roll();
        }

    }


}
