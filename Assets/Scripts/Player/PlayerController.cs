using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using static GameControls;

public class PlayerController : Damageable {

    public WeaponController weaponController;
    public Transform body;
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

            _maxHp = hp;
        }
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

        Vector3 inputDirection = InputManager.Movement;
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
        Vector2 lookValue = InputManager.Look;

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

public static class Controller {

    public const float marginForControllerOutline = 50;

    #region Controller Recognition

    public static bool IsControllerConnected() {
        string[] controllers = Input.GetJoystickNames();

        foreach (var controller in controllers) {
            if (!string.IsNullOrEmpty(controller)) {
                return true;
            }
        }

        return false;
    }

    public static ControllerType GetControllerType() {
        string[] controllers = Input.GetJoystickNames();

        foreach (var controller in controllers) {
            if (!string.IsNullOrEmpty(controller)) {
                if (controller.ToLower().Contains("xbox")) {
                    return ControllerType.xbox;
                }
                if (controller.ToLower().Contains("wireless") || controller.ToLower().Contains("pc")) {
                    return ControllerType.playstation;
                }

                return ControllerType.notDefined;
            }
        }

        // esto b�sicamente significa teclado y rat�n
        return ControllerType.noController;
    }

    #endregion

    #region Controller Movement Trought Screen


