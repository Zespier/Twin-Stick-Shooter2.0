using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootInConeState : AttackBaseState {

    public Transform shootPoint;
    public Transform rotationPoint;
    public float rotationLerpDivider = 0.1f;
    public ShootingPointRotationType shootingPointRotationType;
    public ShooterType shooterType;
    public GameObject bulletPrefab;
    public float fireRate = 8f;
    public float shootingDuration = -1f;
    public float angle = 90f;
    public float amountOfBullets = 5f;

    private float _shootTimer;
    private float _shootDurationTime;
    private float _anglePerBullet;

    public void Shoot() {
        Bullet newBullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity, BulletContainer.instance.transform).GetComponent<Bullet>();
        newBullet.Shoot(shootPoint.up, 2f);
    }

    private void RotateShootingPoint() {
        if (shootingPointRotationType == ShootingPointRotationType.fromRotationPointLookingAtPlayer) {

            Vector3 shootingDirection = (controller.player.position - transform.position).normalized;
            rotationPoint.up = Vector3.Lerp(rotationPoint.up, shootingDirection, Time.deltaTime / rotationLerpDivider);
        }
    }

    public override void OnStateEnter() {
        _shootDurationTime = 0;
        _shootTimer = Time.time;

        rotationPoint.up = controller.player.position - transform.position;

        if (shooterType == ShooterType.shootWithoutMoving) {
            controller.rb.velocity = Vector3.zero;
        }

        _anglePerBullet = angle / amountOfBullets;
    }

    public override void OnStateExit() {
    }

    public override void StateLateUpdate() {
        if (shootingDuration == -1) { return; }

        _shootDurationTime += Time.deltaTime;
        if (_shootDurationTime >= shootingDuration) {
            controller.FinishedShooting();
        }
    }

    public override void StateUpdate() {
        RotateShootingPoint();
        if (_shootTimer + 1f / fireRate < Time.time) {
            _shootTimer = Time.time;
            Shoot();
        }
    }
}
