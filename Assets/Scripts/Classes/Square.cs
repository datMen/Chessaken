using UnityEngine;

/*
==============================
[Square] - Script placed on every square in the board.
==============================
*/
public class Square : MonoBehaviour {
    private Material cur_mat; // Current material

    public Coordinate coor; // Square position in the board
    public Piece holding_piece = null; // Current piece in this square
    public Material start_mat; // Default material

    [SerializeField]
    public int team;

    [SerializeField]
    public Board board;

    void Start() {
        start_mat = GetComponent<Renderer>().material;
    }

    public void holdPiece(Piece piece) {
        holding_piece = piece;
    }

    /*
    ---------------
    Materials related functions
    ---------------
    */ 
    public void hoverSquare(Material mat) {
        cur_mat = GetComponent<Renderer>().material;
        GetComponent<Renderer>().material = mat;
    }

    public void unHoverSquare() {
        GetComponent<Renderer>().material = cur_mat;
    }

    // Reset material to default
    public void resetMaterial() {
        cur_mat = start_mat;
        GetComponent<Renderer>().material = start_mat;
    }
}
