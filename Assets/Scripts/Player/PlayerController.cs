using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : Damageable {

    public WeaponController weaponController;
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

            TakeDamage(transform.position, bullet.Damage, UnityEngine.Random.Range(0, 100) < 10, DamageType.PlayerDamaged);
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




//using System;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Users;
//using static GameControls;

//public enum InputState { Character, Building, Interface }
//public class InputManager : MonoBehaviour, ISystemActions, ICharacterActions, IBuildingActions, IInterfaceActions {
//    public static InputManager Instance { get; private set; }
//    public static GameControls GameControls { get; private set; }
//    public static string CurrentControlScheme = "";

//    public static Action<string, ControllerType> OnControlSchemeChanged;

//    // System Map Events
//    public static Action<InputAction.CallbackContext> OnGamePause;
//    public static Action<InputAction.CallbackContext> OnBuildingToggled;
//    public static Action<InputAction.CallbackContext> OnInventoryToggled;

//    // Character Map Events
//    public static Action<InputAction.CallbackContext> OnCharacterAbility;
//    public static Action<InputAction.CallbackContext> OnCharacterAttack;
//    public static Action<InputAction.CallbackContext> OnCharacterInteract;
//    public static Action<InputAction.CallbackContext> OnCharacterSprint;

//    // Building Map Events
//    public static Action<InputAction.CallbackContext> OnBuildConfirmed;
//    public static Action<InputAction.CallbackContext> OnBuildNavigation;
//    public static Action<InputAction.CallbackContext> OnBuildBranchSwitched;

//    // Interface Map Events
//    public static Action<InputAction.CallbackContext> OnInterfaceBack;
//    public static Action<InputAction.CallbackContext> OnInterfaceSwitchBranch;
//    public static Action<InputAction.CallbackContext> OnInterfaceSwitchMenu;
//    public static Action<InputAction.CallbackContext> OnInterfaceConfirm;
//    public static Action<InputAction.CallbackContext> OnInterfaceDetails;
//    public static Action<InputAction.CallbackContext> OnInterfaceInteraction;
//    public static Action<InputAction.CallbackContext> OnInterfaceNavigation;

//    // Hacks
//    public static Action<InputAction.CallbackContext> HACK_OnChangeSkin;

//    // Character Variables
//    public static Vector2 Movement => GameControls.Character.Movement.ReadValue<Vector2>();
//    public static Vector2 ViewDirection => GameControls.Character.View.ReadValue<Vector2>();
//    public static bool IsMoving => Movement.magnitude > 0.1f;

//    // Building Variables
//    public static Vector2 BuildingRadialSelection => GameControls.Building.RadialSelection.ReadValue<Vector2>();

//    private void Awake() {
//        if (Instance == null) {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//            GameControls = new GameControls();

//            GameControls.System.SetCallbacks(this);
//            GameControls.Character.SetCallbacks(this);
//            GameControls.Building.SetCallbacks(this);
//            GameControls.Interface.SetCallbacks(this);

//            GameControls.System.Enable();

//            InputUser.onChange += OnInputDeviceChange;

//            SetInputState(InputState.Character);
//        } else {
//            Destroy(gameObject);
//        }
//    }

//    private void OnDisable() {
//        InputUser.onChange -= OnInputDeviceChange;

//        GameControls.Disable();
//    }

//    public void SetInputState(InputState newState) {
//        // desactivamos todo
//        GameControls.Character.Disable();
//        GameControls.Building.Disable();
//        GameControls.Interface.Disable();

//        switch (newState) {
//            case InputState.Character:
//                Debug.Log("character enabled");
//                GameControls.Character.Enable();
//                break;
//            case InputState.Building:
//                // Debug.Log("building enabled");
//                GameControls.Building.Enable();
//                break;
//            case InputState.Interface:
//                GameControls.Interface.Enable();
//                // Debug.Log("interface enabled");
//                break;
//        }
//    }

//    private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
//        if (change == InputUserChange.ControlSchemeChanged) {
//            Debug.Log($"Control Scheme Changed: {user.controlScheme.Value.name}\nController type: {Controller.GetControllerType()}");

//            CurrentControlScheme = user.controlScheme.Value.name;

//            OnControlSchemeChanged?.Invoke(CurrentControlScheme, Controller.GetControllerType());
//        }

//        if (change == InputUserChange.DevicePaired) {
//        }
//    }

//    #region System Actions
//    public void OnPause(InputAction.CallbackContext context) {
//        OnGamePause?.Invoke(context);
//    }
//    #endregion


//    #region Character Actions
//    public void OnAttack(InputAction.CallbackContext context) {
//        OnCharacterAttack?.Invoke(context);
//    }

//    public void OnChangeSkin(InputAction.CallbackContext context) {
//        HACK_OnChangeSkin?.Invoke(context);
//    }

//    public void OnDance(InputAction.CallbackContext context) {
//    }

//    public void OnInteract(InputAction.CallbackContext context) {
//        OnCharacterInteract?.Invoke(context);
//    }

//    public void OnJump(InputAction.CallbackContext context) {
//    }

//    public void OnMovement(InputAction.CallbackContext context) {
//    }

//    public void OnSpecialAbility(InputAction.CallbackContext context) {
//        OnCharacterAbility?.Invoke(context);
//    }

//    public void OnSprint(InputAction.CallbackContext context) {
//        OnCharacterSprint?.Invoke(context);
//    }

//    public void OnToggleInventory(InputAction.CallbackContext context) {
//        OnInventoryToggled?.Invoke(context);
//    }

//    public void OnView(InputAction.CallbackContext context) {
//    }
//    #endregion

//    public void OnToggleBuilder(InputAction.CallbackContext context) {
//        OnBuildingToggled?.Invoke(context);
//    }

//    #region Building Actions
//    public void OnConfirmPlacement(InputAction.CallbackContext context) {
//        OnBuildConfirmed?.Invoke(context);
//    }

//    public void OnCycleUpgradesBranch(InputAction.CallbackContext context) {
//        OnBuildBranchSwitched?.Invoke(context);
//    }

//    public void OnRadialSelection(InputAction.CallbackContext context) {
//        OnBuildNavigation?.Invoke(context);
//    }
//    #endregion

//    #region Interface Actions
//    public void OnNavigate(InputAction.CallbackContext context) {
//        OnInterfaceNavigation?.Invoke(context);
//    }

//    public void OnSubmit(InputAction.CallbackContext context) {
//    }

//    public void OnCancel(InputAction.CallbackContext context) {
//    }

//    public void OnPoint(InputAction.CallbackContext context) {
//    }

//    public void OnClick(InputAction.CallbackContext context) {
//    }

//    public void OnScrollWheel(InputAction.CallbackContext context) {
//    }

//    public void OnMiddleClick(InputAction.CallbackContext context) {
//    }

//    public void OnRightClick(InputAction.CallbackContext context) {
//    }

//    public void OnTrackedDevicePosition(InputAction.CallbackContext context) {
//    }

//    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) {
//    }

//    public void OnSwitchBranch(InputAction.CallbackContext context) {
//        OnInterfaceSwitchBranch?.Invoke(context);
//    }

//    public void OnSwitchMenu(InputAction.CallbackContext context) {
//        OnInterfaceSwitchMenu?.Invoke(context);
//    }

//    public void OnAccept(InputAction.CallbackContext context) {
//        OnInterfaceInteraction?.Invoke(context);
//    }

//    public void OnDetails(InputAction.CallbackContext context) {
//        OnInterfaceDetails?.Invoke(context);
//    }

//    public void OnBack(InputAction.CallbackContext context) {
//        OnInterfaceBack?.Invoke(context);
//    }

//    public void OnConfirm(InputAction.CallbackContext context) {
//        OnInterfaceConfirm?.Invoke(context);
//    }
//    #endregion
//}

