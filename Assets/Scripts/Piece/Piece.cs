using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Piece : MonoBehaviour, PieceContext {
    private List<PieceState> states;
    private PieceState cur_state;
    private PieceStateId cur_state_id;

    [SerializeField]
    private string name;

    [SerializeField]
    private int direction;
    private List<Coordinate> allowed_moves = new List<Coordinate>();

    void Start() {
        startStates();
        startState();
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

    void Update () {
        if (cur_state != null) cur_state.onUpdate();
    }

    public void updateState(PieceStateId state) {
        cur_state.onLeave();
        cur_state = states[(int)state];
        cur_state.onEnter();
        cur_state_id = state;
    }

    void startState() {
        cur_state_id = PieceStateId.StandStill;
        cur_state = states[(int)cur_state_id];
        cur_state.onEnter();
    }

    void startStates() {
        string[] state_ids = Enum.GetNames(typeof(PieceStateId));
        states = new List<PieceState>();
        for (int i = 0; i < state_ids.Length ; i++) {
            object[] param = new [] { this };
            PieceState parsedState = Activator.CreateInstance( Type.GetType(state_ids[i], true), param) as PieceState;
            states.Add(parsedState);
        }
    }

    public PieceStateId getCurStateId() {
        return cur_state_id;
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
        }
    }

    private void addBishopAllowedMoves() {
        for (int coor_x = 1; coor_x < 8; coor_x++) {
            addAllowedMove(coor_x, -coor_x);
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

    public void startStandStill() {
        cur_state.startStandStill();
    }

    public void startMoving() {
        cur_state.startMoving();
    }
}