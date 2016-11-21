using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {

    [Range(16, 128)]
    public int resolutionX = 32; // resolution of the grid
    [Range(16, 128)]
    public int resolutionY = 32; // resolution of the grid

    public GameObject plane; // GO for empty part of grid
    public GameObject rollerPath; //GO for grid cells containing a part of a rollercoaster
    public GameObject straightPath;
    public GameObject leftTurn;
    public GameObject rightTurn;
    public GameObject toUpPath;
    public GameObject toDownPath;
    public GameObject stopUpPath;
    public GameObject stopDownPath;
    public GameObject upPath;
    public GameObject downPath;
	public string _tag;
    private GameObject[,] grid; //grid itself

//    void Awake() {
//        generate(resolutionX, resolutionY);
//    }

    public void generate(int _resolutionX, int _resolutionY) {
        clearArea();
        grid = new GameObject[resolutionX, resolutionY];

        resolutionX = _resolutionX;
        resolutionY = _resolutionY;

        RollerCoasterPath rcp = new RollerCoasterPath(resolutionX, resolutionY);
        int[,] map = rcp.getMap();
        float done = 0;

        // CreateMap(map);
        List<int[]> path = rcp.getPath();
        path.Add(path[0]);
        float[] result = { 0, 0 };
        while (done != 1) {
            clearAll();
            result = BuildCoaster(path);

            done = result[0];
            if (result[1] < 3) {
                done = 0;
            }
        }
    }

    public void clearAll() {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }


    public float[] BuildCoaster(List<int[]> path) {
        float maxHeight = 1;
        float height = 1;
        int forGrammar = 0;
        int remains = getActualLength(path) + 1;
        string liter = "s";
        bool isturn = false;
        bool turnNear = false;
        bool isRight = false;
        int rotation = 0;
        if (isTurn(path[path.Count - 1], path[0], path[2])) {
            if (isRightTurn(path[path.Count - 1], path[0], path[2])) {
                liter = "r";
            } else {
                liter = "l";
            }
        }
        //path.Add(path[0]);
        for (int i = 0; i < path.Count - 1; i++) {
            if (path[i][0] == path[i + 1][0] && path[i][1] > path[i + 1][1]) {
                rotation = 0;
            }
            if (path[i][0] == path[i + 1][0] && path[i][1] < path[i + 1][1]) {
                rotation = 180;
            }
            if (path[i][1] == path[i + 1][1] && path[i][0] > path[i + 1][0]) {
                rotation = 90;
            }
            if (path[i][1] == path[i + 1][1] && path[i][0] < path[i + 1][0]) {
                rotation = 270;
            }

            try {
                instantiate(path[i], liter, 1 + height, rotation);
            } catch {
                Debug.Log("yoba, ti vse proebal");
            }

            if (i + 1 < path.Count - 1) {
                isturn = isTurn(path[i], path[i + 1], path[i + 2]);
                isRight = isRightTurn(path[i], path[i + 1], path[i + 2]);
            }

            if (i + 2 < path.Count - 1) {
                turnNear = isTurn(path[i + 1], path[i + 2], path[i + 3]);
            }

            if (String.Compare(liter, "tu") == 0 || String.Compare(liter, "u") == 0 || String.Compare(liter, "tsu") == 0) {
                height += (float)0.666;
                if (height > maxHeight) {
                    maxHeight = height;
                }
                forGrammar++;
            }

            if (String.Compare(liter, "td") == 0 || String.Compare(liter, "d") == 0 || String.Compare(liter, "tsd") == 0) {
                height -= (float)0.666;
                forGrammar--;
            }

            if (!isturn && !turnNear) {
                remains--;
            }

            liter = Grammar.getNext(liter, isturn, isRight, turnNear, forGrammar + 1, remains);


            Debug.Log(liter + " " + forGrammar + " " + remains);

        }
        float[] result = { 0, 0 };
        if (Mathf.Round(height) == 1) {
            result[0] = 1;
        } else {
            result[0] = 0;
        }
        result[1] = maxHeight;
        return result;
    }

    public void instantiate(int[] coordinate, string name, float height, int rotate) {
		GameObject gridPlane = null;
        if (String.Compare(name, "s") == 0) {
             gridPlane = (GameObject)Instantiate(straightPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "u") == 0) {
             gridPlane = (GameObject)Instantiate(upPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "d") == 0) {
             gridPlane = (GameObject)Instantiate(downPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "td") == 0) {
             gridPlane = (GameObject)Instantiate(toDownPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "tu") == 0) {
             gridPlane = (GameObject)Instantiate(toUpPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "l") == 0) {
             gridPlane = (GameObject)Instantiate(leftTurn);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "r") == 0) {
             gridPlane = (GameObject)Instantiate(rightTurn);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "tsu") == 0) {
             gridPlane = (GameObject)Instantiate(stopUpPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
        if (String.Compare(name, "tsd") == 0) {
             gridPlane = (GameObject)Instantiate(stopDownPath);
            gridPlane.transform.parent = this.transform;
            gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + coordinate[0],
                                                        gridPlane.transform.position.y + height,
                                                        gridPlane.transform.position.z + coordinate[1]);
            gridPlane.transform.Rotate(0, rotate, 0);
            grid[coordinate[0], coordinate[1]] = gridPlane;
        }
		gridPlane.tag = _tag;
    }

    public bool isTurn(int[] cur, int[] next, int[] after) {
        int x = cur[0];
        int y = cur[1];
        char in_Dif = 'x';
        char fin_Dif = 'x';
        if (cur[0] != next[0]) {
            in_Dif = 'x';
        }
        if (cur[1] != next[1]) {
            in_Dif = 'y';
        }
        if (after[0] != next[0]) {
            fin_Dif = 'x';
        }
        if (after[1] != next[1]) {
            fin_Dif = 'y';
        }
        if (fin_Dif != in_Dif) {
            return true;
        } else return false;
    }


    public bool isRightTurn(int[] cur, int[] next, int[] after) {
        int indifX = 0;
        int indifY = 0;
        int difX = 0;
        int difY = 0;

        indifX = next[0] - cur[0];
        indifY = next[1] - cur[1];

        difX = after[0] - next[0];
        difY = after[1] - next[1];

        if (indifX > 0) {
            if (difY > 0) {
                return true;
            } else {
                return false;
            }
        }

        if (indifX < 0) {
            if (difY < 0) {
                return true;
            } else {
                return false;
            }
        }

        if (indifY < 0) {
            if (difX > 0) {
                return true;
            } else {
                return false;
            }
        }

        if (indifY > 0) {
            if (difX < 0) {
                return true;
            } else {
                return false;
            }
        }

        return false;

    }

    public int getActualLength(List<int[]> path) {
        int result = 0;
        for (int i = 0; i < path.Count; i++) {
            try {
                if (!isTurn(path[i], path[i + 1], path[i + 2]) && !isTurn(path[i + 1], path[i + 2], path[i + 3]))
                    result++;
            } catch {
                continue;
            }
        }
        return result;
    }


    public void CreateMap(int[,] map) {
        grid = new GameObject[resolutionX, resolutionY];

        for (int x = 0; x < resolutionX; x++) {
            for (int z = 0; z < resolutionY; z++) {
                if (map[x, z] == 0) {
                    GameObject gridPlane = (GameObject)Instantiate(plane);
                    gridPlane.transform.parent = this.transform;
                    gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + x,
                                                                gridPlane.transform.position.y,
                                                                gridPlane.transform.position.z + z);
                    grid[x, z] = gridPlane;
                }
                if (map[x, z] == 2) {
                    GameObject gridPlane = (GameObject)Instantiate(rollerPath);
                    gridPlane.transform.parent = this.transform;
                    gridPlane.transform.position = new Vector3(gridPlane.transform.position.x + x,
                                                                gridPlane.transform.position.y,
                                                                gridPlane.transform.position.z + z);
                    grid[x, z] = gridPlane;
                }
            }
        }

    }

    public void clearArea() {
        try {
            GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");
            foreach (GameObject go in planes) {
                GameObject.Destroy(go);
            }
        } catch {
            Debug.Log("No plane?");
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
