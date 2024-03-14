using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IBullet {

    public Rigidbody rb;
    public float speed = 20f;
    public float damage = 1f;
    public float baseDamagePercentage = 100f;
    public bool destroyOutOfCamera = false;

    [HideInInspector] public WeaponController weaponController;
    protected float _deathTimer;
    protected float _timeToDie = 3f;

    public float Damage { get => damage; set => damage = value; }
    public float BaseDamagePercentage { get => (baseDamagePercentage/* + PlayerStats.instance.*/) / 100f; set => baseDamagePercentage = value; }

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

    /// <summary>
    /// Shoots this bullet in the direction specified with a random angle desviation
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="desviationAngle"></param>
    public void Shoot(Vector3 direction, float desviationAngle) {
        Rotate(direction);
        transform.forward = BulletFireDesviation.RandomBulletFireDesviation(transform, desviationAngle);
        rb.velocity = speed * transform.forward;
    }

    /// <summary>
    /// Rotates towards the specified direction
    /// </summary>
    /// <param name="direction"></param>
    private void Rotate(Vector3 direction) {
        transform.forward = direction;
    }

    /// <summary>
    /// Deactivates and resets the bullet, ready for pooling
    /// </summary>
    public virtual void Deactivate() {
        gameObject.SetActive(false);
        _deathTimer = 0;
    }
}
