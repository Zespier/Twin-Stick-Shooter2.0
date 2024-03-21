using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore_Big : Enemy {

    [Header("Spore_Big Attributes")]
    public float speed_Spore_Big = 5f;
    public ParticleSystem explosion;
    public BoxCollider2D boxCollider;
    public GameObject spore_EggPrefab;
    public float spawnEggTime = 20f;

    private bool _exploded;
    private float _spawnTimer = 20f;

    public override float Speed => speed_Spore_Big;

    protected override void Awake() {
        base.Awake();
        _spawnTimer = spawnEggTime - 6;
    }

    private void OnEnable() {
        StartCoroutine(C_WaitForSpore_MindInstantiation(1, result => { Spore_Mind.instance.TotalSpore_Big += result; }));
    }

    private void OnDisable() {
        if (Spore_Mind.instance == null) {
            Debug.LogError("The spore died without a Spore mind");
            Spore_Mind.instance.TotalSpore_Big--;
        }
    }

    protected override void Update() {
        base.Update();

        if (_spawnTimer >= spawnEggTime) {
            Spawn();
        }
        _spawnTimer += Time.deltaTime;
    }


    protected void Spawn() {
        if (Spore_Mind.instance.TotalSpores >= 100) { return; }

        _spawnTimer = 0;

        Spore_Mind.instance.SpawnSpore(spore_EggPrefab, transform.position);
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
        Deactivate();
    }
}
