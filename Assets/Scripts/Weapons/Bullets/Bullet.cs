using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour, IBullet {

    public float speed = 20f;
    public bool destroyOutOfCamera = false;
    public WeaponController weaponController;

    protected Stats _ownerStats;
    protected float _deathTimer;
    protected float _timeToDie = 3f;

    public float Damage => _ownerStats.Atk;

    private void Awake() {
        if (!IsServer) { return; }
    }

    protected virtual void Update() {
        if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsListening) { return; }
        if (!IsServer) { return; }
        _deathTimer += Time.deltaTime;

        if (_deathTimer >= _timeToDie) {
            Deactivate();
        }

        if (destroyOutOfCamera && !weaponController.bulletLivingArea.Contains(transform.position)) {
            Deactivate();
        }

        Movement();
    }

    private void Movement() {
        //Move and clamp
        Vector3 newPosition = transform.position;
        newPosition += Time.deltaTime * speed * transform.forward;
        newPosition = new Vector3(newPosition.x, 0, newPosition.z);

        transform.position = newPosition;
    }

    public void Shoot(Vector3 position, Vector3 direction, float desviationAngle, Stats ownerStats) {
        Rotate(direction);
        transform.forward = BulletFireDesviation.RandomBulletFireDesviation(transform, desviationAngle);
        speed = Random.Range(28 - 0.5f, 28 + 0.5f);
        this._ownerStats = ownerStats;
    }

    private void Rotate(Vector3 direction) {
        transform.forward = direction;
    }

    public virtual void Deactivate() {
        gameObject.SetActive(false);
        _deathTimer = 0;
    }
}
