using System;
using System.Collections.Generic;

public class RollerCoasterPath {

    int resolutionX;
    int resolutionY;
    int lifes;
    int[,] map;
    int[,] pathMap;

    public RollerCoasterPath(int resX, int resY) {
        resolutionX = resX;
        resolutionY = resY;
        lifes = (resolutionX * resolutionY) / 3;
        map = new int[resX, resY];
        pathMap = new int[resX, resY];
    }

    public void clear() {
        lifes = (resolutionX * resolutionY) / 4;
        map = new int[resolutionX, resolutionY];
    }

    public int[] pointInside() {
        Random r = new Random();
        List<int[]> points = new List<int[]>();

        for (int i = 0; i < resolutionX; i++) {
            for (int y = 0; y < resolutionY; y++) {
                if (map[i, y] == 1) {
                    points.Add(new int[] { i, y });
                }
            }
        }

        return points[r.Next(0, points.Count - 1)];
    }

    public void placeInitial() {
        Random r = new Random();
        bool set = false;

        while (!set) {
            int x = r.Next(0, resolutionX);
            int y = r.Next(0, resolutionY);

            int lengthX = r.Next(1, resolutionX / 5);
            int lengthY = r.Next(1, resolutionY / 5);

            if (x - lengthX > 1 && y - lengthY > 1 && x + lengthX < resolutionX - 1 && y + lengthY < resolutionY - 1) {
                set = true;
                for (int i = x - lengthX; i < x + lengthX; i++) {
                    for (int j = y - lengthY; j < y + lengthY; j++) {
                        map[i, j] = 1;
                        lifes--;
                    }
                }
            }

        }
    }

    public void smooth() {
        for (int i = 1; i < resolutionX - 1; i++) {
            for (int j = 1; j < resolutionY - 1; j++) {
                if (map[i, j] == 0 && needSmooth(i, j)) {
                    map[i, j] = 1;
                    lifes--;
                    // Console.WriteLine("Got it! " + i + " " + j);
                }
            }
        }
    }

    public bool needSmooth(int x, int y) {
        int counter = 0;
        if (map[x + 1, y] == 1) {
            counter++;
        }
        if (map[x - 1, y] == 1) {
            counter++;
        }
        if (map[x, y + 1] == 1) {
            counter++;
        }
        if (map[x, y - 1] == 1) {
            counter++;
        }

        if (counter >= 3) {
            return true;
        } else {
            return false;
        }
    }

    public void placeSquare() {
        Random r = new Random();
        bool set = false;

        while (!set) {
            int[] point = pointInside();
            int x = point[0];
            int y = point[1];

            int lengthX = r.Next(1, resolutionX / 5);
            int lengthY = r.Next(1, resolutionY / 5);

            if (x - lengthX > 1 && y - lengthY > 1 && x + lengthX < resolutionX - 1 && y + lengthY < resolutionY - 1) {
                set = true;
                for (int i = x - lengthX; i < x + lengthX; i++) {
                    for (int j = y - lengthY; j < y + lengthY; j++) {
                        if (map[i, j] == 0) {
                            map[i, j] = 1;
                            lifes--;
                        }
                    }
                }
            }

        }
    }

    public void placePath() {
        for (int i = 0; i < resolutionX - 1; i++) {
            for (int j = 0; j < resolutionY - 1; j++) {
                if (map[i, j] == 0 && map[i + 1, j] == 1) {
                    map[i, j] = 2;
                }

                if (map[i, j] == 1 && map[i + 1, j] == 0) {
                    map[i, j] = 2;
                }
                if (map[i, j] == 0 && map[i, j + 1] == 1) {
                    map[i, j] = 2;
                }
                if (map[i, j] == 1 && map[i, j + 1] == 0) {
                    map[i, j] = 2;
                }
            }
        }
    }

    public void clearQuads() {
        for (int i = 0; i < resolutionX - 1; i++) {
            for (int j = 0; j < resolutionY - 1; j++) {
                if (map[i, j] == 1) map[i, j] = 0;
            }
        }
    }


    public void generate() {
        clear();
        int counter = 1;
        placeInitial();
        while (lifes > 0) {
            placeSquare();
            counter++;
            if (counter > 150) {
                map = null;
                break;
            }
        }
        if (map != null) {
            smooth();
            placePath();
            clearQuads();
            smoothPath();
        }
    }

    public void smoothPath() {
        for (int i = 0; i < resolutionX - 1; i++) {
            for (int j = 0; j < resolutionY - 1; j++) {
                if (map[i, j] == 0 && map[i + 1, j] == 2 && map[i, j + 1] == 2 && map[i + 1, j + 1] != 2) {
                    map[i, j] = 2;
                }
            }
        }
    }


    public int[,] getMap() {
        map = null;
        while (map == null) {
            generate();
        }
        return map;
    }

    public void showMap() {
        for (int i = 0; i < resolutionX; i++) {
            for (int j = 0; j < resolutionY; j++) {
                Console.Write(map[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    public int[] getNext(int[] cur, int[] prev) {
        int[] next = null;
        if (pathMap[cur[0] + 1, cur[1]] == 2 && cur[0] + 1 != prev[0]) {
            next = new int[] { cur[0] + 1, cur[1] };
        }
        if (pathMap[cur[0], cur[1] + 1] == 2 && cur[1] + 1 != prev[1]) {
            next = new int[] { cur[0], cur[1] + 1 };
        }
        if (pathMap[cur[0] - 1, cur[1]] == 2 && cur[0] - 1 != prev[0]) {
            next = new int[] { cur[0] - 1, cur[1] };
        }
        if (pathMap[cur[0], cur[1] - 1] == 2 && cur[1] - 1 != prev[1]) {
            next = new int[] { cur[0], cur[1] - 1 };
        }
        if (next != null) {
            pathMap[next[0], next[1]] = 0;
        }
        return next;
    }

    public List<int[]> getPath() {
        List<int[]> path = new List<int[]>();
        int[] current = null;
        int[] previous = null;
        int[] start = null;
        pathMap = map;

        bool getIt = false;
        for (int i = 0; i < resolutionX; i++) {
            for (int j = 0; j < resolutionY; j++) {
                if (pathMap[i, j] == 2) {
                    start = current = new int[] { i, j };
                    getIt = true;
                    break;
                }
            }
            if (getIt) break;
        }

        previous = current;
        pathMap[current[0], current[1]] = 0;

        while (current != null) {
            path.Add(current);
            int[] temp = current;
            current = getNext(current, previous);
            previous = temp;
        }


        return path;
    }

}