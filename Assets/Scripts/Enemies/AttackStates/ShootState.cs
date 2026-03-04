using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootState : AttackBaseState {

    public List<Transform> shootPoints;
    public GameObject bulletPrefab;
    public float fireRate = 8f;
    public float shootingDuration = -1f;
    public bool changeStateWhenPlayerOutOfReach;

    private float _shootTimer;
    private float _shootDurationTime;

    /// <summary>
    /// Shoots a butllet
    /// </summary>
    public void Shoot() {

        for (int i = 0; i < shootPoints.Count; i++) {
            float speed = 20;
            Vector3 direction = BulletFireDesviation.RandomBulletFireDesviation(shootPoints[i], controller.stats.DesviationAngle);

            BulletContainer.instance.CreateBullet(shootPoints[i].position, direction, speed, true);
        }

        AudioManager.instance.EnemyLaserSound(transform.position);
    }

    public override void OnStateEnter() {
        _shootDurationTime = 0;
        _shootTimer = Time.time;
    }

    public override void OnStateExit() {
    }

    /// <summary>
    /// Checks if the player is out of reach or finished shooting
    /// </summary>
    public override void StateLateUpdate() {

        if (changeStateWhenPlayerOutOfReach && Vector3.Distance(Ship.instance.transform.position, transform.position) > controller.DistanceToReachPlayer) {
            controller.PlayerOutOfReach();
        }

        if (shootingDuration == -1) { return; }

        _shootDurationTime += Time.deltaTime;
        if (_shootDurationTime >= shootingDuration) {
            controller.FinishedShooting();
        }

    }

    /// <summary>
    /// Waits for next shoot
    /// </summary>
    public override void StateUpdate() {
        if (_shootTimer + 1f / fireRate < Time.time) {
            _shootTimer = Time.time;
            Shoot();
        }
    }
}
