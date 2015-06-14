using UnityEngine;
using System.Collections;
using System.Timers;

public class StandStill : PieceState {

    public StandStill(PieceContext cntxt) {
        this.cntxt = cntxt;
    }

    public override void onEnter() { }

    public override void onUpdate() { }

    public override void onLeave() { }

    public override void startStandStill() {}

    public override void startMoving() { }
}