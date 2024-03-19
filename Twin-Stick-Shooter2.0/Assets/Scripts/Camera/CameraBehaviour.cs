using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    public Camera mainCamera;
    public Transform player;
    public PlayerController playerController;
    public float playerSpeedInfluence = 2f;

    public float movementDivision = 0.2f;
    public float distanceDivision = 0.5f;
    public float targetDivision = 0.2f;
    public float targetMultiplier = 1f;
    public float shakeDuration = 0.2f;
    public float shakeAmplitude = 0.1f;
    [Range(5, 20)] public float distance = 7f;
    public float debuggCurrentDistance;

    private Coroutine _cameraShakeAnimation;
    private Vector3 _nextTarget;
    private Vector3 _currentTarget;

    public static CameraBehaviour instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void OnEnable() {
        Events.OnTargetMove += MoveTarget;
    }

    private void OnDisable() {
        Events.OnTargetMove -= MoveTarget;
    }

    private void LateUpdate() {
        DistanceMovement();
        TargetMovement();
        CameraMovement(player.position, _currentTarget);
    }

    private void CameraMovement(Vector3 playerPosition, Vector3 target) {
        transform.position = Vector3.Lerp(transform.position, playerPosition + target, Time.deltaTime / movementDivision);
    }

    private void DistanceMovement() {
        float distanceDivision = this.distanceDivision;
        if (mainCamera.orthographicSize > GetDistance()) {
            distanceDivision *= 3f;
        }

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, GetDistance(), Time.deltaTime / distanceDivision);
        debuggCurrentDistance = mainCamera.orthographicSize;
    }

    private void TargetMovement() {
        _currentTarget = Vector2.Lerp(_currentTarget, _nextTarget, Time.deltaTime / targetDivision);
    }

    private float GetDistance() {
        float movementMagnitude = playerController._moveValue.sqrMagnitude;
        if (movementMagnitude > 1) {
            movementMagnitude = 1;
        }
        return distance - (playerSpeedInfluence / 3f) + (movementMagnitude * playerSpeedInfluence);
    }

    private void MoveTarget(Vector3 position) {
        _nextTarget = position * targetMultiplier;
    }

    public void CameraShake() {
        if (_cameraShakeAnimation != null) {
            StopCoroutine(_cameraShakeAnimation);
        }

        _cameraShakeAnimation = StartCoroutine(CameraShakeAnimation());
    }
    private IEnumerator CameraShakeAnimation() {

        float timer = shakeDuration;
        while (timer >= 0) {

            mainCamera.transform.localPosition = new Vector3(UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude), UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude), UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude));

            timer -= Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = Vector3.zero;
    }

}