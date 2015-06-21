using System.Collections;
using UnityEngine;
 
class DragAndDrop : MonoBehaviour {
    private bool dragging = false;
    private float distance;
    private Piece this_piece;

    [SerializeField]
    private Board board;

    void Start() {
        this_piece = GetComponent<Piece>();
    }
 
    void OnMouseDown() {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp() {
        Square closest_square = board.getClosestSquare(transform.position);
        this_piece.movePiece(closest_square);
        board.resetHoveredSquares();
        dragging = false;
    }
 
    void Update() {
        if (dragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x - 0.5f, 2.5f, rayPoint.z - 0.7f);

            Square closest_square = board.getClosestSquare(transform.position);
            board.hoverValidSquares(transform.position, GetComponent<Piece>());
            board.hoverClosestSquare(closest_square);
        }
    }
}