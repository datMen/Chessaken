using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Piece : MonoBehaviour {
    private List<Move> allowed_moves = new List<Move>();
    private MoveType move_type;
    private Piece castling_tower;

    public List<Coordinate> break_points = new List<Coordinate>();
    public bool started;
    public Square cur_square;
    public Board board;

    [SerializeField]
    public string piece_name;

    [SerializeField]
    public int team; // Whites = -1, Blacks = 1

    [SerializeField]
    public List<Piece> castling_towers;

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
                addDiagonalAllowedMoves();
                break;
            case "Queen":
                addLinealAllowedMoves();
                addDiagonalAllowedMoves();
                break;
            case "King":
                addKingAllowedMoves();
                break;
        }
    }

    public void movePiece(Square square) {
        if (checkValidMove(square)) {
            switch (move_type) {
                case MoveType.StartOnly:
                    if (piece_name == "King" && checkCastling(square)) {
                        if (castling_tower.cur_square.coor.x == 0) {
                            castling_tower.castleTower(castling_tower.cur_square.coor.x + 2);
                        }
                        else {
                            castling_tower.castleTower(castling_tower.cur_square.coor.x - 3);
                        }
                    }
                    break;
                case MoveType.Eat:
                case MoveType.EatMove:
                case MoveType.EatMoveJump:
                    eatPiece(square.holding_piece);
                    break;
            }

            cur_square.holdPiece(null);
            square.holdPiece(this);
            cur_square = square;
            if (!started) started = true;
            board.changeTurn();
        }

        break_points.Clear();
        transform.position = new Vector3(cur_square.coor.pos.x, transform.position.y, cur_square.coor.pos.z);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void castleTower(int coor_x) {
        Coordinate castling_coor = new Coordinate(coor_x, cur_square.coor.y);
        Square square = board.getSquareFromCoordinate(castling_coor);

        cur_square.holdPiece(null);
        square.holdPiece(this);
        cur_square = square;
        if (!started) started = true;

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
                        if (square.holding_piece != null) {
                            break_points.Add(coor_move);
                        } 
                        break;
                }
            }
        }   
    }

    public bool checkValidMove(Square square) {
        Coordinate coor_move = getCoordinateMove(square);

        for (int i = 0; i < allowed_moves.Count ; i++) {
            if (coor_move.x == allowed_moves[i].x && coor_move.y == allowed_moves[i].y) {
                move_type = allowed_moves[i].type;
                switch (move_type) {
                    case MoveType.StartOnly:
                        if (!started && checkCanMove(square) && checkCastling(square)) 
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
                    case MoveType.EatMoveJump:
                        if (checkCanEatMove(square)) {
                            if (piece_name == "King")
                                return true;
                            else
                                return true;
                        }
                        break;
                }
            }
        }
        return false;
    }

    public bool checkValidCheckKingMove(Square square) {
        Piece old_holding_piece = square.holding_piece;
        Square old_square = cur_square;
        
        cur_square.holdPiece(null);
        cur_square = square;
        square.holdPiece(this);

        if (!board.isCheckKing(team) || (square == board.checking_pieces[team].cur_square)) {
            square.holdPiece(old_holding_piece);
            cur_square = old_square;
            cur_square.holdPiece(this);
            return true;
        }

        cur_square = old_square;
        cur_square.holdPiece(this);
        square.holdPiece(old_holding_piece);
        return false;
    }

    public void eatMe() {
        board.destroyPiece(this);
        Destroy(this.gameObject);
    }

    private void eatPiece(Piece piece) {
        if (piece != null && piece.team != team) piece.eatMe();
    }

    private bool checkCanMove(Square square) {
        Coordinate coor_move = getCoordinateMove(square);

        if (square.holding_piece == null && checkBreakPoint(coor_move) && checkValidCheckKingMove(square)) return true;
        return false;
    }

    private bool checkCanEat(Square square) {
        Coordinate coor_move = getCoordinateMove(square);

        if (square.holding_piece != null && square.holding_piece.team != team && checkBreakPoint(coor_move) && checkValidCheckKingMove(square)) return true;
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

    private bool checkCastling(Square square) {
        if (piece_name == "King") {
            float closest_castling = Vector3.Distance(square.coor.pos, castling_towers[0].transform.position);
            castling_tower = castling_towers[0];

            for (int i = 0; i < castling_towers.Count; i++) {
                if (Vector3.Distance(square.coor.pos, castling_towers[i].transform.position) <= closest_castling) {
                    castling_tower = castling_towers[i];
                }
            }
            bool can_castle = board.checkCastlingSquares(cur_square, castling_tower.cur_square, team);

            return (!castling_tower.started && can_castle) ? true : false;
        }  
        else {
            return true;
        }
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
        addLinealAllowedMoves();
    }

    private void addLinealAllowedMoves() {
        for (int coor_x = 1; coor_x < 8; coor_x++) {
            addAllowedMove(coor_x, 0, MoveType.EatMove);
            addAllowedMove(0, coor_x, MoveType.EatMove);
            addAllowedMove(-coor_x, 0, MoveType.EatMove);
            addAllowedMove(0, -coor_x, MoveType.EatMove);
        }
    }

    private void addDiagonalAllowedMoves() {
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

    private void addKingAllowedMoves() {
        // Castling moves
        addAllowedMove(-2, 0, MoveType.StartOnly);
        addAllowedMove(2, 0, MoveType.StartOnly);

        // Normal moves
        addAllowedMove(0, 1, MoveType.EatMove);
        addAllowedMove(1, 1, MoveType.EatMove);
        addAllowedMove(1, 0, MoveType.EatMove);
        addAllowedMove(1, -1, MoveType.EatMove);
        addAllowedMove(0, -1, MoveType.EatMove);
        addAllowedMove(-1, -1, MoveType.EatMove);
        addAllowedMove(-1, 0, MoveType.EatMove);
        addAllowedMove(-1, 1, MoveType.EatMove);
    }
}