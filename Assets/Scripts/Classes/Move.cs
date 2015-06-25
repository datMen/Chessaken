
/*
==============================
[Move] - Piece move class
==============================
*/
public class Move {
    public int x;
    public int y;
    public MoveType type;

    public Move(int x, int y, MoveType type) {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}