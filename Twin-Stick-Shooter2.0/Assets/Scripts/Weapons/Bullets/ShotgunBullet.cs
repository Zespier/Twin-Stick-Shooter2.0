using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class ShotgunBullet : Bullet {

    public int bounces = 3;

    private int _startBounces;

    private void Awake() {
        _startBounces = bounces;
        _timeToDie = 50;
    }

    public override void Deactivate() {
        bounces--;

        if (bounces < 0) {
            gameObject.SetActive(false);
            _deathTimer = 0;
            bounces = _startBounces;
        }
    }

}
