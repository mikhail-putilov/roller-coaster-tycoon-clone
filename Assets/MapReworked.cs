using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapReworked : MonoBehaviour {
    [Header("Map")]
    public int xSize = 500;
    public int ySize = 500;
    [Header("Zones")]
    public Color splits;
    public Color grass;
    public Color footway;
    public Color attraction;
    public Color food;
    [Range(1, 15)]
    public int foodLimit = 3;
    public Color wc;
    [Range(1, 5)]
    public int wcLimit = 2;
    public Color souvenir;
    [Range(1, 3)]
    public int souvenirLimit = 1;


    [Header("BSP settings")]
    [Range(1, 7)]
    public int recursionBase = 4;                  //выше 7ми уже есть задержка при подсчетах, но можно поэксперементировать с остальными параметрами
    [Range(3, 8)]
    public int offsetLimit = 3;                    // ограничение по приближенности сплита к середине зоны, выше -- ближе
    public bool drawZones = true;
    public bool drawGrid = true;
    public bool drawFootWays = true;
    public bool drawSplits = false;
    public bool randomizeEachDir = false;          //делать рандомной дирекцию каждого сплита, иначе чередовать по глубине рекурсии
    public int gridSize = 10;
    public int minZoneHeigth = 100;                  //ограничения на размера зон
    public int minZoneWidth = 100;
    public int minZoneSize = 10000;
    [Range(5, 15)]
    public int footwayWidth = 5;
    [Range(5, 15)]
    public int fillJitter;                         //"смещение" закрашиваемых зон


    List<Rect> zones;                               //список зон    
    Texture2D tex;                                  //текстура
    MeshRenderer mr;                                //юнитевский компанент куда будет вешать текстуру
    Rect map;                                       //карта
    int[,] intMap;                                  //карта в массиве интов



    //стандартный метод от MonoBehaviour, выполняется при интициализации объекта в юнити
    void Start() {

        Debug.Log("wtf");
        intMap = new int[xSize, ySize];             //инициализируем массив
        zones = new List<Rect>();                   //и коллекцию    
        map = new Rect(0, 0, xSize, ySize);         //оперировать будем ректами, ибо с интом циклы ниже потеряли бы какую либо вменяемость 
                                                    //docs.unity3d.com/ScriptReference/Rect.html                

        tex = new Texture2D(xSize, ySize);          //текстурка, в сущности двумерный массив 
        mr = GetComponent<MeshRenderer>();          //кешируем меш рендерер для обновления текстурки

        Roll();
    }



    //паблик метод генерации чтобы дергать с кнопки
    public void Roll() {
        zones.Clear();                  //очищаем коллекцию с зонами каждый новый запуск
        intMap = new int[xSize, ySize]; //как и массив
        FillBG();                       //закрашиваем текстрку
        BSPZoneSplit(map, 0);           //вызываем наш рекурсивный алгоритм
        FillZones();                    //заполняем зоны "обектами"
        DrawGrid();                     //сетка
        tex.Apply();                    //вызывать всегда после редактирования массиви текстурки
        mr.sharedMaterials[0].mainTexture = tex; //применяем текустуру к материалу
    }

    void DrawGrid() {
        if (drawGrid == false)
            return;
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y = y + gridSize) {
                tex.SetPixel(x, y, Color.black);
                intMap[x, y] = (int)Color.black.r;
            }
        for (int x = 0; x < xSize; x = x + gridSize)
            for (int y = 0; y < ySize; y++) {
                tex.SetPixel(x, y, Color.black);
                intMap[x, y] = (int)Color.black.r;
            }
    }

    //стандартное заполенение массива
    void FillBG() {
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y++) {
                tex.SetPixel(x, y, grass);      //рисуем и попутно заполняем инты
                intMap[x, y] = (int)grass.r;    //так же везде в дальнейшем
            }
    }




    //закраска ректа
    void DrawRect(Rect zone, Color col, int fj) {
        int heightJitter = fj;
        int widthJitter = fj;
        for (int i = widthJitter; i < zone.width - widthJitter; i++)
            for (int j = heightJitter; j < zone.height - heightJitter; j++) {
                tex.SetPixel((int)zone.x + i, (int)zone.y + j, col);    //рисуем в Texture2D и
                intMap[(int)zone.x + i, (int)zone.y + j] = (int)col.r;  //закидываем циферку в массив, будем везде брать r компаненту,
                                                                        //а в эдиторе прочекаем цвета, чтобы r была уникальной для каждого цвета
            }
    }


    //закраска зон
    void FillZones() {
        if (drawZones == false)
            return;
        int currentSouv = 0;
        int currentWc = 0;
        int currentFood = 0;
        zones = zones.OrderBy((Rect) => Rect.width * Rect.height).ToList(); //сортируем по возрастанию

        for (int i = 0; i < zones.Count; i++) {
            //            if (i % 3 == 0)
            //                continue;
            if (currentSouv < souvenirLimit) {
                DrawRect(zones.ElementAt(i), souvenir, 0);
                currentSouv++;
                continue;
            }

            if (currentWc < wcLimit) {
                DrawRect(zones.ElementAt(i), wc, 0);
                currentWc++;
                continue;
            }

            if (currentFood < foodLimit) {
                DrawRect(zones.ElementAt(i), food,15);
                currentFood++;
                continue;
            }
            DrawRect(zones.ElementAt(i), attraction,0);
        }
    }



    //отрисовка одной разделяюищей линии для визуализауции нащего алгоритма
    void DrawSplitInZone(Rect zone, int offset, int dir) {
        if (dir > 0)
            for (int i = 0; i < zone.width; i++) {
                tex.SetPixel((int)zone.x + i, (int)zone.y + offset, splits);
                intMap[(int)zone.x + i, (int)zone.y + offset] = (int)splits.r;
            } else
            for (int i = 0; i < zone.height; i++) {
                tex.SetPixel((int)zone.x + offset, (int)zone.y + i, splits);
                intMap[(int)zone.x + offset, (int)zone.y + i] = (int)splits.r;
            }
    }

    void DrawFootWayReworked( Rect ZoneA, Rect ZoneB) {
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(0, 0);

    }

    void DrawCornerLine(Vector2 a, Vector2 b) {
        tex.SetPixel((int)a.x, (int)a.y, Color.black);

        tex.SetPixel((int)b.x, (int)b.y, Color.black);
        int x1 = (int)a.x;
        int x2 = (int)b.x;
        int y1 = (int)a.y;
        int y2 = (int)b.y;
        if (x1 == x2) {
            for(int y = y1; y <= y2; y ++ )
                tex.SetPixel(x1, y, Color.black);
        }
        if (y1 == y2)
            for (int x = x1; x <= x2; x++)
                tex.SetPixel(x, y1, Color.black);

        /*
        else if (x1 > x2) {
            for (int y = 0; y < (y2 - y1)/2; y++)
                tex.SetPixel(x1, y, Color.black);
            for(int x = x2; x<x2; x++)
                tex.SetPixel(x, (y2 - y1) / 2, Color.black);
            for (int y = (y2 - y1) / 2; y < y2; y++)
                tex.SetPixel(x2, y, Color.black);
        }
        
        if (x1 <= x2 && y1 <= y2) {
            for (int x = x1; x <= (x2-x1) / 2; x++)
                tex.SetPixel(x, y1, Color.black);
            for (int y = y1; y <= y2; y++)
                tex.SetPixel((x2-x1) / 2, y, Color.black);
            for(int x = (x2-x1)/2; x<=x2; x ++)
                tex.SetPixel(x, y2, Color.black);
        }

        if (x1>=x2 && y1>=y2) {

            for (int x = x2; x <= (x1-x2) / 2; x++)
                tex.SetPixel(x, y2, Color.black);
            for (int y = y2; y <= y1 ; y++)
                tex.SetPixel((x1-x2) / 2, y, Color.black);
            for (int x = (x1 - x2) / 2; x <= x1; x++)
                tex.SetPixel(x, y1,Color.black);
        }

        if (x1 <= x2 && y1 > y2) {
            for (int x = x1; x <= (x2 - x1)/2 ; x++)
                tex.SetPixel(x, y1, Color.black);
            for (int y = y1; y <= y2; y++)
                tex.SetPixel((x2 - x1) / 2, y, Color.black);
            for (int x = (x2 - x1) / 2; x > x2; x++)
                tex.SetPixel(x, y2, Color.black);
        }

        if (x1 >= x2 && y1 < y2) {
            for (int x = x1; x <= (x2 - x1) / 2; x++)
                tex.SetPixel(x, y1, Color.black);
            for (int y = y1; y <= y2; y++)
                tex.SetPixel((x2 - x1) / 2, y, Color.black);
            for (int x = (x2 - x1) / 2; x > x2; x++)
                tex.SetPixel(x, y2, Color.black);
        }
        */

    }

    //отрисовка дорожек
    //кропотливая планарная геометрия
    //по сути, поиск путей нужно подгонять под конкретню релизацию
    //если это 3д, то в 3д и делать на плейсхолдорах место моделей
    //слишком уж много кастома
    //дороги можно хоть с поворотом на 90 делать, хоть зигзагом если захотеть
    void DrawFootWay(Rect zoneA, Rect zoneB) {
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(0, 0);
        int loopCount = 0;
        //тут горизонтальные дорожки
        if (zoneA.y == zoneB.y) {

            while (true) {
                a = new Vector2((int)zoneA.xMax - fillJitter, (int)Random.Range(zoneA.y, zoneA.yMax - footwayWidth - fillJitter));
                loopCount++;
                if (a.y > SearchRect(a).y + fillJitter && a.y < SearchRect(a).yMax - fillJitter - footwayWidth) {
                    b = new Vector2((int)a.x + fillJitter * 2, (int)a.y);   //вобщем-то вторая точка не нужна, использую для наглядности
                    if ((b.y > SearchRect(b).y + fillJitter) && (b.y < SearchRect(b).yMax - fillJitter - footwayWidth))
                        break;
                }
                if (loopCount > 100) {
                    Debug.LogWarning("It Seems zones are too small to bulid correct footways"); //если не получилось найти нормальную дорожку
                    Debug.Log("Try other settings");
                    break;
                }
            }
            DrawCornerLine(SearchRect(a).center, SearchRect(b).center);
            for (int i = 0; i < SearchRect(b).center.x - SearchRect(a).center.x; i++)
                for (int j = 0; j < footwayWidth; j++) {
                    tex.SetPixel((int)SearchRect(a).center.x + i, (int)SearchRect(a).center.y + j, footway);
                    //intMap[(int)a.x + i, (int)a.y + j] = (int)footway.r;
                }

            //а тут вертикальные
        } else {
            while (true) {

                a = new Vector2((int)Random.Range(zoneA.x, zoneA.xMax - footwayWidth - fillJitter), zoneA.yMax - fillJitter);
                loopCount++;
                if (a.x > SearchRect(a).x + fillJitter && a.x < SearchRect(a).xMax - fillJitter - footwayWidth) {
                    b = new Vector2((int)a.x, (int)a.y + fillJitter * 2);
                    if ((b.x > SearchRect(b).x + fillJitter + footwayWidth) && (b.x < SearchRect(b).xMax - fillJitter - footwayWidth))
                        break;
                }
                if (loopCount > 100) {
                    Debug.LogWarning("It Seems zones are too small to bulid correct footways");
                    Debug.Log("Try other settings");
                    break;
                }
            }
            DrawCornerLine(SearchRect(a).center, SearchRect(b).center);
            for (int i = 0; i < SearchRect(b).center.y - SearchRect(a).center.y; i++)
                for (int j = 0; j < footwayWidth; j++) {
                    tex.SetPixel((int)SearchRect(a).center.x + j, (int)SearchRect(a).center.y + i, footway);
                    //intMap[(int)a.x + j, (int)a.y + i] = (int)footway.r;
                }
        }
    }

    //вивел отдельны метод чтобы разгрузить предыдуший
    Rect SearchRect(Vector2 point) {
        List<Rect> t = zones.Where((rect) => (point.x >= rect.x && point.x <= rect.xMax) && (point.y >= rect.y && point.y <= rect.yMax)).ToList();
        return t.OrderBy((Rect) => Rect.width * Rect.height).ElementAt(0);
    }

    //тут рекурсивный bsp алгорим
    int dir = 0;
    void BSPZoneSplit(Rect zone, int deep) {

        if (deep == recursionBase || zone.height * zone.width * 2 < minZoneSize ||
            zone.height * 2 < minZoneHeigth || zone.width * 2 < minZoneWidth) {    //если достигли базы или минимального размера зоны
            zones.Add(zone);                              //закидываем зону в листи выходим из итерации
            return;
        }

        dir = dir == 0 ? 1 : 0; // поочередно меняем направление разреза на каждом уровне рекурсии
                                //0 вертикальный, 1 горизонтальный

        if (randomizeEachDir)
            dir = Random.Range(0, 2);   //либо выбираем направление разреза случайным образом

        Rect subZoneA = zone;
        Rect subZoneB = zone;


        //выбрана горизонтальная дирекция
        if (dir > 0) {
            //выбираем случайное смещение от середины плюс-минус
            int offset = (int)zone.height / 2 + (int)Random.Range(-zone.height / offsetLimit, zone.height / offsetLimit);

            subZoneA = new Rect(zone.x, zone.y, zone.width, offset);                           //вычисляем координаты подзон
            subZoneB = new Rect(zone.x, zone.y + offset, zone.width, zone.height - offset);    //немного арифметики        

            if (drawSplits) DrawSplitInZone(zone, offset, dir);                                //рисуем разрез если нужно
        }

        //аналогичная ситуация для вертикальной дирекции, только арифметика своя
        else {
            int offset = (int)zone.width / 2 + (int)Random.Range(-zone.width / offsetLimit, zone.width / offsetLimit);

            subZoneA = new Rect(zone.x, zone.y, offset, zone.height);
            subZoneB = new Rect(zone.x + offset, zone.y, zone.width - offset, zone.height);

            if (drawSplits) DrawSplitInZone(zone, offset, dir);
        }


        BSPZoneSplit(subZoneA, deep + 1);                         //рекурсивный вызов для первой подзоны
        BSPZoneSplit(subZoneB, deep + 1);                         //и для второй   

        dir = dir == 0 ? 1 : 0;

        if (drawFootWays) DrawFootWay(subZoneA, subZoneB);        //рисуем дорожку если нужно
    }

}
