using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour {
    public Coordinate coor;
    public Vector3 position;

    [SerializeField]
    public Board board;

    void Start() {
        position = transform.position;
        coor = board.getSquareCoordinate(transform.position);
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "Piece") {
            // Debug.Log("x: " + coor.x + " y: " + coor.y + " pos: " + coor.pos);
            col.GetComponent<Piece>().setStartCoor(coor);
        }
    }
}
