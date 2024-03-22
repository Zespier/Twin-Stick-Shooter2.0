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
        Explode();
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

        StartCoroutine(C_WaitSecondsToDestroy(explosion.main.startLifetime.constant + 0.1f));
    }

    /// <summary>
    /// Waits bofre destroygin this enemy
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    private IEnumerator C_WaitSecondsToDestroy(float seconds) {
        yield return new WaitForSeconds(seconds);
        Deactivate();
    }
}
