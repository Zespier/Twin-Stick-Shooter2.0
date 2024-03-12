using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntState : AttackBaseState {

    public float minDistance = 5f;
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
        if (/*Vector3.Distance(transform.position, controller.player.position) < minDistance ||*/ _huntTimer >= huntTime) {
            controller.ChangeState(typeof(DashState));
        }
    }

    public override void StateUpdate() {

        (controller as Wolf).movementDirectionHelper.up = transform.position - controller.player.position;
        Vector2 direction = (controller as Wolf).movementDirectionHelper.right;
        controller.rb.velocity = controller.Speed * direction;

        Vector2 clampedPosition = controller.player.position;
        clampedPosition += (Vector2)(controller as Wolf).movementDirectionHelper.up * selfieStickSize;

        transform.position = Vector3.Lerp(transform.position, clampedPosition, Time.deltaTime / 0.27f);

        //controller.rb.velocity = Vector3.zero;
        //Vector2 targetPosition = (Vector2)controller.player.position + _selfieStick;
        //Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
    }
}
