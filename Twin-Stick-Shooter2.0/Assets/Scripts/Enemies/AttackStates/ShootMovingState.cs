using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMovingState : AttackBaseState {

    public float shootingMovementSpeed = 4f;
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public float fireRate = 8f;
    public float shootingDuration = -1f;


    private float _shootTimer;
    private float _shootDurationTime;

    /// <summary>
    /// Shots a bullet
    /// </summary>
    public void Shoot() {
        Bullet newBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity, BulletContainer.instance.transform).GetComponent<Bullet>();
        newBullet.Shoot(shootPoint.forward, 2f);

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

        if (Vector3.Distance(controller.player.position, transform.position) < controller.DistanceToReachPlayer) {
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

        controller.rb.velocity = (controller.player.position - transform.position).normalized * shootingMovementSpeed;

        if (_shootTimer + 1f / fireRate < Time.time) {
            _shootTimer = Time.time;
            Shoot();
        }
    }
}
