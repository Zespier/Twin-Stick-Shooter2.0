using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore_Normal : Enemy {

    [Header("Spore_Normal Attributes")]
    public float speed_Spore_Normal = 6f;
    public float distanceToReachPlayer_Spore_Normal = 5f;

    public override float Speed => speed_Spore_Normal;
    public override float DistanceToReachPlayer => distanceToReachPlayer_Spore_Normal;

    private void OnEnable() {
        StartCoroutine(C_WaitForSpore_MindInstantiation(1, result => { Spore_Mind.instance.TotalSpore_Big += result; }));
    }

    private void OnDisable() {
        if (Spore_Mind.instance == null) {
            Debug.LogError("The spore died without a Spore mind");
        }
        Spore_Mind.instance.TotalSpore_Big--;
    }

    private IEnumerator C_WaitForSpore_MindInstantiation(int value, Action<int> action) {

        while (Spore_Mind.instance == null) {
            yield return null;
        }

        action?.Invoke(value);
    }

    #region Behaviour with States

    public override void FinishedShooting() {
        ChangeState(typeof(ZombieState));
    }

    #endregion

    public override void ReachingPlayer() {
        ChangeState(typeof(ShootInConeState));
    }

    private void Attack() {
        //TODO: Damageplayer
    }

}
