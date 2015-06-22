using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour {
    private List<Square> hovered_squares = new List<Square>();
    private Square closest_square;

    public int cur_turn = -1; // -1 = whites; 1 = blacks

    [SerializeField]
    MainCamera main_camera;

    [SerializeField]
    Material square_hover_mat;

    [SerializeField]
    Material square_closest_mat;

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
        if (closest_square) closest_square.resetMaterial();
        square.setMaterial(square_closest_mat);
        closest_square = square;
    }

    public void hoverValidSquares(Vector3 pos, Piece piece) {
        addPieceBreakPoints(piece);
        for (int i = 0; i < squares.Count ; i++) {
            if (piece.checkValidMove(squares[i])) {
                squares[i].setMaterial(square_hover_mat);
                hovered_squares.Add(squares[i]);
            }
        }
    }

    public void addPieceBreakPoints(Piece piece) {
        for (int i = 0; i < squares.Count ; i++) {
            piece.addBreakPoint(squares[i]);
        }
    }

    public void resetHoveredSquares() {
        for (int i = 0; i < hovered_squares.Count ; i++) {
            hovered_squares[i].resetMaterial();
        }
        closest_square.resetMaterial();
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
        main_camera.changeTeam(cur_turn);
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
            closest_square.holding_piece = pieces[i];
            pieces[i].setStartSquare(closest_square);
        }
    }
}
