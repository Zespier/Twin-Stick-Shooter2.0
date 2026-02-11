using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : Damageable {

    public Transform body;
    public PlayerInputs playerInputs;
    public float hp = 2000f;
    public ParticleSystem deathExplosion;
    public GameObject mesh;
    //public PlayerHealth playerHealth;
    public Stats Stats;
    public Queue<MovementInput> pendingInputs = new();

    [HideInInspector] public bool _dead;
    private float _maxHp;
    private int _currentTick;
    private Vector3 _authoritativePosition;
    private Vector3 _authoritativeLerpedDirection;
    private Vector3 _predictedPosition;
    private Queue<PredictedPosition> _predictedPositionsQueue = new();
    [HideInInspector] public Vector2 _lastMovementDirectionForRotation;

    public static PlayerController instance;

    public override void OnNetworkSpawn() {
        if (IsOwner) {
            if (!instance) {
                instance = this;
            }

            playerInputs = new PlayerInputs();
            playerInputs.Enable();
            _maxHp = hp;
        }
    }

    private void OnDisable() {
        playerInputs.Disable();
    }

    private void Update() {
        if (NetworkManager.Singleton == null) { return; }
        if (!IsOwner) { return; }
        if (_dead) { return; }

        Movement();
        Rotation();
    }

    //THIS IS THE CONCEPT OF THE MOVEMENT
    //Server moves you
    //1 2 3 4 5 6 7 8 9    THE SERVER SAYS YOU ARE HERE, EVERYONE SEES THIS 

    //1 2 3 4 5 6 7 8 9 1 2 3 4 5 6 7 8 9 //YOU ARE POOR AND DON'T HAVE INTERNET, so you want to be where the server says + all those inputs
    private void Movement() {

        Vector3 inputDirection = playerInputs.Player.Move.ReadValue<Vector2>();
        _lastMovementDirectionForRotation = inputDirection;

        //Move and clamp
        _predictedPosition += Time.deltaTime * Stats.Speed * new Vector3(inputDirection.x, 0, inputDirection.y);
        _predictedPosition = new Vector3(_predictedPosition.x, 0, _predictedPosition.z);

        transform.position = _predictedPosition;

        MovementInput input = new() {
            tick = _currentTick++,
            direction = inputDirection,
        };

        PredictedPosition predictedPosition = new() {
            tick = input.tick,
            predictedPositionThisTick = _predictedPosition,
        };

        pendingInputs.Enqueue(input);
        _predictedPositionsQueue.Enqueue(predictedPosition);

        SendMovementInputsServerRpc(input);
    }

    [ServerRpc]
    public void SendMovementInputsServerRpc(MovementInput input) {

        //TODO: ADD LERP to the movement
        //_authoritativeLerpedDirection = Vector2.Lerp(_authoritativeLerpedDirection, input.direction, Time.deltaTime / 0.1f);
        _authoritativePosition += Time.deltaTime * Stats.Speed * new Vector3(input.direction.x, 0, input.direction.y);
        _authoritativePosition = new Vector3(_authoritativePosition.x, 0, _authoritativePosition.z);
        AudioManager.instance.ShipSound(_authoritativeLerpedDirection * Stats.Speed);

        SendStateClientRpc(_authoritativePosition, input);
    }

    //This is for recalculation
    [ClientRpc]
    void SendStateClientRpc(Vector3 serverPos, MovementInput processedInput) {
        if (!IsOwner) { return; }

        //Every frame the player is predicting his movement, sends the input and where he thinks he is at that tick, when the server checkes and moves him, we have to see if the player was correct, if there is suficient error, then recolocate the player.
        Vector3 predictedPositionOnServerTick = Vector3.zero;

        while (_predictedPositionsQueue.Count > 0) {

            PredictedPosition _predictedPosition = _predictedPositionsQueue.Dequeue();
            if (_predictedPosition.tick == processedInput.tick) {
                predictedPositionOnServerTick = _predictedPosition.predictedPositionThisTick;
            }
        }

        float error = Vector3.Distance(serverPos, predictedPositionOnServerTick);
        if (error > 0.05f) {
            //Reset the prediction to the position of the tick processed
            _predictedPosition = serverPos;
            _predictedPositionsQueue.Clear();

            //And again, the player predict his position based on the server + his own inputs that are still on the way to be checked
            foreach (var input in pendingInputs) {
                if (input.tick > processedInput.tick) {
                    _predictedPosition += Time.deltaTime * Stats.Speed * new Vector3(input.direction.x, 0, input.direction.y);
                    _predictedPosition = new Vector3(_predictedPosition.x, 0, _predictedPosition.z);

                    PredictedPosition predictedPosition = new() {
                        tick = input.tick,
                        predictedPositionThisTick = _predictedPosition,
                    };

                    _predictedPositionsQueue.Enqueue(predictedPosition);
                }
            }

            transform.position = _predictedPosition;
        }

        // Remove all inputs processed by the server
        while (pendingInputs.Count > 0 &&
               pendingInputs.Peek().tick <= processedInput.tick) {
            pendingInputs.Dequeue();
        }
    }

    private void Rotation() {

        Vector2 lookValue = GetLookValue();
        Vector3 lookValue3D = new Vector3(lookValue.x, 0, lookValue.y);

        body.forward = Vector3.Lerp(body.forward, lookValue3D, Time.deltaTime / 0.03f);

        Events.OnTargetMove?.Invoke(lookValue3D.normalized);
    }

    private Vector2 GetLookValue() {
        Vector2 lookValue = playerInputs.Player.Look.ReadValue<Vector2>();


        if (lookValue == Vector2.zero) {
            lookValue = _lastMovementDirectionForRotation;
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
        //playerHealth.ReduceHealthBar(hp, Stats.HP);
        if (hp < 0) {
            Death();
        }
    }

    public void Death() {
        mesh.SetActive(false);
        deathExplosion.Play();
        _dead = true;

        CameraBehaviour.instance.CameraShake();

        AudioManager.instance.ExplosionSound(transform.position, "player");
        GameOver.instance.ShowGameOverPanel();

        UpgradeCardManager.instance.canvas.SetActive(false);
        Time.timeScale = 1;
        MenuDeTrucos.instance.Canvas_SetActive(false);
    }

    public void Heal() {
        hp = _maxHp;

        //playerHealth.ReduceHealthBar(hp, Stats.HP);

    }
}

[System.Serializable]
public struct MovementInput : INetworkSerializable {
    public int tick;
    public Vector2 direction;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {

        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref direction);
    }
}

public struct PredictedPosition {
    public int tick;
    public Vector3 predictedPositionThisTick;
}