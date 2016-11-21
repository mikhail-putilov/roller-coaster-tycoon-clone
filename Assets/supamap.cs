using UnityEngine;
using System.Collections;



public class supamap : MonoBehaviour {

    [Header("Map")]
    public int xSize = 500;
    public int ySize = 500;
    [Header("Zones")]
    public Color grass;
    public Color footway;
    public Color attraction;
    [Range(75, 125)]
    public int attractionSize = 100;
    public bool randomizeAttr = true;
    public Color food;
    [Range(1, 3)]
    public int foodLimit = 1;
    public Color wc;
    [Range(1, 2)]
    public int wcLimit = 2;

    public bool drawGrid = true;
    public int gridSize = 10;
    public GameObject rollerCoaster;



    Texture2D tex;                                  //текстура
    MeshRenderer mr;                                //юнитевский компанент куда будет вешать текстуру
    int[,] intMap;                                  //карта в массиве интов

    // Use this for initialization
    void Start() {

        Debug.Log(this.transform.position.z);

        tex = new Texture2D(500, 500);          //текстурка, в сущности двумерный массив 
        mr = GetComponent<MeshRenderer>();          //кешируем меш рендерер для обновления текстурки

        DrawMap();
    }
	static void Swap<T>(ref T lhs, ref T rhs)
	{
		T temp;
		temp = lhs;
		lhs = rhs;
		rhs = temp;
	}
	static float toAbsCoord(float _internal) {
		return 300 - _internal / 5;
	}
//    //placement of roller coaster using rollerCoaster game object prefab
//    public void placeRollerCoaster(int dir, float x, float z, float width, float height) {
//		if (dir < 0) {
//			Swap<float>(ref x, ref z);
//			Swap<float> (ref width, ref height);
//			Debug.Log("swaped!");
//		}
//		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
//		cube.transform.position = new Vector3 (toAbsCoord(x), this.transform.position.y, toAbsCoord(z));
//		GameObject cube2 = GameObject.CreatePrimitive (PrimitiveType.Cube);
//		cube2.transform.position = new Vector3 (toAbsCoord(x + width), this.transform.position.y, toAbsCoord(z + height));
//		//попробуй поставить роллер костер в (0, 0, 0)
//		GameObject _rollerCoaster = (GameObject)Instantiate(rollerCoaster);// roller coaster instantiation
//        
//		_rollerCoaster.transform.position = new Vector3(toAbsCoord(x+width),
//			this.transform.position.y,
//			toAbsCoord(z+height));
////        _rollerCoaster.GetComponent<GridController>().generate((int)width, (int)height); //generating coaster at given zone
//
////        _rollerCoaster.transform.parent = this.transform;// ... and placement
//
////		_rollerCoaster.transform.lossyScale.Scale(new Vector3(0.15f, 0.15f, 0.15f));
//
//    }
	public void placeRollerCoaster(int dir, float x, float z, float width, float height) {
		if (dir < 0) {
			Swap<float>(ref x, ref z);
			Swap<float> (ref width, ref height);
			Debug.Log("swaped!");
		}
		GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube.transform.position = new Vector3 (toAbsCoord(x), this.transform.position.y, toAbsCoord(z));
		GameObject cube2 = GameObject.CreatePrimitive (PrimitiveType.Cube);
		cube2.transform.position = new Vector3 (toAbsCoord(x + width), this.transform.position.y, toAbsCoord(z + height));




		//попробуй поставить роллер костер в (0, 0, 0)
		GameObject _rollerCoaster = (GameObject)Instantiate(rollerCoaster, Vector3.zero, this.transform.rotation);// roller coaster instantiation
		_rollerCoaster.GetComponent<GridController>().generate(32, 32); //generating coaster at given zone

		_rollerCoaster.transform.position = new Vector3(toAbsCoord(x+width),
			this.transform.position.y,
			toAbsCoord(z+height));

		//        _rollerCoaster.transform.parent = this.transform;// ... and placement

		//		_rollerCoaster.transform.lossyScale.Scale(new Vector3(0.15f, 0.15f, 0.15f));

	}

