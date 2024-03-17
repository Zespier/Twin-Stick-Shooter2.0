using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable {

    [Header("Enemy Base Attributes")]
    public Rigidbody rb;
    public float speed = 10f;
    public Transform player;
    public float baseDamage = 1f;
    public float attackRate = 1f;
    [SerializeField] private float distanceToReachPlayer = 0.6f;

    public virtual float Speed => speed;
    public virtual float DistanceToReachPlayer => distanceToReachPlayer;

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
        currentState.StateUpdate();
    }

    private void LateUpdate() {
        currentState.StateLateUpdate();
    }

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

    public virtual void ReachingPlayer() {

    }

    #endregion

    #region Behaviour with States

    public virtual void FinishedShooting() {

    }

    public virtual void FinishedGuarding() {

    }

    #endregion

    #region Taking damage and deactivation

    public override void TakeDamage(Vector3 position, float damage, bool crit, string damageType) {
        base.TakeDamage(position, damage, crit, damageType);

        hp -= damage;
        CheckDeath();
    }

    private void CheckDeath() {
        if (hp < 0) {
            Deactivate();
        }
    }

    public void Deactivate() {
        gameObject.SetActive(false);

        Events.OnEnemyDeath(this);
    }

    #endregion
}
