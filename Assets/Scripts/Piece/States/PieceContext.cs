using UnityEngine;

public interface PieceContext {
    void updateState(PieceStateId state);
    PieceStateId getCurStateId();

    void startStandStill();
    void startMoving();
}
