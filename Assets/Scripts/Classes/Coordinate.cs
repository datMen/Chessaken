using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Coordinate {
    public int x;
    public int y;
    public Transform position;

    public Coordinate(int x, int y) {
        this.x = x;
        this.y = y;
    }
}