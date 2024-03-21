using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Turret : Enemy {

    public float turretSpeed = 0f;
    public Transform Body;
    public float guard_RotationSpeed = 50f;
    public float shoot_RotationSpeed = 150f;

    public override float Speed => turretSpeed;

    public float RotationSpeed { get; set; }

    protected override void Update() {
        base.Update();
    }

    #region Behaviour with States

    public override void FinishedShooting() {
        RotationSpeed = guard_RotationSpeed;
        ChangeState(typeof(GuardState));
    }
    public override void FinishedGuarding() {
        RotationSpeed = shoot_RotationSpeed;
        ChangeState(typeof(ShootState));
    }

    #endregion

    protected override void RotateBody() {
        float rotationValue = Vector3.SignedAngle(Body.up, player.position - Body.transform.position, Vector3.forward) > 0 ? RotationSpeed : -RotationSpeed;
        Body.up = Quaternion.AngleAxis(rotationValue * Time.deltaTime, Vector3.forward) * Body.up;
    }

}
