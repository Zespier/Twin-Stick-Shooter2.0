using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntState : AttackBaseState {

    public float selfieStickSize = 6f;
    public float huntTime = 2f;
    public List<Transform> shootPoints;
    public GameObject bulletPrefab;
    public float fireRate = 8f;
    public float bulletsToShot = 4f;

    private float _huntTimer;
    private float _shootTimer;
    private int _bulletsShot;
    private float _waitingToShoot;

    /// <summary>
    /// Shots a bullet
    /// </summary>
    public void Shoot() {
        for (int i = 0; i < shootPoints.Count; i++) {

            Bullet newBullet = Instantiate(bulletPrefab, shootPoints[i].position, Quaternion.identity, BulletContainer.instance.transform).GetComponent<Bullet>();
            newBullet.Shoot(shootPoints[i].forward, 1.1f, controller.stats, spawnNetworkObject: true);

        }
        AudioManager.instance.EnemyLaserSound(transform.position);

        _bulletsShot++;
    }

    public override void OnStateEnter() {
        _huntTimer = 0;
        _bulletsShot = 0;
        _waitingToShoot = 0;
    }

    public override void OnStateExit() {
    }

    /// <summary>
    /// Check wheter it should change to dash state
    /// </summary>
    public override void StateLateUpdate() {
        _huntTimer += Time.deltaTime;
        if (_huntTimer >= huntTime) {
            controller.ChangeState(typeof(DashState));
        }
    }

    /// <summary>
    /// Clamps the position to a circle around the player, so it can't scape
    /// </summary>
    public override void StateUpdate() {
        Vector3 newForward = PlayerController.instance.transform.position - transform.position;
        newForward.y = 0;
        controller.body.forward = newForward;

        Vector3 direction = controller.body.right;
        /*??*/
        transform.position += Time.deltaTime * controller.Speed * direction;

        Vector3 clampedPosition = PlayerController.instance.transform.position;
        clampedPosition -= controller.body.forward * selfieStickSize;
        clampedPosition.y = 0;

        /*??*/
        transform.position = Vector3.Lerp(transform.position, clampedPosition, Time.deltaTime / 0.27f);


        if (_shootTimer + 1f / fireRate < Time.time) {
            _shootTimer = Time.time;
            if (_bulletsShot < bulletsToShot && _waitingToShoot >= 1f) {
                Shoot();
            }
        }
        _waitingToShoot += Time.deltaTime;

    }
}
