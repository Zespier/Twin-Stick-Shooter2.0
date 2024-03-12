using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public float speed = 9f;
    public Transform body;
    public PlayerInputs playerInputs;

    [HideInInspector] public Vector2 _moveValue;
    [HideInInspector] public Vector2 _moveDirectionLerped;

    public static PlayerController instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }

        playerInputs = new PlayerInputs();
    }

    private void OnEnable() {
        playerInputs.Enable();
    }

    private void OnDisable() {
        playerInputs.Disable();
    }

    private void Update() {
        Movement();
        Rotation();
    }

    /// <summary>
    /// Reads the player inputs on the left joystick to move
    /// </summary>
    private void Movement() {
        _moveValue = playerInputs.Player.Move.ReadValue<Vector2>();

        _moveDirectionLerped = Vector2.Lerp(_moveDirectionLerped, _moveValue, Time.deltaTime / 0.1f);
        transform.position += Time.deltaTime * speed * new Vector3(_moveDirectionLerped.x, _moveDirectionLerped.y, 0);
    }

    /// <summary>
    /// Reads the player inputs on the right joystick to rotate
    /// </summary>
    private void Rotation() {
        Vector2 lookValue = GetLookValue();

        body.up = Vector3.Lerp(body.up, lookValue, Time.deltaTime / 0.03f);

        Events.OnTargetMove?.Invoke(lookValue.normalized);
    }

    private Vector2 GetLookValue() {
        Vector2 lookValue = playerInputs.Player.Look.ReadValue<Vector2>();

        if (lookValue == Vector2.zero) {
            lookValue = _moveValue;
        }

        return lookValue;
    }
}
