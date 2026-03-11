using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayState : AttackBaseState {

    public float maxDistance = 7f;

    public override void OnStateEnter() {
    }

    public override void OnStateExit() {
    }

    public override void StateLateUpdate() {
        if (Vector3.Distance(transform.position, Ship.instanceOfClient.transform.position) > maxDistance) {
            controller.ChangeState(typeof(HuntState));
        }
    }

    public override void StateUpdate() {
        Vector2 direction = (transform.position - Ship.instanceOfClient.transform.position).normalized;
        transform.position += Time.deltaTime * controller.Speed * new Vector3(direction.x, 0, direction.y);
    }
}
