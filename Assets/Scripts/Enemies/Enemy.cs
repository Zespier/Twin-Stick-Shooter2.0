using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : Damageable {

    [Header("Drops")]
    public List<MineralDrop> mineralDrops;
    public int monedaBarataDrop;
    public int monedaCaraDrop;

    [Header("Enemy Base Attributes")]
    public EnemyType type;
    [SerializeField] private float speed = 10f;
    public Transform body;
    public float baseDamage = 1f;
    public float attackRate = 1f;
    [SerializeField] private float distanceToReachPlayer = 0.6f;
    [SerializeField] private float rotationLerpSpeed = 0.1f;

    public virtual float Speed => speed;
    public virtual float DistanceToReachPlayer => distanceToReachPlayer;
    public virtual float RotationLerpSpeed => rotationLerpSpeed;
    public WaveEnemy AssetReference { get; set; }

    [Header("Damageable")]
    public float hp = 1000f;
    public Stats stats;

    [Header("States")]
    public List<AttackBaseState> states;
    [HideInInspector] public AttackBaseState currentState;

    public NetworkVariable<Vector3> NetPosition =
    new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Server
    );

    protected virtual void Awake() {
        if (states != null && states.Count > 0) {
            currentState = states[0];
        } else {
            Debug.LogError("You forgot to put states in this enemy: " + this.GetType().ToString());
        }

        hp = stats.HP;
    }

    public override void OnNetworkSpawn() { 
        base.OnNetworkSpawn();

        if (IsServer) {
            EnemyContainer.instance.AddEnemy(this);
        }
    }

    public override void OnNetworkDespawn() {

        if (IsServer) {
            EnemyContainer.instance.RemoveEnemy(this);
        }

        gameObject.SetActive(false); //Safe to o after the despawn

        base.OnNetworkDespawn();
    }

    protected virtual void Update() {
        if (IsServer) {
            ServerTick();
        }

        if (!IsServer) {
            ClientTick();
        }
    }

    private void ServerTick() {
        if (Ship.instanceOfClient == null) return;
        if (Ship.instanceOfClient._dead) return;

        RotateBody();
        currentState.StateUpdate();

        NetPosition.Value = transform.position;
    }

    private void ClientTick() {
        transform.position = Vector3.Lerp(transform.position, NetPosition.Value, Time.deltaTime * 10f);
    }

    private void LateUpdate() {
        if (Ship.instanceOfClient == null) { return; }

        if (Ship.instanceOfClient._dead) {
            return;
        }

        currentState.StateLateUpdate();
    }

    public void InitializeHP() {
        hp = stats.HP;
    }

    protected virtual void RotateBody() {
        Vector3 targetLookDirection = Ship.instanceOfClient.transform.position - transform.position;
        targetLookDirection.y = 0f;
        body.forward = Vector3.Lerp(body.forward, targetLookDirection, Time.deltaTime / RotationLerpSpeed);
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

    public override void TakeDamage(Vector3 position, float damage, bool crit, DamageType damageType) {

        base.TakeDamage(position, damage, crit, damageType);

        hp -= damage;
        EnemySpawner.instance.EnemyTookDamage(AssetReference, (hp / stats.HP) * 100f);
        CheckDeath();
    }

    protected virtual void CheckDeath() {
        if (hp < 0) {
            DeathDrops();
            GameEvents.OnEnemyDeath?.Invoke(this);
            Deactivate();
        }
    }

    public void DeathDrops() {
        MineralsDropManager.instance.DropMinerals(this);

        Ship.instanceOfClient.monedaBarata += monedaBarataDrop;
        Ship.instanceOfClient.monedaCara += monedaCaraDrop;
    }

    public void Deactivate() {
        Events.OnEnemyDeath?.Invoke(this);

        //gameObject.SetActive(false);
        AudioManager.instance.ExplosionSound(transform.position, "enemy");

        MejoritasRecogiblesManager.instance.SpawnMejoritaRecogible(transform.position);
        GetComponent<NetworkObject>().Despawn();
    }

    public void ResetSpecificVariables() {
        InitializeHP();
    }

    #endregion
}
