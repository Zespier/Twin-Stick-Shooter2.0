using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour, IBullet {

    public Rigidbody rb;
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
    }

    public void Shoot(Vector3 direction, float desviationAngle, Stats ownerStats) {
        Rotate(direction);
        transform.forward = BulletFireDesviation.RandomBulletFireDesviation(transform, desviationAngle);
        rb.linearVelocity = speed * transform.forward;
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
