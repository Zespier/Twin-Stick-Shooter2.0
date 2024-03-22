using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable {

    [Header("Enemy Base Attributes")]
    public Rigidbody rb;
    [SerializeField] private float speed = 10f;
    public Transform player;
    public Transform body;
    public float baseDamage = 1f;
    public float attackRate = 1f;
    [SerializeField] private float distanceToReachPlayer = 0.6f;
    [SerializeField] private float rotationLerpSpeed = 0.1f;


    public virtual float Speed => speed;
    public virtual float DistanceToReachPlayer => distanceToReachPlayer;
    public virtual float RotationLerpSpeed => rotationLerpSpeed;

    [Header("Damageable")]
    public float hp = 1000f;

    [Header("States")]
    public List<AttackBaseState> states;
    [HideInInspector] public AttackBaseState currentState;

    protected virtual void Awake() {
        if (states != null && states.Count > 0) {
            currentState = states[0];
        } else {
            Debug.LogError("You forgot to put states in this enemy: " + this.GetType().ToString());
        }
    }

    private void Start() {
        currentState.OnStateEnter();
        player = PlayerController.instance.transform;
    }

    protected virtual void Update() {
        if (PlayerController.instance._dead) {
            rb.velocity = Vector3.zero;
            return;
        }

        RotateBody();
        currentState.StateUpdate();
    }

    private void LateUpdate() {
        if (PlayerController.instance._dead) {
            rb.velocity = Vector3.zero;
            return;
        }
        currentState.StateLateUpdate();
    }

    /// <summary>
    /// Rotates the body looking at the player with a lerp
    /// </summary>
    protected virtual void RotateBody() {
        Vector3 targetLookDirection = player.transform.position - transform.position;
        targetLookDirection.y = 0f;
        body.forward = Vector3.Lerp(body.forward, targetLookDirection, Time.deltaTime / RotationLerpSpeed);
    }

    /// <summary>
    /// Changes state, calling the OnStateExit and onStateEnter
    /// </summary>
    /// <param name="state"></param>
    public virtual void ChangeState(Type state) {
        if (currentState.GetType() == state) { return; }

        if (!states.Exists(s => s.GetType() == state)) {
            Debug.LogError("This state is not registered");
            return;
        }

        currentState.OnStateExit();
        currentState = states.Find(s => s.GetType() == state);
        currentState.OnStateEnter();
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.TryGetComponent(out IBullet bullet)) {
            TakeDamage(transform.position, bullet.Damage * bullet.BaseDamagePercentage, UnityEngine.Random.Range(0, 100) < 10, "energy");
            bullet.Deactivate();
            Events.OnBulletImpact?.Invoke(transform.position);
        }
    }

    #region Behaviour with player

    /// <summary>
    /// How each enemy reacts when reaching the player, every herited class that want to do something special needs to override this
    /// </summary>
    public virtual void ReachingPlayer() {

    }

    #endregion

    #region Behaviour with States

    /// <summary>
    /// How each enemy reacts when finished shooting, every herited class that want to do something special needs to override this
    /// </summary>
    public virtual void FinishedShooting() {

    }

    /// <summary>
    /// How each enemy reacts when finished guarding, every herited class that want to do something special needs to override this
    /// </summary>
    public virtual void FinishedGuarding() {

    }

    /// <summary>
    /// How each enemy reacts when the player is out of reach, every herited class that want to do something special needs to override this
    /// </summary>
    public virtual void PlayerOutOfReach() {

    }

    #endregion

    #region Taking damage and deactivation

    /// <summary>
    /// basic override for every enemy, it will send a damage feedback from the damageable and reduce health
    /// </summary>
    /// <param name="position"></param>
    /// <param name="damage"></param>
    /// <param name="crit"></param>
    /// <param name="damageType"></param>
    public override void TakeDamage(Vector3 position, float damage, bool crit, string damageType) {
        base.TakeDamage(position, damage, crit, damageType);

        hp -= damage;
        CheckDeath();
    }

    /// <summary>
    /// Checks if the enemy us dead
    /// </summary>
    private void CheckDeath() {
        if (hp < 0) {
            Deactivate();
        }
    }

    /// <summary>
    /// Deactivates the enemy for future pooling
    /// little explosion for feedback
    /// </summary>
    public void Deactivate() {
        Events.OnEnemyDeath?.Invoke(this);

        gameObject.SetActive(false);
        AudioManager.instance.ExplosionSound(transform.position, "enemy");

        EnemySpawner.instance.RemoveEnemyFromGenerated(this);
    }

    #endregion
}
