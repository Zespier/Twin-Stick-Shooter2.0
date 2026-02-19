using static GameControls;
using System;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem;
using UnityEngine;

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
        if (Instance == this) {
            InputUser.onChange -= OnInputDeviceChange;
            GameControls.Disable();
        }
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
