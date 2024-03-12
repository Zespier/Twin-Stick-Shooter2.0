using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DashState : AttackBaseState {

    public float dashDistance = 20f;

    private float _distanceTraveled = 0;
    private Vector2 _dashDirection;

    public override void OnStateEnter() {
        _distanceTraveled = 0;
        _dashDirection = -(controller as Wolf).movementDirectionHelper.up;
    }

    public override void OnStateExit() {
    }

    public override void StateLateUpdate() {
        if (_distanceTraveled > dashDistance) {
            controller.ChangeState(typeof(HuntState));
        }
    }

    public override void StateUpdate() {
        controller.rb.velocity = controller.Speed * _dashDirection;
        _distanceTraveled += (Time.deltaTime * controller.Speed * _dashDirection).magnitude;
    }

}
