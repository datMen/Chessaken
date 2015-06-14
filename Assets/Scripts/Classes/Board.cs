using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Board : MonoBehaviour {
    private bool started;

    [SerializeField]
    List<Square> squares = new List<Square>();

    [SerializeField]
    Material square_hover_mat;

    void Start() {
        // addSquareCoordinates();
    }
    void Update() {
        if (!started) {
            int i;
            for (i = 0; i < squares.Count ; i++) {
                if (squares[i].position == new Vector3(0,0,0)) {
                    break;
                }
            }
            if (i == squares.Count) {
                addSquareCoordinates();
                started = true;
            }
        }
    }

    public Coordinate getSquareCoordinate(Vector3 pos) {
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].position == pos) {
                return squares[i].coor;
            }
        }
        return new Coordinate(0, 0);
    }

    public Coordinate getClosestSquare(Vector3 pos) {
        Coordinate coor = squares[0].coor;
        float closest = Vector3.Distance(pos, squares[0].coor.pos);

        for (int i = 0; i < squares.Count ; i++) {
            float distance = Vector3.Distance(pos, squares[i].coor.pos);
            if (distance < closest) {
                coor = squares[i].coor;
                closest = distance;
            }
        }
        return coor;
    }

    public void hoverSquare(Coordinate coor) {
        // GameObject square = getSquareFromCoordinate(coor);

        // square.renderer.material = square_hover_mat;
    }

    private Square getSquareFromCoordinate(Coordinate coor) {
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].coor.x == coor.x && squares[i].coor.y == coor.y) {
                return squares[i];
            }
        }
        return new Square();
    }

    private void addSquareCoordinates() {
        int coor_x = 0;
        int coor_y = 0;
        for (int i = 0; i < squares.Count ; i++) {
            squares[i].coor.x = coor_x;
            squares[i].coor.y = coor_y;
            squares[i].coor.pos = new Vector3(squares[i].position.x - 0.5f, squares[i].position.y, squares[i].position.z - 0.5f);

            if (coor_y > 0 && coor_y % 7 == 0) {
                coor_x++;
                coor_y = 0;
            }
            else {
                coor_y++;
            }
        }
    }
}
