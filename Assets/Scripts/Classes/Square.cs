using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {
    public Coordinate coor;
    public Vector3 position;
    private Material start_mat;

    [SerializeField]
    public Board board;

    void Start() {
        coor = board.getSquareCoordinate(transform.position);
        position = transform.position;
        start_mat = renderer.material;
    }

    public void setMaterial(Material mat) {
        renderer.material = mat;
    }

    public void resetMaterial() {
        renderer.material = start_mat;
    }
}
