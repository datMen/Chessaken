using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Coordinate {
    public int x;
    public int y;
    public Transform position;
    public Vector3 pos;

    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
        pos = new Vector3(0, 0, 0);
    }
}