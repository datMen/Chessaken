using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
==============================
[Board] - Main script, controls the game
==============================
*/
public class Board : MonoBehaviour {
    private List<Square> hovered_squares = new List<Square>(); // List squares to hover
    private Square closest_square; // Current closest square when dragging a piece
    private int cur_theme = 0;

    public int cur_turn = -1; // -1 = whites; 1 = blacks
    public Dictionary<int, Piece> checking_pieces = new Dictionary<int, Piece>(); // Which piece is checking the king (key = team)
    
    // UI variables
    public bool use_hover; // Hover valid moves & closest square
    public bool rotate_camera; // Enable/disable camera rotation

    [SerializeField]
    MainCamera main_camera;

    [SerializeField]
    Material square_hover_mat; // Piece's valid squares material

    [SerializeField]
    Material square_closest_mat; // Piece's closest square material

    [SerializeField]
    GameObject win_msg;

    [SerializeField]
    TextMesh win_txt;

    [SerializeField]
    List<Theme> themes = new List<Theme>();

    [SerializeField]
    List<Renderer> board_sides = new List<Renderer>();

    [SerializeField]
    List<Renderer> board_corners = new List<Renderer>();

    [SerializeField]
    List<Square> squares = new List<Square>(); // List of all game squares (64) - ordered

    [SerializeField]
    List<Piece> pieces = new List<Piece>(); // List of all pieces in the game (32)

    void Start() {
        setBoardTheme();
        addSquareCoordinates(); // Add "local" coordinates to all squares
        setStartPiecesCoor(); // Update all piece's coordinate
    }

    /*
    ---------------
    Squares related functions
    ---------------
    */ 
    // Returns closest square to the given position
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

