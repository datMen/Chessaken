using UnityEngine;
using System.Collections;

public abstract class PieceState {
    protected PieceContext cntxt;

    public abstract void onEnter();
    public abstract void onUpdate();
    public abstract void onLeave();

    public abstract void startStandStill();
    public abstract void startMoving();
}