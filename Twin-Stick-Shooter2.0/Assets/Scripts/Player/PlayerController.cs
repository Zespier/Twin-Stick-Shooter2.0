using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Damageable {

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
        transform.position += Time.deltaTime * speed * new Vector3(_moveDirectionLerped.x, 0, _moveDirectionLerped.y);
    }

    /// <summary>
    /// Reads the player inputs on the right joystick to rotate
    /// </summary>
    private void Rotation() {
        Vector2 lookValue = GetLookValue();
        Vector3 lookValue3D = new Vector3(lookValue.x, 0, lookValue.y);

        body.forward = Vector3.Lerp(body.forward, lookValue3D, Time.deltaTime / 0.03f);

        Events.OnTargetMove?.Invoke(lookValue3D.normalized);
    }

    private Vector2 GetLookValue() {
        Vector2 lookValue = playerInputs.Player.Look.ReadValue<Vector2>();

        if (lookValue == Vector2.zero) {
            lookValue = _moveValue;
        }

        return lookValue;
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.GetComponent<Collider>().TryGetComponent(out IBullet bullet)) {
            TakeDamage(transform.position, bullet.Damage * bullet.BaseDamagePercentage, Random.Range(0, 100) < 10, "energy");
            bullet.Deactivate();
        }
    }
}
