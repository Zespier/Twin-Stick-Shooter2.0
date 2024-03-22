using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class DashState : AttackBaseState {

    public float dashDistance = 20f;

    private float _distanceTraveled = 0;
    private Vector3 _dashDirection;

    public override void OnStateEnter() {
        _distanceTraveled = 0;
        _dashDirection = controller .body.forward;
        controller.rb.velocity = controller.Speed * 1.5f * _dashDirection;
    }

    public override void OnStateExit() {
    }

    /// <summary>
    /// Checks if the dash should end
    /// </summary>
    public override void StateLateUpdate() {
        if (_distanceTraveled > dashDistance) {
            controller.ChangeState(typeof(HuntState));
        }
    }

    /// <summary>
    /// Calculates the distance traveled
    /// </summary>
    public override void StateUpdate() {
        _distanceTraveled += (Time.deltaTime * controller.Speed * _dashDirection).magnitude;
    }

}
