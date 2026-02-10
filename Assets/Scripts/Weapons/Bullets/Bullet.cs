using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour, IBullet {

    public float speed = 20f;
    public bool destroyOutOfCamera = false;

    public WeaponController weaponController;
    protected Stats ownerStats;
    protected float _deathTimer;
    protected float _timeToDie = 3f;

    public float Damage => ownerStats.Atk;

    protected virtual void Update() {
        _deathTimer += Time.deltaTime;

        if (_deathTimer >= _timeToDie) {
            Deactivate();
        }

        if (!destroyOutOfCamera) { return; }
        if (!weaponController.bulletLivingArea.Contains(transform.position)) {
            Deactivate();
        }

        if (IsServer) {
            transform.position = Time.deltaTime * speed * Vector3.forward;
        }
    }

    public void Shoot(Vector3 direction, float desviationAngle, Stats ownerStats) {
        Rotate(direction);
        transform.forward = BulletFireDesviation.RandomBulletFireDesviation(transform, desviationAngle);
        speed = Random.Range(28 - 0.5f, 28 + 0.5f);
        this.ownerStats = ownerStats;
    }

    private void Rotate(Vector3 direction) {
        transform.forward = direction;
    }

    public virtual void Deactivate() {
        gameObject.SetActive(false);
        _deathTimer = 0;
    }
}
