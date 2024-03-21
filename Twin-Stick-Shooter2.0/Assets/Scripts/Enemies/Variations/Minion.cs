using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Enemy {

    [Header("Minion Attributes")]
    public float speed_Spore_Small = 5f;
    public float distanceToReachPlayer_Boss_Small = 4f;
    public float rotationLerpSpeed_Minion = 0.3f;


    public override float Speed => speed_Spore_Small;
    public override float DistanceToReachPlayer => distanceToReachPlayer_Boss_Small;
    public override float RotationLerpSpeed => rotationLerpSpeed_Minion;

    public override void ReachingPlayer() {
        ChangeState(typeof(ShootState));
    }

    public override void PlayerOutOfReach() {
        ChangeState(typeof(ShootMovingState));
    }
}
