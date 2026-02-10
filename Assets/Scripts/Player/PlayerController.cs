using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Damageable {

    public Rigidbody rb;
    public Transform body;
    public PlayerInputs playerInputs;
    public float hp = 2000f;
    public ParticleSystem deathExplosion;
    public GameObject mesh;
    public PlayerHealth playerHealth;
    public TrailRenderer trailRenderer;
    public Stats Stats;
    public Queue<MovementInput> pendingInputs;

    [HideInInspector] public bool _dead;
    [HideInInspector] public Vector2 _moveValue;
    [HideInInspector] public Vector2 _authoritativeMoveDirectionLerped;
    private float _maxHp;
    private int _currentTick;
    private int _lastProcessedTick;
    private Vector3 _authoritativePosition;
    private Vector3 _predictedPosition;

    public static PlayerController instance;
    private void Awake() {
        Application.targetFrameRate = 100;
        if (!instance) {
            instance = this;
        }

        playerInputs = new PlayerInputs();
        _maxHp = hp;
    }

    private void OnEnable() {
        playerInputs.Enable();
    }

    private void OnDisable() {
        playerInputs.Disable();
    }

    private void Update() {
        if (!IsOwner) { return; }
        if (_dead) { return; }

        Movement();
        Rotation();
    }

    private void Movement() {

        //if (UpgradeCardManager.instance != null && UpgradeCardManager.instance.canvas.gameObject.activeSelf) {
        //    _moveValue = Vector2.zero;
        //    return;

        //} else {
        //_moveValue = playerInputs.Player.Move.ReadValue<Vector2>();
        //}
        MovementInput input = new() {
            tick = _currentTick++,
            direction = playerInputs.Player.Move.ReadValue<Vector2>(),
        };

        pendingInputs.Enqueue(input);

        //Predict the position from the last point the server synced
        Vector3 predictedPosition = _authoritativePosition;

        Vector3 _predictedDirectionLerped = _authoritativeMoveDirectionLerped;

        foreach (var pendingInput in pendingInputs) {
            if (pendingInput.tick > _lastProcessedTick) {
                _predictedDirectionLerped = Vector2.Lerp(_predictedDirectionLerped, input.direction, Time.deltaTime / 0.1f);

                predictedPosition += Time.deltaTime * Stats.Speed * new Vector3(_predictedDirectionLerped.x, 0, _predictedDirectionLerped.y);
            }
        }

        while (pendingInputs.Count > 0 && pendingInputs.Peek().tick <= _lastProcessedTick) {
            pendingInputs.Dequeue();
        }

        predictedPosition = new Vector3(predictedPosition.x, 0, predictedPosition.z);

        transform.position += predictedPosition;

        SendMovementInputsServerRpc(input);
    }

    [ServerRpc]
    public void SendMovementInputsServerRpc(MovementInput input) {

        _authoritativeMoveDirectionLerped = Vector2.Lerp(_authoritativeMoveDirectionLerped, input.direction, Time.deltaTime / 0.1f);
        _authoritativePosition += Time.deltaTime * Stats.Speed * new Vector3(_authoritativeMoveDirectionLerped.x, 0, _authoritativeMoveDirectionLerped.y);
        _authoritativePosition = new Vector3(_authoritativePosition.x, 0, _authoritativePosition.z);
        AudioManager.instance.ShipSound(_authoritativeMoveDirectionLerped * Stats.Speed);

        _lastProcessedTick = input.tick;
        SendStateClientRpc(_authoritativePosition, _lastProcessedTick);
    }

    [ClientRpc]
    void SendStateClientRpc(Vector3 serverPos, int serverTick) {
        if (!IsOwner) { return; }

        // If prediction was wrong
        float error = Vector3.Distance(_predictedPosition, serverPos);
        if (error > 0.01f) {
            _predictedPosition = serverPos;

            // Replay inputs the server hasn't seen yet
            foreach (var input in pendingInputs) {
                if (input.tick > serverTick) {
                    _predictedPosition += new Vector3(input.move.x, 0, input.move.y)
                                          * speed * Time.deltaTime;
                }
            }

            transform.position = _predictedPosition;
        }

        // Remove confirmed inputs
        while (pendingInputs.Count > 0 &&
               pendingInputs.Peek().tick <= serverTick) {
            pendingInputs.Dequeue();
        }
    }

    private void Rotation() {
        if (UpgradeCardManager.instance != null && UpgradeCardManager.instance.canvas.gameObject.activeSelf) {
            return;
        }

        Vector2 lookValue = GetLookValue();
        Vector3 lookValue3D = new Vector3(lookValue.x, 0, lookValue.y);

        body.forward = Vector3.Lerp(body.forward, lookValue3D, Time.deltaTime / 0.03f);

        Events.OnTargetMove?.Invoke(lookValue3D.normalized);
    }

    /// <summary>
    /// Gets the look value
    /// </summary>
    /// <returns></returns>
    private Vector2 GetLookValue() {
        Vector2 lookValue = playerInputs.Player.Look.ReadValue<Vector2>();

        if (lookValue == Vector2.zero) {
            lookValue = _moveValue;
        }

        return lookValue;
    }

    private void OnTriggerEnter(Collider collision) {
        if (_dead) {
            return;
        }

        if (collision.GetComponent<Collider>().TryGetComponent(out IBullet bullet)) {

            AudioManager.instance.PlayBulletExplosionAgainstTheWall(collision.transform.position);
            FeedbackController.instance.Particles(ParticleType.smallExplosion, collision.transform.position, Vector3.forward);

            TakeDamage(transform.position, bullet.Damage, Random.Range(0, 100) < 10, DamageType.PlayerDamaged);
            bullet.Deactivate();
            RemoveHealth(bullet.Damage);
        }
    }

    public void RemoveHealth(float amount) {
        hp -= amount;
        playerHealth.ReduceHealthBar(hp, Stats.HP);
        if (hp < 0) {
            Death();
        }
    }

    public void Death() {
        mesh.SetActive(false);
        deathExplosion.Play();
        _dead = true;
        rb.linearVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        CameraBehaviour.instance.CameraShake();

        AudioManager.instance.ExplosionSound(transform.position, "player");
        GameOver.instance.ShowGameOverPanel();

        UpgradeCardManager.instance.canvas.SetActive(false);
        Time.timeScale = 1;
        MenuDeTrucos.instance.Canvas_SetActive(false);
    }

    public void Heal() {
        hp = _maxHp;

        playerHealth.ReduceHealthBar(hp, Stats.HP);

    }
}

public struct MovementInput {
    public int tick;
    public Vector2 direction;
}