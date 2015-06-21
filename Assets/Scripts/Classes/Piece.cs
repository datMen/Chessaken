using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Piece : MonoBehaviour {
    private List<Move> allowed_moves = new List<Move>();
    private Square cur_square;
    private bool started;
    private List<Coordinate> break_points = new List<Coordinate>();

    [SerializeField]
    private string piece_name;

    [SerializeField]
    private int team; // Whites = -1, Blacks = 1

    void Start() {
        // Initialize valid moves
        switch (piece_name) {
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

    public void movePiece(Square square) {
        if (checkValidMove(square)) {
            cur_square.holdPiece(null);
            square.holdPiece(this);
            cur_square = square;
            if (!started) started = true;
        }

        break_points.Clear();
        transform.position = new Vector3(cur_square.coor.pos.x, transform.position.y, cur_square.coor.pos.z);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void setStartSquare(Square square) {
        cur_square = square;
    }

    public Coordinate getCoordinateMove(Square square) { 
        int coor_x = (square.coor.x - cur_square.coor.x) * team;
        int coor_y = (square.coor.y - cur_square.coor.y) * team;

        return new Coordinate(coor_x, coor_y);
    }

    public void addBreakPoint(Square square) {
        Coordinate coor_move = getCoordinateMove(square);
        for (int j = 0; j < allowed_moves.Count ; j++) {
            if (coor_move.x == allowed_moves[j].x && coor_move.y == allowed_moves[j].y) {
                switch (allowed_moves[j].type) {
                    case MoveType.StartOnly:
                    case MoveType.Move:
                    case MoveType.Eat:
                    case MoveType.EatMove:
                        if (square.holding_piece != null) break_points.Add(coor_move);
                        break;
                }
            }
        }   
    }

    public bool checkValidMove(Square square) {
        Coordinate coor_move = getCoordinateMove(square);

        for (int i = 0; i < allowed_moves.Count ; i++) {
            if (coor_move.x == allowed_moves[i].x && coor_move.y == allowed_moves[i].y) {
                switch (allowed_moves[i].type) {
                    case MoveType.StartOnly:
                        if (!started && checkCanMove(square)) 
                            return true;
                        break;
                    case MoveType.Move:
                        if (checkCanMove(square)) {
                            return true;
                        } 
                        break;
                    case MoveType.Eat:
                        if (checkCanEat(square)) 
                            return true;
                        break;
                    case MoveType.EatMove:
                        if (checkCanEatMove(square)) 
                            return true;
                        break;
                   case MoveType.EatMoveJump:
                        if (checkCanEatMove(square)) 
                            return true;
                        break;
                }
            }
        }
        return false;
    }

    private bool checkCanMove(Square square) {
        Coordinate coor_move = getCoordinateMove(square);

        if (square.holding_piece == null && checkBreakPoint(coor_move)) return true;
        return false;
    }

    private bool checkCanEat(Square square) {
        Coordinate coor_move = getCoordinateMove(square);

        if (square.holding_piece != null && square.holding_piece.team != team && checkBreakPoint(coor_move)) return true;
        return false;
    }

    private bool checkCanEatMove(Square square) {
        if (checkCanEat(square) || checkCanMove(square)) return true; 
        return false;
    }

    private bool checkBreakPoint(Coordinate coor) {
        for (int i = 0; i < break_points.Count; i++) {
            if (break_points[i].x == 0 && coor.x == 0){
                if (break_points[i].y < 0 && (coor.y < break_points[i].y)) {
                    return false;
                }
                else if (break_points[i].y > 0 && (coor.y > break_points[i].y)) {
                    return false;
                }
            }
            else if (break_points[i].y == 0 && coor.y == 0){
                if (break_points[i].x > 0 && (coor.x > break_points[i].x)) {
                    return false;
                }
                else if (break_points[i].x < 0 && (coor.x < break_points[i].x)) {
                    return false;
                }
            }
            else if (break_points[i].y > 0 && (coor.y > break_points[i].y)) {
                if (break_points[i].x > 0 && (coor.x > break_points[i].x)) {
                    return false;
                }
                else if (break_points[i].x < 0 && (coor.x < break_points[i].x)) {
                    return false;
                }
            }
            else if (break_points[i].y < 0 && (coor.y < break_points[i].y)){
                if (break_points[i].x > 0 && (coor.x > break_points[i].x)) {
                    return false;
                }
                else if (break_points[i].x < 0 && (coor.x < break_points[i].x)) {
                    return false;
                }
            }
        }
        return true;
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
                    addAllowedMove(coor_x, coor_y, MoveType.EatMoveJump);
                    addAllowedMove(-coor_x, -coor_y, MoveType.EatMoveJump);
                    addAllowedMove(coor_x, -coor_y, MoveType.EatMoveJump);
                    addAllowedMove(-coor_x, coor_y, MoveType.EatMoveJump);
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