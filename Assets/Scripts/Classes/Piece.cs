using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Piece : MonoBehaviour {
    private List<Coordinate> allowed_moves = new List<Coordinate>();
    private Coordinate cur_coor;
    private bool started;

    [SerializeField]
    private string name;

    [SerializeField]
    private int direction;

    void Start() {
        // Initialize valid moves
        switch (name) {
            case "Pawn":
                addPawnAllowedMoves();
                break;
            case "Tower":
                addTowerAllowedMoves();
                break;
            case "Horse":
                addHorseAllowedMoves();
                break;
            case "Bishop":
                addBishopAllowedMoves();
                break;
            case "King":
                addKingAllowedMoves();
                break;
        }
    }

    public void movePiece(Coordinate coor) {
        // if (start_position) {
        //     cur_coor = coor;
        //     start_position = false;
        //     return;
        // }

        int coor_x = (coor.x - cur_coor.x) * direction;
        int coor_y = (coor.y - cur_coor.y) * direction;

        if (checkValidMove(coor_x, coor_y)) {
            cur_coor = coor;
        }
        else {
            Debug.Log("YOU CANT MOVE THERE: x:" + coor_x + " y: " + coor_y);
        }

        transform.position = new Vector3(cur_coor.pos.x, transform.position.y, cur_coor.pos.z);
    }

    public void setStartCoor(Coordinate coor) {
        if (!started) {
            cur_coor = coor;
            started = true;
        }
    }

    public bool checkValidMove(int coor_x, int coor_y) {
        for (int i = 0; i < allowed_moves.Count ; i++) {
            if (coor_x == allowed_moves[i].x && coor_y == allowed_moves[i].y) return true;
        }
        return false;
    }

    private void addAllowedMove(int coor_x, int coor_y) {
        Coordinate new_move = new Coordinate(coor_x, coor_y);
        allowed_moves.Add(new_move);
        // Debug.Log(name + ": " + coor_x + ',' + coor_y);
    }

    private void addPawnAllowedMoves() {
        addAllowedMove(0, 1);
        addAllowedMove(0, 2);
        addAllowedMove(1, 1);
        addAllowedMove(1, -1);
    }

    private void addTowerAllowedMoves() {
        for (int coor_x = 1; coor_x < 8; coor_x++) {
            addAllowedMove(coor_x, 0);
            addAllowedMove(0, coor_x);
            addAllowedMove(-coor_x, 0);
            addAllowedMove(0, -coor_x);
        }
    }

    private void addBishopAllowedMoves() {
        for (int coor_x = 1; coor_x < 8; coor_x++) {
            addAllowedMove(coor_x, -coor_x);
            addAllowedMove(-coor_x, coor_x);
            addAllowedMove(coor_x, coor_x);
            addAllowedMove(-coor_x, -coor_x);
        }
    }

    private void addHorseAllowedMoves() {
        for (int coor_x = 1; coor_x < 3; coor_x++) {
            for (int coor_y = 1; coor_y < 3; coor_y++) {
                if (coor_y != coor_x) {
                    addAllowedMove(coor_x, coor_y);
                    addAllowedMove(-coor_x, -coor_y);
                    addAllowedMove(coor_x, -coor_y);
                    addAllowedMove(-coor_x, coor_y);
                }
            }
        }
    }

    // @FIXME: King allowed positions algorithm
    // [0, 1], [1, 1], [1, 0], [1, -1], [0, -1], [-1, -1], [-1, 0], [-1, 1]
    private void addKingAllowedMoves() {
        // for (int coor_x = 0; coor_x < 2; coor_x++) {
        //     for (int coor_y = 0; coor_y < 2; coor_y++) {
        //         addAllowedMove(coor_x, coor_y);
        //         addAllowedMove(-coor_x, -coor_y);
        //         addAllowedMove(coor_x, -coor_y);
        //         addAllowedMove(-coor_x, coor_y);
        //     }
        // }
    }
}