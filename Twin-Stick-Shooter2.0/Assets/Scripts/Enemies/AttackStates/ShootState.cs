using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : AttackBaseState {

    public Transform shootPoint;
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
        Bullet newBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity, BulletContainer.instance.transform).GetComponent<Bullet>();
        newBullet.Shoot(shootPoint.forward, 2f, controller.stats);

        AudioManager.instance.EnemyLaserSound(transform.position);
    }

    public override void OnStateEnter() {
        _shootDurationTime = 0;
        _shootTimer = Time.time;

        controller.rb.velocity = Vector3.zero;
    }

    public override void OnStateExit() {
    }

    /// <summary>
    /// Checks if the player is out of reach or finished shooting
    /// </summary>
    public override void StateLateUpdate() {

        if (changeStateWhenPlayerOutOfReach && Vector3.Distance(controller.player.position, transform.position) > controller.DistanceToReachPlayer) {
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
