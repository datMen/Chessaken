using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {
    private Material start_mat;
    private Material cur_mat;

    public Coordinate coor;
    public Piece holding_piece = null;

    [SerializeField]
    public Board board;

    void Start() {
        start_mat = renderer.material;
    }

    public void holdPiece(Piece piece) {
        holding_piece = piece;
    }

    public void setMaterial(Material mat) {
        renderer.material = mat;
    }

    public void hoverSquare(Material mat) {
        cur_mat = renderer.material;
        renderer.material = mat;
    }

    public void unHoverSquare() {
        renderer.material = cur_mat;
    }

    public void resetMaterial() {
        cur_mat = start_mat;
        renderer.material = start_mat;
    }
}
