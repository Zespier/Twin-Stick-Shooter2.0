using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayState : AttackBaseState {

    public float maxDistance = 7f;

    public override void OnStateEnter() {
    }

    public override void OnStateExit() {
    }

    /// <summary>
    /// Checks wheteer it should change to hunt state or not
    /// </summary>
    public override void StateLateUpdate() {
        if (Vector3.Distance(transform.position, controller.player.position) > maxDistance) {
            controller.ChangeState(typeof(HuntState));
        }
    }

    /// <summary>
    /// Runs away from the player
    /// </summary>
    public override void StateUpdate() {
        Vector2 direction = (transform.position - controller.player.position).normalized;
        controller.rb.velocity = controller.Speed * direction;
    }

}
