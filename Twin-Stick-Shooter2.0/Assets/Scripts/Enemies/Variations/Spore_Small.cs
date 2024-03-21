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

    private void OnEnable() {
        StartCoroutine(C_WaitForSpore_MindInstantiation(1, result => { Spore_Mind.instance.TotalSpore_Small += result; }));
    }

    private void OnDisable() {
        if (Spore_Mind.instance == null) {
            Debug.LogError("The spore died without a Spore mind");
        }
        Spore_Mind.instance.TotalSpore_Small--;
    }

    private IEnumerator C_WaitForSpore_MindInstantiation(int value, Action<int> action) {

        while (Spore_Mind.instance == null) {
            yield return null;
        }

        action?.Invoke(value);
    }

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