    public static SelectableItemForController GetPosibleSelectableItem(List<SelectableItemForController> items, SelectableItemForController outlinedItem, OutlineDirection direction) {


        float closestRowHeightDifferenceToTheRight = float.MaxValue;
        float closestRowHeightDifferenceToTheLeft = float.MaxValue;
        float closestColumnHeightDifferenceToTheDown = float.MaxValue;
        float closestColumnHeightDifferenceToTheUp = float.MaxValue;
        for (int i = 0; i < items.Count; i++) {
            if (items[i] == outlinedItem) { continue; }

            if (items[i].rectTransform.position.x > outlinedItem.rectTransform.position.x && Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y) < closestRowHeightDifferenceToTheRight) {
                closestRowHeightDifferenceToTheRight = Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y);
            }
            if (items[i].rectTransform.position.x < outlinedItem.rectTransform.position.x && Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y) < closestRowHeightDifferenceToTheLeft) {
                closestRowHeightDifferenceToTheLeft = Mathf.Abs(outlinedItem.rectTransform.position.y - items[i].rectTransform.position.y);
            }
            if (items[i].rectTransform.position.y < outlinedItem.rectTransform.position.y && Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x) < closestColumnHeightDifferenceToTheDown) {
                closestColumnHeightDifferenceToTheDown = Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x);
            }
            if (items[i].rectTransform.position.y > outlinedItem.rectTransform.position.y && Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x) < closestColumnHeightDifferenceToTheUp) {
                closestColumnHeightDifferenceToTheUp = Mathf.Abs(outlinedItem.rectTransform.position.x - items[i].rectTransform.position.x);
            }
        }

        SelectableItemForController leftSlot = outlinedItem;
        SelectableItemForController rightSlot = outlinedItem;
        SelectableItemForController downSlot = outlinedItem;
        SelectableItemForController upSlot = outlinedItem;

        for (int i = 0; i < items.Count; i++) {
            if (items[i] == outlinedItem) { continue; }

            float heightDifference = Mathf.Abs(items[i].rectTransform.position.y - outlinedItem.rectTransform.position.y);
            if (items[i].rectTransform.position.x < leftSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheLeft) < marginForControllerOutline) {
                leftSlot = items[i];
            }
            if (items[i].rectTransform.position.x > rightSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheRight) < marginForControllerOutline) {
                rightSlot = items[i];
            }

            float widthDifference = Mathf.Abs(items[i].rectTransform.position.x - outlinedItem.rectTransform.position.x);
            if (items[i].rectTransform.position.y < downSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheDown) < marginForControllerOutline) {
                downSlot = items[i];
            }
            if (items[i].rectTransform.position.y > upSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheUp) < marginForControllerOutline) {
                upSlot = items[i];
            }
        }

        SelectableItemForController closestLeftSlot = leftSlot;
        SelectableItemForController closestRightSlot = rightSlot;
        SelectableItemForController closestDownSlot = downSlot;
        SelectableItemForController closestUpSlot = upSlot;

        for (int i = 0; i < items.Count; i++) {
            if (items[i] == outlinedItem) { continue; }

            float heightDifference = Mathf.Abs(items[i].rectTransform.position.y - outlinedItem.rectTransform.position.y);
            if (items[i].rectTransform.position.x < outlinedItem.rectTransform.position.x && items[i].rectTransform.position.x > closestLeftSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheLeft) < marginForControllerOutline) {
                closestLeftSlot = items[i];
            }
            if (items[i].rectTransform.position.x > outlinedItem.rectTransform.position.x && items[i].rectTransform.position.x < closestRightSlot.rectTransform.position.x && Mathf.Abs(heightDifference - closestRowHeightDifferenceToTheRight) < marginForControllerOutline) {
                closestRightSlot = items[i];
            }

            float widthDifference = Mathf.Abs(items[i].rectTransform.position.x - outlinedItem.rectTransform.position.x);
            if (items[i].rectTransform.position.y < outlinedItem.rectTransform.position.y && items[i].rectTransform.position.y > closestDownSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheDown) < marginForControllerOutline) {
                closestDownSlot = items[i];
            }
            if (items[i].rectTransform.position.y > outlinedItem.rectTransform.position.y && items[i].rectTransform.position.y < closestUpSlot.rectTransform.position.y && Mathf.Abs(widthDifference - closestColumnHeightDifferenceToTheUp) < marginForControllerOutline) {
                closestUpSlot = items[i];
            }
        }

        switch (direction) {
            case OutlineDirection.Left:
                return leftSlot;
            case OutlineDirection.Right:
                return rightSlot;
            case OutlineDirection.Down:
                return downSlot;
            case OutlineDirection.Up:
                return upSlot;
            case OutlineDirection.ClosestLeft:
                return closestLeftSlot;
            case OutlineDirection.ClosestRight:
                return closestRightSlot;
            case OutlineDirection.ClosestDown:
                return closestDownSlot;
            case OutlineDirection.ClosestUp:
                return closestUpSlot;
            default:
                return outlinedItem;
        }
    }

    public static SelectableItemForController GetCloseSelectableItem(List<SelectableItemForController> items, SelectableItemForController outlinedItem, OutlineDirection direction) {
        if (outlinedItem == null) {
            outlinedItem = items[0];
            return outlinedItem;
        }

        switch (direction) {

            case OutlineDirection.Left:
                SelectableItemForController closestLeftSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestLeft);
                if (outlinedItem.canJumpToTheOtherSide && closestLeftSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Right);
                } else {
                    return closestLeftSlot;
                }
            case OutlineDirection.Right:
                SelectableItemForController closestRightSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestRight);
                if (outlinedItem.canJumpToTheOtherSide && closestRightSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Left);
                } else {
                    return closestRightSlot;
                }
            case OutlineDirection.Down:
                SelectableItemForController closestDownSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestDown);
                if (outlinedItem.canJumpToTheOtherSide && closestDownSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Up);
                } else {
                    return closestDownSlot;
                }
            case OutlineDirection.Up:
                SelectableItemForController closestUpSlot = GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.ClosestUp);
                if (outlinedItem.canJumpToTheOtherSide && closestUpSlot == outlinedItem) {
                    return GetPosibleSelectableItem(items, outlinedItem, OutlineDirection.Down);
                } else {
                    return closestUpSlot;
                }
            default:
                return outlinedItem;
        }
    }

    #endregion
}