    public int dir;
    public void DrawMap() {
        intMap = new int[500, 500];
        FillBG();                       //закрашиваем текстрку

        dir = (Random.Range(0, 2) * 2 - 1); //выбираем направение первой дороги

        Vector2 a = new Vector2(Random.Range(75, 125), Random.Range(225, 275)); //рандомим несколько точек
        Vector2 b = new Vector2(Random.Range(375, 425), a.y);
        int cdir = (Random.Range(0, 2) * 2 - 1);
        Vector2 c = new Vector2(a.x, a.y + Random.Range(50, 100) * cdir);
        int ddir = (Random.Range(0, 2) * 2 - 1);
        Vector2 d = new Vector2(b.x, b.y + Random.Range(50, 100) * ddir);




        if (randomizeAttr)
            attractionSize = Random.Range(75, 125);
        Vector2 e;  //посленюю рандомим исходя из предыдущих
        if (cdir > 0 && ddir > 0) { // и тут же рисуем мелкие зоны
            e = new Vector2(Random.Range(c.x + 30, d.x - 30), a.y - Random.Range(50, 100));
            DrawRect(new Rect(e.x, a.y + 15, 30, 30), wc);
            DrawRect(new Rect(e.x + 30 * (Random.Range(0, 2) * 2 - 1), a.y - 30, 30, 30), food);
            DrawRect(new Rect(e.x - attractionSize / 2, e.y - attractionSize, attractionSize, attractionSize), attraction); //и сразу рисуем зону
            try {
				placeRollerCoaster(dir, e.x - attractionSize / 2, e.y - attractionSize, attractionSize, attractionSize);
            } catch (System.Exception exc) {
                Debug.LogException(exc, this);
                Debug.Log("Coordinates: " + (e.x - attractionSize / 2) + " " + (e.y - attractionSize) + " " + (attractionSize) + (attractionSize));
            }
        } else if (cdir < 0 && ddir < 0) {
            e = new Vector2(Random.Range(c.x + 30, d.x - 30), a.y + Random.Range(50, 100));
            DrawRect(new Rect(e.x + 30 * (Random.Range(0, 2) * 2 - 1), a.y + 15, 30, 30), food);
            DrawRect(new Rect(e.x - attractionSize / 2, e.y, attractionSize, attractionSize), attraction);
            try {
				placeRollerCoaster(dir, e.x - attractionSize / 2, e.y, attractionSize, attractionSize);
            } catch (System.Exception exc) {
                Debug.LogException(exc, this);
                Debug.Log("Coordinates: " + (e.x - attractionSize / 2) + " " + (e.y - attractionSize) + " " + (attractionSize) + (attractionSize));
            }
        } else if (cdir > 0 && ddir < 0) {
            e = new Vector2(Random.Range(250, d.x - 30), a.y + Random.Range(50, 100));
            DrawRect(new Rect(e.x - 45, a.y + 15, 30, 30), wc);
            DrawRect(new Rect(e.x - 30, a.y - 30, 30, 30), food);
            DrawRect(new Rect(e.x - attractionSize / 2, e.y, attractionSize, attractionSize), attraction);
            try {
				placeRollerCoaster(dir, e.x - attractionSize / 2, e.y, attractionSize, attractionSize);
            } catch (System.Exception exc) {
                Debug.LogException(exc, this);
                Debug.Log("Coordinates: " + (e.x - attractionSize / 2) + " " + (e.y - attractionSize) + " " + (attractionSize) + (attractionSize));
            }
        } else {
            e = new Vector2(Random.Range(c.x + 30, 250), a.y + Random.Range(50, 100));
            DrawRect(new Rect(e.x - attractionSize / 2, e.y, attractionSize, attractionSize), attraction);
            DrawRect(new Rect(e.x + 45, a.y + 15, 30, 30), food);
            try {
				placeRollerCoaster(dir, e.x - attractionSize / 2, e.y, attractionSize, attractionSize);
            } catch (System.Exception exc) {
                Debug.LogException(exc, this);
                Debug.Log("Coordinates: " + (e.x - attractionSize / 2) + " " + (e.y - attractionSize) + " " + (attractionSize) + (attractionSize));
            }
        }
        if (e.x > 250) {
            DrawRect(new Rect(c.x + 15, a.y - 30, 30, 30), wc);
            DrawRect(new Rect(c.x + 15, a.y - 30, 30, 30), wc);
        }
        if (e.x < 250) {
            DrawRect(new Rect(d.x - Random.Range(20, 45), a.y - 30, 30, 30), wc);
            DrawRect(new Rect(d.x - Random.Range(75, 90), a.y - 30, 30, 30), wc);
        }



        if (randomizeAttr) //рисуем остальные зоны
            attractionSize = Random.Range(75, 125);
		if (cdir < 0) {
			DrawRect (new Rect (c.x - attractionSize / 2, c.y - attractionSize, attractionSize, attractionSize), attraction);
			this.placeRollerCoaster (dir, c.x - attractionSize / 2, c.y - attractionSize, attractionSize, attractionSize);
		} else {
			DrawRect (new Rect (c.x - attractionSize / 2, c.y, attractionSize, attractionSize), attraction);
			this.placeRollerCoaster (dir, c.x - attractionSize / 2, c.y, attractionSize, attractionSize);
		}
        if (randomizeAttr)
            attractionSize = Random.Range(75, 125);

		if (ddir < 0) {
			DrawRect (new Rect (d.x - attractionSize / 2, d.y - attractionSize, attractionSize, attractionSize), attraction);
			this.placeRollerCoaster (dir, d.x - attractionSize / 2, d.y - attractionSize, attractionSize, attractionSize);
		} else {
			DrawRect (new Rect (d.x - attractionSize / 2, d.y, attractionSize, attractionSize), attraction);
			this.placeRollerCoaster (dir, d.x - attractionSize / 2, d.y, attractionSize, attractionSize);

		}


        //рисуем пути
        DrawRect(new Rect(a.x, a.y, Mathf.Abs(a.x - b.x) + 15, 15), footway);
        DrawRect(new Rect(a.x, Mathf.Min(c.y, a.y), 15, Mathf.Abs(a.y - c.y)), footway);
        DrawRect(new Rect(b.x, Mathf.Min(d.y, b.y), 15, Mathf.Abs(b.y - d.y)), footway);
        DrawRect(new Rect(e.x, Mathf.Min(e.y, a.y), 15, Mathf.Abs(a.y - e.y)), footway);



        DrawGrid(); //рисуем сетку

        tex.Apply();                    //вызывать всегда после редактирования массиви текстурки
        mr.sharedMaterials[0].mainTexture = tex; //применяем текустуру к материалу

    }

