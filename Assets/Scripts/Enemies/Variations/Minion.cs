using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Enemy {

    [Header("Minion Attributes")]
    public float speed_Spore_Small = 5f;
    public float distanceToReachPlayer_Boss_Small = 4f;
    public float rotationLerpSpeed_Minion = 0.3f;
    public ParticleSystem explosion;

    private bool _exploded;

    public override float Speed => speed_Spore_Small;
    public override float DistanceToReachPlayer => distanceToReachPlayer_Boss_Small;
    public override float RotationLerpSpeed => rotationLerpSpeed_Minion;


    /// <summary>
    /// Also explodes
    /// </summary>
    protected override void CheckDeath() {
        if (hp < 0) {
            Explode();
        }
        base.CheckDeath();
    }

    /// <summary>
    /// Expllodes
    /// </summary>
    private void Explode() {
        if (_exploded) { return; }
        _exploded = true;

        explosion.Play();
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        enabled = false;


        explosion.transform.SetParent(EnemyContainer.instance.transform);
        Deactivate();
    }

    /// <summary>
    /// Change to shootstate when reaching the player
    /// </summary>
    public override void ReachingPlayer() {
        ChangeState(typeof(ShootState));
    }

    /// <summary>
    /// Change to ShootMovingState when PlayerOutOfReach
    /// </summary>
    public override void PlayerOutOfReach() {
        ChangeState(typeof(ShootMovingState));
    }
}