public class SelectableItemForController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

    public bool canJumpToTheOtherSide = true;
    public MenuWithSelectableItems menu;
    public RectTransform rectTransform;
    public Image imageThatChangesWithHover;
    public Sprite spriteBase;
    public Sprite spriteHovered;
    public Vector3 defaultScale = Vector3.one;
    public float hoveredScale = 1.05f;
    public bool unHoverWhenUsed = false;

    protected Coroutine c_Scaling;
    protected bool _lastHovered;

    public virtual bool Hovered => menu.hoveredItem == this;

    protected virtual void Update() {

        if (Hovered && !_lastHovered) {
            _lastHovered = true;
            imageThatChangesWithHover.sprite = spriteHovered;
            Scaling(moreScale: true);

        } else if (!Hovered && _lastHovered) {
            _lastHovered = false;
            imageThatChangesWithHover.sprite = spriteBase;
            Scaling(moreScale: false);
        }
    }

    public virtual void Use() {
        if (unHoverWhenUsed) {
            UnHover();
        }
    }

    public virtual void Hover() {
        menu.hoveredItem = this;
    }

    public virtual void UnHover() {
        menu.hoveredItem = null;
    }

    #region Interface

    public virtual void OnPointerDown(PointerEventData eventData) {
        Use();
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        Hover();
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        UnHover();
    }

    #endregion

    protected void Scaling(bool moreScale) {
        if (c_Scaling != null) {
            StopCoroutine(c_Scaling);
        }

        c_Scaling = StartCoroutine(C_Scaling(moreScale));
    }

    protected IEnumerator C_Scaling(bool moreScale) {

        if (moreScale) {

            rectTransform.localScale = defaultScale;
            while (rectTransform.localScale != defaultScale * hoveredScale) {
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, defaultScale * hoveredScale, Time.unscaledDeltaTime * 5);
                yield return null;
            }

        } else {
            while (rectTransform.localScale != defaultScale) {
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, defaultScale, Time.unscaledDeltaTime * 5);
                yield return null;
            }
        }
    }
}

public enum ControllerType : byte {
    notDefined,
    xbox,
    playstation,
    noController,
}

public enum InputState : byte { Character, Building, Interface }
public class InputManager : MonoBehaviour, ISystemActions, ICharacterActions, IBuildingActions, IInterfaceActions {
    public static InputManager Instance { get; private set; }
    public static GameControls GameControls { get; private set; }
    public static string CurrentControlScheme = "";

    public static Action<string, ControllerType> OnControlSchemeChanged;

    // System Map Events
    public static Action<InputAction.CallbackContext> OnGamePause;
    public static Action<InputAction.CallbackContext> OnBuildingToggled;
    public static Action<InputAction.CallbackContext> OnInventoryToggled;

    // Character Map Events
    public static Action<InputAction.CallbackContext> OnCharacterAbility;
    public static Action<InputAction.CallbackContext> OnCharacterAttack;
    public static Action<InputAction.CallbackContext> OnCharacterInteract;
    public static Action<InputAction.CallbackContext> OnCharacterSprint;

    // Building Map Events
    public static Action<InputAction.CallbackContext> OnBuildConfirmed;
    public static Action<InputAction.CallbackContext> OnBuildNavigation;
    public static Action<InputAction.CallbackContext> OnBuildBranchSwitched;

    // Interface Map Events
    public static Action<InputAction.CallbackContext> OnInterfaceBack;
    public static Action<InputAction.CallbackContext> OnInterfaceSwitchBranch;
    public static Action<InputAction.CallbackContext> OnInterfaceSwitchMenu;
    public static Action<InputAction.CallbackContext> OnInterfaceConfirm;
    public static Action<InputAction.CallbackContext> OnInterfaceDetails;
    public static Action<InputAction.CallbackContext> OnInterfaceInteraction;
    public static Action<InputAction.CallbackContext> OnInterfaceNavigation;

    // Hacks
    public static Action<InputAction.CallbackContext> HACK_OnChangeSkin;

    // Character Variables
    public static Vector2 Movement => GameControls.Character.Movement.ReadValue<Vector2>();
    public static Vector2 Look => GameControls.Character.View.ReadValue<Vector2>();
    public static bool IsMoving => Movement.magnitude > 0.1f;

    // Building Variables
    public static Vector2 BuildingRadialSelection => GameControls.Building.RadialSelection.ReadValue<Vector2>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameControls = new GameControls();

            GameControls.System.SetCallbacks(this);
            GameControls.Character.SetCallbacks(this);
            GameControls.Building.SetCallbacks(this);
            GameControls.Interface.SetCallbacks(this);

            GameControls.System.Enable();

            InputUser.onChange += OnInputDeviceChange;

