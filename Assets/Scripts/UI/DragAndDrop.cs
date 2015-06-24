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
        if (board.cur_turn == this_piece.team && !board.isCheckKing(this_piece.team)) {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            board.hoverValidSquares(GetComponent<Piece>());
            dragging = true;
        }
        else if (board.isCheckKing(this_piece.team)) {
            // if (this_piece.piece_name == "King" || board.canInterceptCheckKing(this_piece)) {
                distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                board.hoverValidSquares(GetComponent<Piece>());
                dragging = true;
            // }
        }
    }
 
    void OnMouseUp() {
        if (dragging) {
            Square closest_square = board.getClosestSquare(transform.position);
            this_piece.movePiece(closest_square);
            board.resetHoveredSquares();
            dragging = false;
        }
    }
 
    void Update() {
        if (dragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = new Vector3(rayPoint.x - 0.5f, 2.7f, rayPoint.z - 0.5f);
            transform.rotation = new Quaternion(0, 0, 0, 0);

            Square closest_square = board.getClosestSquare(transform.position);
            board.hoverClosestSquare(closest_square);
        }
    }
}