    void DrawGrid() { // сетка
        if (drawGrid == false)
            return;
        for (int x = 0; x < xSize; x++)
            for (int y = 0; y < ySize; y = y + gridSize) {
                tex.SetPixel(x, y, Color.white);
                intMap[x, y] = (int)Color.white.r;
            }
        for (int x = 0; x < xSize; x = x + gridSize)
            for (int y = 0; y < ySize; y++) {
                tex.SetPixel(x, y, Color.white);
                intMap[x, y] = (int)Color.white.r;
            }
    }

    void FillBG() {
        for (int x = 0; x < 500; x++)
            for (int y = 0; y < 500; y++) {
                tex.SetPixel(x, y, grass);      //рисуем и попутно заполняем инты
                intMap[x, y] = (int)grass.r;    //так же везде в дальнейшем
            }
    }

    void DrawRect(Rect zone, Color col) {
        for (int i = 0; i < zone.width; i++)
            for (int j = 0; j < zone.height; j++) {
                if (dir > 0) {
                    tex.SetPixel((int)zone.x + i, (int)zone.y + j, col);
                    intMap[(int)zone.x + i, (int)zone.y + j] = (int)col.r;
                }
                if (dir < 0) {
                    tex.SetPixel((int)zone.y + j, (int)zone.x + i, col);
                    intMap[(int)zone.y + j, (int)zone.x + i] = (int)col.r;
                }
            }
    }

    // Update is called once per frame
    void Update() {

    }
}