            SetInputState(InputState.Character);
        } else {
            Destroy(gameObject);
        }
    }

    private void OnDisable() {
        InputUser.onChange -= OnInputDeviceChange;

        GameControls.Disable();
    }

    public void SetInputState(InputState newState) {
        // desactivamos todo
        GameControls.Character.Disable();
        GameControls.Building.Disable();
        GameControls.Interface.Disable();

        switch (newState) {
            case InputState.Character:
                Debug.Log("character enabled");
                GameControls.Character.Enable();
                break;
            case InputState.Building:
                // Debug.Log("building enabled");
                GameControls.Building.Enable();
                break;
            case InputState.Interface:
                GameControls.Interface.Enable();
                // Debug.Log("interface enabled");
                break;
        }
    }

    private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
        if (change == InputUserChange.ControlSchemeChanged) {
            Debug.Log($"Control Scheme Changed: {user.controlScheme.Value.name}\nController type: {Controller.GetControllerType()}");

            CurrentControlScheme = user.controlScheme.Value.name;

            OnControlSchemeChanged?.Invoke(CurrentControlScheme, Controller.GetControllerType());
        }

        if (change == InputUserChange.DevicePaired) {
        }
    }

    #region System Actions
    public void OnPause(InputAction.CallbackContext context) {
        OnGamePause?.Invoke(context);
    }
    #endregion


    #region Character Actions
    public void OnAttack(InputAction.CallbackContext context) {
        OnCharacterAttack?.Invoke(context);
    }

    public void OnChangeSkin(InputAction.CallbackContext context) {
        HACK_OnChangeSkin?.Invoke(context);
    }

    public void OnDance(InputAction.CallbackContext context) {
    }

    public void OnInteract(InputAction.CallbackContext context) {
        OnCharacterInteract?.Invoke(context);
    }

    public void OnJump(InputAction.CallbackContext context) {
    }

    public void OnMovement(InputAction.CallbackContext context) {
    }

    public void OnSpecialAbility(InputAction.CallbackContext context) {
        OnCharacterAbility?.Invoke(context);
    }

    public void OnSprint(InputAction.CallbackContext context) {
        OnCharacterSprint?.Invoke(context);
    }

    public void OnToggleInventory(InputAction.CallbackContext context) {
        OnInventoryToggled?.Invoke(context);
    }

    public void OnView(InputAction.CallbackContext context) {
    }
    #endregion

    public void OnToggleBuilder(InputAction.CallbackContext context) {
        OnBuildingToggled?.Invoke(context);
    }

    #region Building Actions
    public void OnConfirmPlacement(InputAction.CallbackContext context) {
        OnBuildConfirmed?.Invoke(context);
    }

    public void OnCycleUpgradesBranch(InputAction.CallbackContext context) {
        OnBuildBranchSwitched?.Invoke(context);
    }

    public void OnRadialSelection(InputAction.CallbackContext context) {
        OnBuildNavigation?.Invoke(context);
    }
    #endregion

    #region Interface Actions
    public void OnNavigate(InputAction.CallbackContext context) {
        OnInterfaceNavigation?.Invoke(context);
    }

    public void OnSubmit(InputAction.CallbackContext context) {
    }

    public void OnCancel(InputAction.CallbackContext context) {
    }

    public void OnPoint(InputAction.CallbackContext context) {
    }

    public void OnClick(InputAction.CallbackContext context) {
    }

    public void OnScrollWheel(InputAction.CallbackContext context) {
    }

    public void OnMiddleClick(InputAction.CallbackContext context) {
    }

    public void OnRightClick(InputAction.CallbackContext context) {
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context) {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) {
    }

    public void OnSwitchBranch(InputAction.CallbackContext context) {
        OnInterfaceSwitchBranch?.Invoke(context);
    }

    public void OnSwitchMenu(InputAction.CallbackContext context) {
        OnInterfaceSwitchMenu?.Invoke(context);
    }

    public void OnAccept(InputAction.CallbackContext context) {
        OnInterfaceInteraction?.Invoke(context);
    }

    public void OnDetails(InputAction.CallbackContext context) {
        OnInterfaceDetails?.Invoke(context);
    }

    public void OnBack(InputAction.CallbackContext context) {
        OnInterfaceBack?.Invoke(context);
    }

    public void OnConfirm(InputAction.CallbackContext context) {
        OnInterfaceConfirm?.Invoke(context);
    }
    #endregion
}

public enum MineralEnum : byte {
    Mineral1, Mineral2, Mineral3, Mineral4, Mineral5, Mineral6, Mineral7,
}

//Un arma tiene sizeUI para colocarla, tiene sell value y todo lo demas si

public enum MadnessChant : byte {
    Hunter,
    Fighter,
    Tank,
    Healer,
    Hell,
    Speedster,

}

