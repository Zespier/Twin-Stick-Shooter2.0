using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy {

    [Header("Wolf Attributes")]
    public float wolfSpeed = 20f;
    public Transform movementDirectionHelper;

    public override float Speed => wolfSpeed;

}
