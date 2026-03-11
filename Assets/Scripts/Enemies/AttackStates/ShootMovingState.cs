using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootMovingState : AttackBaseState {

    public float shootingMovementSpeed = 4f;
    public List<Transform> shootPoints;
    public GameObject bulletPrefab;
    public float fireRate = 8f;
    public float shootingDuration = -1f;

    private float _shootTimer;
    private float _shootDurationTime;

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
    /// Checks if reached the player or finished shooting
    /// </summary>
    public override void StateLateUpdate() {

        if (Vector3.Distance(Ship.instanceOfClient.transform.position, transform.position) < controller.DistanceToReachPlayer) {
            controller.ReachingPlayer();
        }

        if (shootingDuration == -1) { return; }

        _shootDurationTime += Time.deltaTime;
        if (_shootDurationTime >= shootingDuration) {
            controller.FinishedShooting();
        }
    }

    /// <summary>
    /// Moves and waits for next shoot
    /// </summary>
    public override void StateUpdate() {

        transform.position += Time.deltaTime * shootingMovementSpeed * (Ship.instanceOfClient.transform.position - transform.position).normalized;

        if (_shootTimer + 1f / fireRate < Time.time) {
            _shootTimer = Time.time;
            Shoot();
        }
    }
}