public class Weapon : InventoryItem {
    public Tier tier;
    public float damage;
    public MadnessChant madnessChant;
}

public class InventoryItem {

    public virtual Vector2 SizeInUI => new Vector2(600, 200);
    public virtual bool Stackable => false;
    public virtual int MaxStacks => 999;
    public virtual float SellValue => 0;
}

public class MineralsDropManager : MonoBehaviour {

    public EnemyWithDrops enemyKilled;
    public ClientDrop clientDropPrefab;

    public void DropMinerals() {
        for (int i = 0; i < enemyKilled.mineralDrops.Count; i++) {
            Instantiate(clientDropPrefab);
        }
    }
}

public class EnemyWithDrops {
    public List<MineralDrop> mineralDrops = new();
}

public struct MineralDrop {
    public InventoryItem mineral;
    public int amount;
}

public class ClientDrop : MonoBehaviour {

    public float dropTime = 0.5f;
    public float reachPlayerTime = 1;
    public float distanceForPlayerToReachThisDrop = 5f;
    public bool _playerReached;
    public List<MineralDrop> mineralDrops;

    private Coroutine c_Drop;
    private bool _droping;
    private Coroutine c_GoToPlayer;
    private bool _goingToPlayer;

    private void Awake() {
        c_Drop = StartCoroutine(C_Drop());
    }

    private void OnDisable() {
        StopCoroutine(c_Drop);
        StopCoroutine(c_GoToPlayer);
    }

    private void Update() {
        if (PlayerController.instance == null) {
            return;
        }

        if ((PlayerController.instance.transform.position - transform.position).sqrMagnitude < distanceForPlayerToReachThisDrop * distanceForPlayerToReachThisDrop) {
            _playerReached = true;
        }

        if (_playerReached && !_droping && !_goingToPlayer) {
            c_GoToPlayer = StartCoroutine(C_GoToPlayer());
        }
    }

    private IEnumerator C_Drop() {
        _droping = true;

        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = transform.position + new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));

        float timer = Time.time;
        while (Time.time - timer < dropTime) {
            transform.position = Vector3.Slerp(initialPosition, finalPosition, (Time.time - timer) / dropTime);
            yield return null;
        }

        _droping = false;
    }

    private IEnumerator C_GoToPlayer() {
        _goingToPlayer = true;

        Vector3 initialPosition = transform.position;
        float timer = Time.time;
        while (Time.time - timer < reachPlayerTime) {
            transform.position = Vector3.Slerp(initialPosition, PlayerController.instance.transform.position, (Time.time - timer) / reachPlayerTime);
            yield return null;
        }

        _goingToPlayer = false;
    }
}

public enum Tier : byte {
    tier1 = 1, tier2 = 2, tier3 = 3, tier4 = 4, tier5 = 5,
}

public class RAM : InventoryItem {
    public Tier tier;
    public int ramSpace = 5;
    public override Vector2 SizeInUI => base.SizeInUI;
    public override float SellValue => 1000 * (int)tier;
}

public class RamFusionVisualizer : MonoBehaviour {
    [Header("Triangle Sides")]
    public RectTransform ramA;
    public RectTransform ramB;
    public RectTransform ramC;

    [Header("Result RAM (Center)")]
    public RectTransform resultRam;

    [Header("Settings")]
    public float thickness = 20f;
    public float baseValue = 5f;          // Minimum RAM value
    public float maxValue = 10f;          // Maximum RAM value
    public float outwardMoveMultiplier = 40f; // How much movement per extra RAM

    private float valueA;
    private float valueB;
    private float valueC;

    public void SetRamValues(float a, float b, float c) {
        valueA = Mathf.Clamp(a, baseValue, maxValue);
        valueB = Mathf.Clamp(b, baseValue, maxValue);
        valueC = Mathf.Clamp(c, baseValue, maxValue);

        Build();
    }

    void Build() {
        float result = CalculateFusion(valueA, valueB, valueC);

        // Set center width
        resultRam.sizeDelta = new Vector2(result * 20f, thickness);

        // Calculate how much bigger than minimum
        float expansionAmount = result - (baseValue + 1f);

        MoveTriangleOutward(expansionAmount);
    }

    float CalculateFusion(float a, float b, float c) {
        return ((a + b + c) / 3f) + 1f;
    }

