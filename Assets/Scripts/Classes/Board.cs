using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour {
    [SerializeField]
    List<Coordinate> board = new List<Coordinate>();

    void Start() {
        addBoardCoordinates();
    }

    private void addBoardCoordinates() {
        int coor_x = 0;
        int coor_y = 0;
        for (int i = 0; i < board.Count ; i++) {
            board[i].x = coor_x;
            board[i].y = coor_y;
            Debug.Log("TR: " + board[i].position + " x: " + board[i].x + " y: " + board[i].y);

            if (coor_y > 0 && coor_y % 7 == 0) coor_x++;
            coor_y++;
        }
    }
}
