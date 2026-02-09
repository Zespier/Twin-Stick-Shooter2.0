using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Spore_Small : Enemy {

    [Header("Spore_Small Attributes")]
    public float speed_Spore_Small = 15f;
    public float distanceToReachPlayer_Boss_Small = 0.1f;
    public ParticleSystem explosion;
    public BoxCollider boxCollider;

    private bool _exploded;

    public override float Speed => speed_Spore_Small;
    public override float DistanceToReachPlayer => distanceToReachPlayer_Boss_Small;

    /// <summary>
    /// Explodes when reaching the player
    /// </summary>
    public override void ReachingPlayer() {
        if (_exploded) { return; }
        PlayerController.instance.RemoveHealth(500);

        hp -= 500;
        EnemySpawner.instance.EnemyTookDamage(AssetReference, (hp / stats.HP) * 100f);
        CheckDeath();
    }

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
        boxCollider.enabled = false;
        enabled = false;

        PlayerController.instance.TakeDamage(transform.position, 500, true, DamageType.PlayerDamaged);


        explosion.transform.SetParent(EnemyContainer.instance.transform);
        Deactivate();
    }
}
