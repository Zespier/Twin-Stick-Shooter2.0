using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Spore_Small : Enemy {

    [Header("Spore_Small Attributes")]
    public float speed_Spore_Small = 15f;
    public float distanceToReachPlayer_Spore_Small = 0.1f;

    private float _attackTimer;

    public override float Speed => speed_Spore_Small;
    public override float DistanceToReachPlayer => distanceToReachPlayer_Spore_Small;

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

        if (_attackTimer + (1f / attackRate) < Time.time) {

            _attackTimer = Time.time;
            Attack();

        } else {
            return;
        }
    }

    private void Attack() {
        //TODO: Damageplayer
    }

}
