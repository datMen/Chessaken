using UnityEngine;
using System;

/*
==============================
[Theme] - Game theme class
==============================
*/
[System.Serializable]
public class Theme : System.Object {
    public string theme_name;
    public Material board_corner;
    public Material board_side;
    public Material piece_black;
    public Material piece_white;
    public Material square_black;
    public Material square_white;
    public Material square_closest;
    public Material square_hover;
}