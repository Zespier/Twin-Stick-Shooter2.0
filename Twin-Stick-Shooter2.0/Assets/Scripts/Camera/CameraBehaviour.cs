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
    public float shakeSpeed = 5f;
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

    public void Update() {
        DistanceMovement();
        TargetMovement();
        CameraMovement(player.position, _currentTarget);
    }

    /// <summary>
    /// Lerped movement of the camera
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <param name="target"></param>
    private void CameraMovement(Vector3 playerPosition, Vector3 target) {
        transform.position = Vector3.Lerp(transform.position, playerPosition + target, Time.deltaTime / movementDivision);
    }

    /// <summary>
    /// Lerped movement of the camera distance from the player (vertically)
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <param name="target"></param>
    private void DistanceMovement() {
        float distanceDivision = this.distanceDivision;
        if (mainCamera.orthographicSize > GetDistance()) {
            distanceDivision *= 3f;
        }

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, GetDistance(), Time.deltaTime / distanceDivision);
        debuggCurrentDistance = mainCamera.orthographicSize;
    }

    /// <summary>
    /// Lerped movement of the camera target
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <param name="target"></param>
    private void TargetMovement() {
        _currentTarget = Vector2.Lerp(_currentTarget, _nextTarget, Time.deltaTime / targetDivision);
    }

    /// <summary>
    /// Gets the current distance from the player depending on the speed
    /// </summary>
    /// <returns></returns>
    private float GetDistance() {
        float movementMagnitude = playerController._moveValue.sqrMagnitude;
        if (movementMagnitude > 1) {
            movementMagnitude = 1;
        }
        return distance - (playerSpeedInfluence / 3f) + (movementMagnitude * playerSpeedInfluence);
    }

    /// <summary>
    /// Sets the next target value that is going to be lerped later
    /// </summary>
    /// <param name="position"></param>
    private void MoveTarget(Vector3 position) {
        _nextTarget = position * targetMultiplier;
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    public void CameraShake() {
        if (_cameraShakeAnimation != null) {
            StopCoroutine(_cameraShakeAnimation);
        }

        _cameraShakeAnimation = StartCoroutine(CameraShakeAnimation());
    }

    /// <summary>
    /// Coroutine that imitates the shake animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator CameraShakeAnimation() {

        float timer = shakeDuration;
        Vector3 nextTarget = new Vector3(UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude), mainCamera.transform.localPosition.y, UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude));
        while (timer >= 0) {

            if (Vector3.Distance(mainCamera.transform.localPosition, nextTarget) <= 0.1f) {
                nextTarget = new Vector3(UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude), mainCamera.transform.localPosition.y, UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude));
            }

            mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, nextTarget, Time.deltaTime * shakeSpeed);

            timer -= Time.deltaTime;
            yield return null;
        }

        nextTarget= new Vector3(0, mainCamera.transform.localPosition.y, 0);

        while (Vector3.Distance(mainCamera.transform.localPosition, nextTarget) > 0.1f) {
            mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, nextTarget, Time.deltaTime * shakeSpeed);
        }

        mainCamera.transform.localPosition = new Vector3(0, mainCamera.transform.localPosition.y, 0);
    }

}