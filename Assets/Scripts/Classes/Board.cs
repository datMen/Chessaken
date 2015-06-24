using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour {
    private List<Square> hovered_squares = new List<Square>();
    private Square closest_square;

    public int cur_turn = -1; // -1 = whites; 1 = blacks
    public Dictionary<int, Piece> checking_pieces = new Dictionary<int, Piece>();

    [SerializeField]
    MainCamera main_camera;

    [SerializeField]
    Material square_hover_mat;

    [SerializeField]
    Material square_closest_mat;

    [SerializeField]
    GameObject win_msg;

    [SerializeField]
    TextMesh win_txt;

    [SerializeField]
    List<Square> squares = new List<Square>();

    [SerializeField]
    List<Piece> pieces = new List<Piece>();

    void Start() {
        addSquareCoordinates();
        setStartPiecesCoor();
    }

    public Coordinate getSquareCoordinate(Vector3 pos) {
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].transform.position == pos) {
                return squares[i].coor;
            }
        }
        return new Coordinate(0, 0);
    }

    public Square getClosestSquare(Vector3 pos) {
        Square square = squares[0];
        float closest = Vector3.Distance(pos, squares[0].coor.pos);

        for (int i = 0; i < squares.Count ; i++) {
            float distance = Vector3.Distance(pos, squares[i].coor.pos);

            if (distance < closest) {
                square = squares[i];
                closest = distance;
            }
        }
        return square;
    }

    public void hoverClosestSquare(Square square) {
        if (closest_square) closest_square.unHoverSquare();
        square.hoverSquare(square_closest_mat);
        closest_square = square;
    }

    public void hoverValidSquares(Piece piece) {
        addPieceBreakPoints(piece);
        for (int i = 0; i < squares.Count ; i++) {
            if (piece.checkValidMove(squares[i])) {
                squares[i].hoverSquare(square_hover_mat);
                hovered_squares.Add(squares[i]);
            }
        }
    }

    public void addPieceBreakPoints(Piece piece) {
        piece.break_points.Clear();
        for (int i = 0; i < squares.Count ; i++) {
            piece.addBreakPoint(squares[i]);
        }
    }

    public void resetHoveredSquares() {
        for (int i = 0; i < hovered_squares.Count ; i++) {
            hovered_squares[i].resetMaterial();
        }
        hovered_squares.Clear();
        closest_square.resetMaterial();
        closest_square = null;
    }

    public Square getSquareFromCoordinate(Coordinate coor) {
        Square square = squares[0];
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].coor.x == coor.x && squares[i].coor.y == coor.y) {
                return squares[i];
            }
        }
        return square;
    }

    public void changeTurn() {
        cur_turn = (cur_turn == -1) ? 1 : -1;
        if (isCheckMate(cur_turn)) {
            doCheckMate(cur_turn);
        }
        else {
            main_camera.changeTeam(cur_turn);
        }
    }

    public bool isCheckKing(int team) {
        Piece king = getKingPiece(team);

        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team != king.team) {
                addPieceBreakPoints(pieces[i]);
                if (pieces[i].checkValidMove(king.cur_square)) {
                    checking_pieces[team] = pieces[i];
                    return true;
                } 
            }
        }
        return false;
    }

    public bool isCheckMate(int team) {
        if (isCheckKing(team)) {
            Piece king = getKingPiece(team);
            int valid_moves = 0;

            addPieceBreakPoints(king);
            for (int i = 0; i < squares.Count ; i++) {
                if (king.checkValidMove(squares[i])) {
                    valid_moves++;
                }
            }

            if (valid_moves == 0) {
                return true;
            }
        }
        return false;
    }

    public void doCheckMate(int loser) {
        string winner = (loser == 1) ? "White" : "Black";

        win_txt.text = winner + win_txt.text;
        int txt_rotation = (cur_turn == -1) ? 0 : 180;


        win_msg.transform.rotation = Quaternion.Euler(0, txt_rotation, 0);
        win_msg.GetComponent<Rigidbody>().useGravity = true;
    }

    public Piece getKingPiece(int team) {
        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team == team && pieces[i].piece_name == "King") {
                return pieces[i];
            }
        }
        return pieces[0];
    }

    public bool checkCastlingSquares(Square square1, Square square2, int castling_team) {
        List<Square> castling_squares = new List<Square>();

        if (square1.coor.x < square2.coor.x) {
            for (int i = square1.coor.x; i < square2.coor.x; i++) {
                Coordinate coor = new Coordinate(i, square1.coor.y);
                castling_squares.Add(getSquareFromCoordinate(coor));
            }
        }
        else {
            for (int i = square1.coor.x; i > square2.coor.x; i--) {
                Coordinate coor = new Coordinate(i, square1.coor.y);
                castling_squares.Add(getSquareFromCoordinate(coor));
            }
        }
        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team != castling_team) {
                addPieceBreakPoints(pieces[i]);
                for (int j = 0; j < castling_squares.Count; j++) {
                    if (pieces[i].checkValidMove(castling_squares[j])) return false;
                }
            }
        }
        
        return true;
    }

    public bool canInterceptCheckKing(Piece piece) {
        addPieceBreakPoints(piece);
        for (int i = 0; i < squares.Count ; i++) {
            if (piece.checkValidMove(squares[i])) {
                Piece old_holding_piece = squares[i].holding_piece;
                Square old_square = piece.cur_square;
                
                piece.cur_square.holdPiece(null);
                piece.cur_square = squares[i];
                squares[i].holdPiece(piece);

                if (!isCheckKing(piece.team) || (squares[i] == checking_pieces[piece.team].cur_square)) {
                    squares[i].holdPiece(old_holding_piece);
                    piece.cur_square = old_square;
                    piece.cur_square.holdPiece(piece);
                    return true;
                }

                piece.cur_square = old_square;
                piece.cur_square.holdPiece(piece);
                squares[i].holdPiece(old_holding_piece);
                return false;
            }
        }
        return false;
    }

    public void destroyPiece(Piece piece) {
        pieces.Remove(piece);
    }

    private void addSquareCoordinates() {
        int coor_x = 0;
        int coor_y = 0;
        for (int i = 0; i < squares.Count ; i++) {
            squares[i].coor = new Coordinate(coor_x, coor_y);
            squares[i].coor.pos = new Vector3(squares[i].transform.position.x - 0.5f, squares[i].transform.position.y, squares[i].transform.position.z - 0.5f);

            if (coor_y > 0 && coor_y % 7 == 0) {
                coor_x++;
                coor_y = 0;
            }
            else {
                coor_y++;
            }
        }
    }

    private void setStartPiecesCoor() {
        for (int i = 0; i < pieces.Count ; i++) {
            Square closest_square = getClosestSquare(pieces[i].transform.position);
            closest_square.holdPiece(pieces[i]);
            pieces[i].setStartSquare(closest_square);
            pieces[i].board = this;
        }
    }
}
