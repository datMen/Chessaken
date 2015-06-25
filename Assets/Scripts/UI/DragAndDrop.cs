using UnityEngine;

/*
==============================
[DragAndDrop] - Script placed on every piece in the board.
==============================
*/
class DragAndDrop : MonoBehaviour {
    private bool dragging = false;
    private float distance;
    private Piece this_piece;

    [SerializeField]
    private Board board;

    void Start() {
        this_piece = GetComponent<Piece>(); // Get piece's component
    }

    void Update() {
        if (dragging) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);

            // Update piece's dragging position, we try to place it as close as we can to the mouse
            transform.position = new Vector3(rayPoint.x - 0.5f, 2.7f, rayPoint.z - 0.5f);
            transform.rotation = new Quaternion(0, 0, 0, 0);

            // Hover the square this piece could go id we drop it
            Square closest_square = board.getClosestSquare(transform.position);
            board.hoverClosestSquare(closest_square);
        }
    }

    void OnMouseDown() {
        // If it's my turn
        if (board.cur_turn == this_piece.team) {
            // Set distance between the mouse & this piece
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            board.hoverValidSquares(this_piece);
            dragging = true; // Start dragging
        }
    }
 
    void OnMouseUp() {
        if (dragging) {
            // Get closest square & try to move the piece to it
            Square closest_square = board.getClosestSquare(transform.position);
            this_piece.movePiece(closest_square);
            
            board.resetHoveredSquares();
            dragging = false; // Stop dragging
        }
    }
}