    void MoveTriangleOutward(float expansionAmount) {
        float offset = expansionAmount * outwardMoveMultiplier;

        // Each side moves away from center along its perpendicular direction

        MoveSideOutward(ramA, offset);
        MoveSideOutward(ramB, offset);
        MoveSideOutward(ramC, offset);
    }

    void MoveSideOutward(RectTransform side, float offset) {
        // Get outward direction (perpendicular to the rectangle's right vector)
        Vector3 outward = Vector3.Cross(side.right, Vector3.forward).normalized;

        side.anchoredPosition = outward * offset;
    }
}


public class MenuWithSelectableItems : Menu {

    public List<SelectableItemForController> items;
    public List<GroupForController> groups;
    public SelectableItemForController hoveredItem;

    void OnEnable() {
        InputManager.OnInterfaceNavigation += OnArrow;
        InputManager.OnInterfaceInteraction += OnUse;
        InputManager.OnInterfaceSwitchBranch += OnChangeBranch;
    }

    void OnDisable() {
        InputManager.OnInterfaceNavigation -= OnArrow;
        InputManager.OnInterfaceInteraction -= OnUse;
        InputManager.OnInterfaceSwitchBranch -= OnChangeBranch;
    }

    public void HoverDefault() {
        items[0].Hover();
    }

    public override void UseItem() {
        bool found = false;
        for (int i = 0; i < groups.Count; i++) {
            for (int j = 0; j < groups[i].items.Count; j++) {
                if (groups[i].items[j] == hoveredItem) {
                    groups[i].lastUsedItem = hoveredItem;
                    found = true;
                    break;
                }
            }
            if (found) {
                break;
            }
        }

        if (hoveredItem != null) {

            hoveredItem.Use();
        }
    }

    public virtual void OnUse(InputAction.CallbackContext context) {
        if (context.started) {
            UseItem();
        }
    }

    public virtual void OnArrow(InputAction.CallbackContext context) {
        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.x > 0) {
            MoveOutline(OutlineDirection.Right);
        } else if (direction.x < 0) {
            MoveOutline(OutlineDirection.Left);
        } else if (direction.y > 0) {
            MoveOutline(OutlineDirection.Up);
        } else if (direction.y < 0) {
            MoveOutline(OutlineDirection.Down);
        }
    }

    public void MoveOutline(OutlineDirection direction) {

        SelectableItemForController nextSelectableItem = Controller.GetCloseSelectableItem(items, hoveredItem, direction);

        if (hoveredItem == null) {
            hoveredItem = items[0];
            return;
        }

        if (hoveredItem == nextSelectableItem) { return; }

        GroupForController selectedItemGroup = null;
        for (int i = 0; i < groups.Count; i++) {
            for (int j = 0; j < groups[i].items.Count; j++) {
                if (groups[i].items[j] == hoveredItem) {
                    selectedItemGroup = groups[i];
                    break;
                }
            }
            if (selectedItemGroup != null) {
                break;
            }
        }

        GroupForController nextSelectableItemGroup = null;
        for (int i = 0; i < groups.Count; i++) {
            for (int j = 0; j < groups[i].items.Count; j++) {
                if (groups[i].items[j] == nextSelectableItem) {
                    nextSelectableItemGroup = groups[i];
                    break;
                }
            }
            if (nextSelectableItemGroup != null) {
                break;
            }
        }

        if (selectedItemGroup != nextSelectableItemGroup) {
            nextSelectableItemGroup.HoverPreferedItem(this, nextSelectableItem);
        } else {
            nextSelectableItem.Hover(); //This will do hoveredItem = nextSelectableItem
        }
    }


    private void OnChangeBranch(InputAction.CallbackContext context) {

        float input = context.ReadValue<float>();

        if (context.started) {
            if (input > 0) {
                OnDeckMenuChangeBranchRight();
            } else if (input < 0) {
                OnDeckMenuChangeBranchLeft();
            }
        }
    }

    public void OnDeckMenuChangeBranchLeft() {
        //deckBranches[selectedBranch].OnPointerExit(null);

        //selectedBranch--;
        //if (selectedBranch < 0) {
        //    selectedBranch = 0;
        //}

        //deckBranches[selectedBranch].OnPointerEnter(null);
        //deckBranches[selectedBranch].Use();
    }

    public void OnDeckMenuChangeBranchRight() {
        //deckBranches[selectedBranch].OnPointerExit(null);

        //selectedBranch++;
        //if (selectedBranch >= deckBranches.Count) {
        //    selectedBranch = deckBranches.Count - 1;
        //}

        //deckBranches[selectedBranch].OnPointerEnter(null);
        //deckBranches[selectedBranch].Use();
    }
}

