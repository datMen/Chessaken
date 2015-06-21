using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {
    private Material start_mat;

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

    public void resetMaterial() {
        renderer.material = start_mat;
    }
}
