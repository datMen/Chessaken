using System.Collections;
using UnityEngine;
 
class DragAndDrop : MonoBehaviour {
    private bool dragging = false;
    private float distance;

    [SerializeField]
    private Board board;

    void Start() {
        board.addPiece(GetComponent<Piece>());
    }
 
    void OnMouseDown() {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp() {
        Coordinate closest_square = board.getClosestSquare(transform.position);
        GetComponent<Piece>().movePiece(closest_square);
        board.resetHoveredSquares();
        dragging = false;
    }
 
    void Update() {
        if (dragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x - 0.5f, 2.5f, rayPoint.z - 0.7f);

            Square closest_square = board.getSquareFromCoordinate(board.getClosestSquare(transform.position));
            board.hoverValidSquares(transform.position, GetComponent<Piece>());
            board.hoverClosestSquare(closest_square);
        }
    }
}