public class ControllerMovementForAllMenus : MonoBehaviour {


}
public enum OutlineDirection : byte {
    Left,
    Right,
    Down,
    Up,
    ClosestLeft,
    ClosestRight,
    ClosestDown,
    ClosestUp,
}

public class Menu : MonoBehaviour {
    // me podrían quitar el carne de programador por esto
    protected bool _isInTutorial;
    [SerializeField] private CanvasGroup _canvasGroup;

    public CanvasGroup canvasGroup => _canvasGroup;

    public bool IsInTutorial => _isInTutorial;
    public bool isOpen => _canvasGroup.alpha >= 0.99f;
    public Action<bool> OnMenuDisplayed;

    public virtual void UseItem() {
    }

    public virtual void SetTutorialState(bool state) {
        _isInTutorial = state;
    }

    public virtual void ActiveCanvasGroup(bool active) {
        _canvasGroup.alpha = active ? 1 : 0;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;

        OnMenuDisplayed?.Invoke(active);
    }

    public void FadeCanvasGroup(bool active, float time, bool timeScaled = false, Action onComplete = null) {
        //UIAnimator.Fade(canvasGroup, active, time, timeScaled, onComplete);
    }
}

public class GroupForController : MonoBehaviour {

    public List<SelectableItemForController> items;
    public SelectableItemForController lastUsedItem;
    public bool alwaysHoverLastUsedItem;
    //Quiero moverme entre items, eso bien, pero me gustaría que grupos tuvieran diferentes comportamientos, es decir, si voy al grupo de teamSlots, me gustaría que el cursor se pusiera en el que esté actualmente abierto
    //Realmente por lo demás me da igual xd.
    //Luego entre items si que necesitaría herencias. Aunque es cierto que no va a haber muchas cosas además de botones, pero solo por si acaso.

    public void HoverPreferedItem(MenuWithSelectableItems menu, SelectableItemForController posibleItem) {
        if (alwaysHoverLastUsedItem) {
            lastUsedItem.Hover();
            //menu.hoveredItem = alwaysHoverLastUsedItem ? lastUsedItem : posibleItem;
        } else {
            posibleItem.Hover();
            //menu.hoveredItem = posibleItem;
        }
    }
}

public class Ship : PlayerController {

    public List<Ammo> ammoInventory = new();
    public Ammo laserBeingUsed;

    public void AddAmmo(AmmoType type, Tier tier, int amount) {
        for (int i = 0; i < ammoInventory.Count; i++) {
            if (ammoInventory[i].type == type && ammoInventory[i].tier == tier) {
                ammoInventory[i].amount += amount;
            }
        }
    }

    public void RemoveLaserAmmo() {
        for (int i = 0; i < ammoInventory.Count; i++) {
            if (ammoInventory[i].type == laserBeingUsed.type && ammoInventory[i].tier == laserBeingUsed.tier) {
                ammoInventory[i].amount -= 1;
            }
        }
    }

    public virtual void StartSpecialHability() {
    }

    public virtual void EndSpecialHability() {
    }
}

public class Hunter : Ship {

    public override void StartSpecialHability() {
        //TODO: FUCKING RUN MORE AND INMUNE AND NO ATTACK

        Stats.HunterSpeed = 1.2f;
    }

    public override void EndSpecialHability() {

        Stats.HunterSpeed = 1f;
    }
}

public enum AmmoType {
    Laser,
    Missile,
}

[Serializable]
public class Ammo {

    public AmmoType type;
    public Tier tier;
    public int amount;

}

public class LaserBuyUI : SelectableItemForController {

    public Ammo ammo;
    public int amountToBuy = 100; //Solo 3 numeros => 100 / 1.000 / 10.000

    public override void Use() {
        base.Use();
        Ship ship = (PlayerController.instance) as Ship;
        ship.AddAmmo(ammo.type, ammo.tier, amountToBuy);
    }
}