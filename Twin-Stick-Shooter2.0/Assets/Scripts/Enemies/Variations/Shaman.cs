using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaman : Enemy {

    [Header("Shaman Attributes")]
    public float speed_Shaman = 5f;
    public ParticleSystem explosion;
    public BoxCollider2D boxCollider;

    private bool _exploded;

    public override float Speed => speed_Shaman;

    public override void ReachingPlayer() {
        Explode();
    }

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

    private IEnumerator C_WaitSecondsToDestroy(float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

}