    // Returns the square that is at the given coordinate (local position in the board)
    public Square getSquareFromCoordinate(Coordinate coor) {
        Square square = squares[0];
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].coor.x == coor.x && squares[i].coor.y == coor.y) {
                return squares[i];
            }
        }
        return square;
    }

    // Hover piece's closest square
    public void hoverClosestSquare(Square square) {
        if (closest_square) closest_square.unHoverSquare();
        square.hoverSquare(themes[cur_theme].square_closest);
        closest_square = square;
    }

    // Hover all the piece's allowed moves squares
    public void hoverValidSquares(Piece piece) {
        addPieceBreakPoints(piece);
        for (int i = 0; i < squares.Count ; i++) {
            if (piece.checkValidMove(squares[i])) {
                squares[i].hoverSquare(themes[cur_theme].square_hover);
                hovered_squares.Add(squares[i]);
            }
        }
    }

    // Once the piece is dropped, reset all squares materials to the default
    public void resetHoveredSquares() {
        for (int i = 0; i < hovered_squares.Count ; i++) {
            hovered_squares[i].resetMaterial();
        }
        hovered_squares.Clear();
        closest_square.resetMaterial();
        closest_square = null;
    }

    // If the king is trying to castle with a tower, we'll check if an enemy piece is targeting any square
    // between the king and the castling tower
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

    // Set start square's local coordinates & its current position
    private void addSquareCoordinates() {
        int coor_x = 0;
        int coor_y = 0;
        for (int i = 0; i < squares.Count ; i++) {
            squares[i].coor = new Coordinate(coor_x, coor_y);
            squares[i].coor.pos = new Vector3(squares[i].transform.position.x - 0.5f, squares[i].transform.position.y, squares[i].transform.position.z - 0.5f);
            if (squares[i].team == -1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_white;
            else if (squares[i].team == 1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_black;
            squares[i].start_mat = squares[i].GetComponent<Renderer>().material;

            if (coor_y > 0 && coor_y % 7 == 0) {
                coor_x++;
                coor_y = 0;
            }
            else {
                coor_y++;
            }
        }
    }

    /*
    ---------------
    Pieces related functions
    ---------------
    */ 
    // Add pieces squares that are breaking the given piece's allowed positions
    public void addPieceBreakPoints(Piece piece) {
        piece.break_points.Clear();
        for (int i = 0; i < squares.Count ; i++) {
            piece.addBreakPoint(squares[i]);
        }
    }

    // Check if the king's given team is in check
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

    // Check if the given team lost
    public bool isCheckMate(int team) {
        if (isCheckKing(team)) {
            int valid_moves = 0;

            for (int i = 0; i < squares.Count ; i++) {
                for (int j = 0; j < pieces.Count; j++) {
                    if (pieces[j].team == team) {
                        if (pieces[j].checkValidMove(squares[i])) {
                            valid_moves++;
                        }
                    }
                }
            }

            if (valid_moves == 0) {
                return true;
            }
        }
        return false;
    }

    // Get king's given team
    public Piece getKingPiece(int team) {
        for (int i = 0; i < pieces.Count; i++) {
            if (pieces[i].team == team && pieces[i].piece_name == "King") {
                return pieces[i];
            }
        }
        return pieces[0];
    }

    // Remove the given piece from the pieces list
    public void destroyPiece(Piece piece) {
        pieces.Remove(piece);
    }

    // Update each piece's coordinates getting the closest square
    private void setStartPiecesCoor() {
        for (int i = 0; i < pieces.Count ; i++) {
            Square closest_square = getClosestSquare(pieces[i].transform.position);
            closest_square.holdPiece(pieces[i]);
            pieces[i].setStartSquare(closest_square);
            pieces[i].board = this;
            if (pieces[i].team == -1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_white);
            else if (pieces[i].team == 1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_black);
        }
    }

    private void setPieceTheme(Transform piece_tr, Material mat) {
        for (int i = 0; i < piece_tr.childCount; ++i) {
            Transform child = piece_tr.GetChild(i);
            try {
                child.GetComponent<Renderer>().material = mat;
            }
            catch (Exception e) {
                for (int j = 0; j < child.childCount; ++j) {
                    Transform child2 = child.GetChild(j);
                    child2.GetComponent<Renderer>().material = mat;
                }
            }
        }
    }

    /*
    ---------------
    Game related functions
    ---------------
    */ 
    // Change current turn, we check if a team lost before rotating the camera
    public void changeTurn() {
        cur_turn = (cur_turn == -1) ? 1 : -1;
        if (isCheckMate(cur_turn)) {
            doCheckMate(cur_turn);
        }
        else if(rotate_camera) {
            main_camera.changeTeam(cur_turn);
        }
    }

    // Show check mate message
    public void doCheckMate(int loser) {
        string winner = (loser == 1) ? "White" : "Black";

        win_txt.text = winner + win_txt.text;
        int txt_rotation = (cur_turn == -1) ? 0 : 180;

        win_msg.transform.rotation = Quaternion.Euler(0, txt_rotation, 0);
        win_msg.GetComponent<Rigidbody>().useGravity = true;
    }

    /*
    ---------------
    User Interface related functions
    ---------------
    */ 
    public void useHover(bool use) {
        use_hover = use;
    }

    public void rotateCamera(bool rotate) {
        rotate_camera = rotate;
    }

    public void setBoardTheme() {
        for (int i = 0; i < board_sides.Count ; i++) {
            board_sides[i].material = themes[cur_theme].board_side;
            board_corners[i].material = themes[cur_theme].board_corner;
        }
    }

    public void updateGameTheme(int theme) {
        cur_theme = theme;
        setBoardTheme();
        for (int i = 0; i < pieces.Count ; i++) {
            if (pieces[i].team == -1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_white);
            else if (pieces[i].team == 1) setPieceTheme(pieces[i].transform, themes[cur_theme].piece_black);
        }
        for (int i = 0; i < squares.Count ; i++) {
            if (squares[i].team == -1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_white;
            else if (squares[i].team == 1) squares[i].GetComponent<Renderer>().material = themes[cur_theme].square_black;
            squares[i].start_mat = squares[i].GetComponent<Renderer>().material;
        }
    }
}
