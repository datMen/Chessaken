using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Piece : MonoBehaviour {

    [SerializeField]
    private string name;

    [System.Serializable]
    public class Move {  public int[] move = new int[2]; };
    [SerializeField] 
    public List<Move> allowed_moves;

    void Start() {
        switch (name) {
            case "Tower":
                setupTowerAllowedMoves();
                break;
            case "Horse":
                setupHorseAllowedMoves();
                break;
            case "Bishop":
                setupBishopAllowedMoves();
                break;
            case "King":
                setupKingAllowedMoves();
                break;
        }
    }

    public bool checkValidMove(int[] move) {
        for (int i = 0; i < allowed_moves.Count ; i++) {
            if (move[0] == allowed_moves[i].move[0] && move[1] == allowed_moves[i].move[1]) return true;
        }
        return false;
    }

    private void addAllowedMove(int[] move) {
        Move new_move = new Move();
        new_move.move = move;
        allowed_moves.Add(new_move);
        Debug.Log(name + ": " + move[0] + ',' + move[1]);
    }

    private void setupTowerAllowedMoves() {
        for (int i = 1; i < 8; i++) {
            addAllowedMove(new int[] {i, 0});
            addAllowedMove(new int[] {0, i});
        }
    }

    private void setupBishopAllowedMoves() {
        for (int i = 1; i < 8; i++) {
            addAllowedMove(new int[] {i, -i});
        }
    }

    private void setupHorseAllowedMoves() {
        for (int i = 1; i < 3; i++) {
            for (int j = 1; j < 3; j++) {
                if (j != i) {
                    addAllowedMove(new int[] {i, j});
                    addAllowedMove(new int[] {-i, -j});
                    addAllowedMove(new int[] {i, -j});
                    addAllowedMove(new int[] {-i, j});
                }
            }
        }
    }

    // @FIXME: King allowed positions algorithm
    // [0, 1], [1, 1], [1, 0], [1, -1], [0, -1], [-1, -1], [-1, 0], [-1, 1]
    private void setupKingAllowedMoves() {
        // for (int i = 0; i < 2; i++) {
        //     for (int j = 0; j < 2; j++) {
        //         addAllowedMove(new int[] {i, j});
        //         addAllowedMove(new int[] {-i, -j});
        //         addAllowedMove(new int[] {i, -j});
        //         addAllowedMove(new int[] {-i, j});
        //     }
        // }
    }
}