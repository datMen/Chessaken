using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Piece : MonoBehaviour {
    private List<Move> allowed_moves = new List<Move>();
    private Coordinate cur_coor;
    private bool started;

    [SerializeField]
    private string name;

    [SerializeField]
    private int team; // Whites = -1, Blacks = 1

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
            case "Queen":
                addTowerAllowedMoves();
                addBishopAllowedMoves();
                break;
            case "King":
                addKingAllowedMoves();
                break;
        }
    }

    public void movePiece(Coordinate coor) {
        int[] move = getMove(coor);

        if (checkValidMove(move[0], move[1])) {
            cur_coor = coor;
            if (!started) started = true;
        }
        else {
            Debug.Log("YOU CANT MOVE THERE: x:" + move[0] + " y: " + move[1]);
        }

        transform.position = new Vector3(cur_coor.pos.x, transform.position.y, cur_coor.pos.z);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void setStartCoor(Coordinate coor) {
        cur_coor = coor;
    }

    public int[] getMove(Coordinate coor) {
        int coor_x = (coor.x - cur_coor.x) * team;
        int coor_y = (coor.y - cur_coor.y) * team;

        return new int[] { coor_x, coor_y };
    }

    public bool checkValidMove(int coor_x, int coor_y) {
        for (int i = 0; i < allowed_moves.Count ; i++) {
            if (coor_x == allowed_moves[i].x && coor_y == allowed_moves[i].y) {
                if (allowed_moves[i].type != MoveType.StartOnly) {
                    return true;
                }
                else if (!started) {
                    return true;
                }
            }
        }
        return false;
    }

    private void addAllowedMove(int coor_x, int coor_y, MoveType type) {
        Move new_move = new Move(coor_x, coor_y, type);
        allowed_moves.Add(new_move);
    }

    private void addPawnAllowedMoves() {
        addAllowedMove(0, 1, MoveType.Move);
        addAllowedMove(0, 2, MoveType.StartOnly);
        addAllowedMove(1, 1, MoveType.Eat);
        addAllowedMove(-1, 1, MoveType.Eat);
    }

    private void addTowerAllowedMoves() {
        for (int coor_x = 1; coor_x < 8; coor_x++) {
            addAllowedMove(coor_x, 0, MoveType.EatMove);
            addAllowedMove(0, coor_x, MoveType.EatMove);
            addAllowedMove(-coor_x, 0, MoveType.EatMove);
            addAllowedMove(0, -coor_x, MoveType.EatMove);
        }
    }

    private void addBishopAllowedMoves() {
        for (int coor_x = 1; coor_x < 8; coor_x++) {
            addAllowedMove(coor_x, -coor_x, MoveType.EatMove);
            addAllowedMove(-coor_x, coor_x, MoveType.EatMove);
            addAllowedMove(coor_x, coor_x, MoveType.EatMove);
            addAllowedMove(-coor_x, -coor_x, MoveType.EatMove);
        }
    }

    private void addHorseAllowedMoves() {
        for (int coor_x = 1; coor_x < 3; coor_x++) {
            for (int coor_y = 1; coor_y < 3; coor_y++) {
                if (coor_y != coor_x) {
                    addAllowedMove(coor_x, coor_y, MoveType.EatMove);
                    addAllowedMove(-coor_x, -coor_y, MoveType.EatMove);
                    addAllowedMove(coor_x, -coor_y, MoveType.EatMove);
                    addAllowedMove(-coor_x, coor_y, MoveType.EatMove);
                }
            }
        }
    }

    // @FIXME: King allowed positions algorithm
    // [0, 1], [1, 1], [1, 0], [1, -1], [0, -1], [-1, -1], [-1, 0], [-1, 1]
    private void addKingAllowedMoves() {
        // for (int coor_x = 0; coor_x < 2; coor_x++) {
        //     for (int coor_y = 0; coor_y < 2; coor_y++) {
        //         addAllowedMove(coor_x, coor_y, MoveType.EatMove);
        //         addAllowedMove(-coor_x, -coor_y, MoveType.EatMove);
        //         addAllowedMove(coor_x, -coor_y, MoveType.EatMove);
        //         addAllowedMove(-coor_x, coor_y, MoveType.EatMove);
        //     }
        // }
    }
}