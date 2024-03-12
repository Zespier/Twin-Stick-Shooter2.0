using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardState : AttackBaseState {

    public float guardTime = -1f;

    private float _guardTimer;

    public override void OnStateEnter() {
        _guardTimer = 0;
    }

    public override void OnStateExit() {
    }

    public override void StateLateUpdate() {
        if (guardTime == -1) { return; }

        _guardTimer += Time.deltaTime;
        if (_guardTimer >= guardTime) {
            controller.FinishedGuarding();
        }
    }

    public override void StateUpdate() {
    }
}
