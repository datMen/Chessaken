using UnityEngine;

/*
==============================
[Square] - Script placed on every square in the board.
==============================
*/
public class Square : MonoBehaviour {
    private Material start_mat; // Default material
    private Material cur_mat; // Current material

    public Coordinate coor; // Square position in the board
    public Piece holding_piece = null; // Current piece in this square

    [SerializeField]
    public Board board;

    void Start() {
        start_mat = renderer.material;
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
        cur_mat = renderer.material;
        renderer.material = mat;
    }

    public void unHoverSquare() {
        renderer.material = cur_mat;
    }

    // Reset material to default
    public void resetMaterial() {
        cur_mat = start_mat;
        renderer.material = start_mat;
    }
}
