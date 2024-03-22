using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy {

    [Header("Wolf Attributes")]
    public float wolfSpeed = 20f;

    public override float Speed => wolfSpeed;

    public ParticleSystem explosion;

    private bool _exploded;

    /// <summary>
    /// Also explodes
    /// </summary>
    protected override void CheckDeath() {
        if (Hp < 0) {
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

}
