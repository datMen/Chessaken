using System.Collections;
using UnityEngine;
 
class DragAndDrop : MonoBehaviour {
    private bool dragging = false;
    private float distance;

    [SerializeField]
    private Board board;
 
    void OnMouseDown() {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp() {
        Coordinate closest_square = board.getClosestSquare(transform.position);
        GetComponent<Piece>().movePiece(closest_square);
        dragging = false;
    }
 
    void Update() {
        if (dragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x - 0.5f, 2.5f, rayPoint.z - 0.7f);

            // Coordinate closest_square = board.getClosestSquare(transform.position);
            // board.hoverSquare(closest_square);
        }
    }
}