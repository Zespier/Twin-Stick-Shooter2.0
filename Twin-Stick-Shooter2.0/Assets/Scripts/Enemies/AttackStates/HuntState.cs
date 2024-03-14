using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntState : AttackBaseState {

    public float selfieStickSize = 6f;
    public float huntTime = 2f;

    private float _huntTimer;

    public override void OnStateEnter() {
        _huntTimer = 0;
    }

    public override void OnStateExit() {
    }

    public override void StateLateUpdate() {
        _huntTimer += Time.deltaTime;
        if (_huntTimer >= huntTime) {
            controller.ChangeState(typeof(DashState));
        }
    }

    public override void StateUpdate() {
        Vector3 newForward = controller.player.position - transform.position;
        newForward.y = 0;
        (controller as Wolf).movementDirectionHelper.forward = newForward;

        Vector3 direction = (controller as Wolf).movementDirectionHelper.right;
        controller.rb.velocity = controller.Speed * direction;

        Vector3 clampedPosition = controller.player.position;
        clampedPosition -= (controller as Wolf).movementDirectionHelper.forward * selfieStickSize;
        clampedPosition.y = 0;

        transform.position = Vector3.Lerp(transform.position, clampedPosition, Time.deltaTime / 0.27f);

    }
}
