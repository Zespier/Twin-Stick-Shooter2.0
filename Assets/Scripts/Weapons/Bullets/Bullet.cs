using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour, IBullet {

    public bool isEnemyBullet;
    public float hitRadius = 1f;
    public float speed = 20f;
    public bool destroyOutOfCamera = false;

    protected Stats _ownerStats;
    protected float _deathTimer;
    protected float _timeToDie = 3f;

    public float Damage => _ownerStats.Atk;

    private void Awake() {
        if (!IsServer) { return; }
    }

    protected virtual void Update() {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) { return; }
        if (!IsServer) { return; }
        _deathTimer += Time.deltaTime;

        if (_deathTimer >= _timeToDie) {
            Deactivate();
        }

        if (destroyOutOfCamera && !PlayerController.instance.weaponController.bulletLivingArea.Contains(transform.position)) {
            Deactivate();
        }

        Movement();
        CheckDamage();
    }

    private void Movement() {
        //Move and clamp
        Vector3 newPosition = transform.position;
        newPosition += Time.deltaTime * speed * transform.forward;
        newPosition = new Vector3(newPosition.x, 0, newPosition.z);

        transform.position = newPosition;
    }

    public void Shoot(Vector3 direction, float desviationAngle, Stats ownerStats, bool spawnNetworkObject = false) {
        Rotate(direction);
        transform.forward = BulletFireDesviation.RandomBulletFireDesviation(transform, desviationAngle);
        speed = Random.Range(28 - 0.5f, 28 + 0.5f);
        this._ownerStats = ownerStats;
        if (spawnNetworkObject) {
            GetComponent<NetworkObject>().Spawn();
        }
    }

    private void Rotate(Vector3 direction) {
        transform.forward = direction;
    }

    public virtual void Deactivate() {
        FeedbackController.instance.Particles(ParticleType.smallExplosion, transform.position, Vector3.forward);
        AudioManager.instance.PlayBulletExplosionAgainstTheWall(transform.position);
        gameObject.SetActive(false);
        _deathTimer = 0;
    }

    //The damage works a little different, real bullets move on the server, so the enemies are on the same tick, and so, I can perfectly hit them without any predictions.
    //TODO: Maybe send to the clients that the enemy received damage so they can see the numbers? Yeah, I want the numbers from other players too
    public void CheckDamage() {
        if (isEnemyBullet) {

            if ((transform.position - PlayerController.instance.transform.position).sqrMagnitude < hitRadius * hitRadius) {
                PlayerController.instance.TakeDamage(transform.position, damage: Damage, Random.Range(0, 100) < 10, DamageType.PlayerDamaged);
                Deactivate();
            }


        } else {

            for (int i = 0; i < EnemyContainer.instance.activeEnemies.Count; i++) {
                Enemy enemy = EnemyContainer.instance.activeEnemies[i];
                if ((transform.position - enemy.transform.position).sqrMagnitude < hitRadius * hitRadius) {
                    EnemyContainer.instance.activeEnemies[i].TakeDamage(transform.position, damage: Damage, Random.Range(0, 100) < 10, DamageType.DefaultWhite);
                    Deactivate();
                }
            }
        }
    }
}
