using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieState : AttackBaseState {

    public override void OnStateEnter() {
    }

    public override void OnStateExit() {
    }

    /// <summary>
    /// Checks if it reached the player
    /// </summary>
    public override void StateLateUpdate() {
        if (Vector3.Distance(controller.player.position, transform.position) < controller.DistanceToReachPlayer) {
            controller.ReachingPlayer();
        }
    }

    /// <summary>
    /// moves towards the player
    /// </summary>
    public override void StateUpdate() {
        controller.rb.velocity = (controller.player.position - transform.position).normalized * controller.Speed;
    }
}
