using UnityEngine;

/*
==============================
[Coordinate] - Board "local" coordinate
==============================
*/
public class Coordinate {
    public int x;
    public int y;
    public Vector3 pos; // Transform position in the scene

    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
        pos = new Vector3(0, 0, 0);
    }
}