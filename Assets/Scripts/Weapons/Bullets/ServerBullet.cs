using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerBullet : MonoBehaviour {

    public ushort id;

    public bool isEnemyBullet;
    public float hitRadius = 1f;
    public float speed = 20f;

    public float damage;
    public float damageMultiplier;
    protected float _deathTimer;
    protected float _timeToDie = 3f;


    private void OnEnable() {
        _deathTimer = Time.time;
    }

    protected virtual void Update() {

        if (Time.time - _deathTimer >= _timeToDie) {
            Deactivate();
        }

        //if (destroyOutOfCamera && !PlayerController.instance.weaponController.bulletLivingArea.Contains(transform.position)) {
        //    Deactivate();
        //}

        Movement();
        CheckDamage();
    }

    private void Movement() {
        Vector3 newPosition = transform.position;
        newPosition += Time.deltaTime * speed * transform.forward;
        newPosition = new Vector3(newPosition.x, 0, newPosition.z);

        transform.position = newPosition;
    }

    public virtual void Deactivate(bool avoidExplosionVFXAndSound = false) {
        if (!avoidExplosionVFXAndSound) {
            FeedbackController.instance.Particles(ParticleType.smallExplosion, transform.position, Vector3.forward);
            AudioManager.instance.PlayBulletExplosionAgainstTheWall(transform.position);
        }

        gameObject.SetActive(false);
        BulletContainer.instance.DeactivateGhostBulletClientRpc(id);
    }

    public void CheckDamage() {
        if (isEnemyBullet) {

            if ((transform.position - Ship.instanceOfClient.transform.position).sqrMagnitude < hitRadius * hitRadius) {
                Ship.instanceOfClient.TakeDamage(transform.position, damage * damageMultiplier, Random.Range(0, 100) < 10, DamageType.PlayerDamaged);
                Deactivate();
            }

        } else {

            for (int i = 0; i < EnemyContainer.instance.activeEnemies.Count; i++) {
                Enemy enemy = EnemyContainer.instance.activeEnemies[i];
                if ((transform.position - enemy.transform.position).sqrMagnitude < hitRadius * hitRadius) {
                    enemy.TakeDamage(transform.position, damage * damageMultiplier, Random.Range(0, 100) < 10, DamageType.DefaultWhite);
                    Deactivate();
                }
            }
        }
    }
}
