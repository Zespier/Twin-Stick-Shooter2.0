using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Debris : MonoBehaviour {

    public float speed = 2f;
    public float speedRange = 1f;
    public float rotationSpeed = 1f;

    private Vector3 _direction;
    private float _timer;

    public float Speed { get; set; }

    private void Awake() {
        ChooseDirection(false);
    }

    private void Update() {

        if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) > 30) {
            ChooseDirection(true);

        } else if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) > 20 && _timer >= 15) {
            _timer = 0;
            ChooseDirection(false);
        }

        Movement();
        Rotation();

        _timer += Time.deltaTime;
    }

    /// <summary>
    /// Moves the debri
    /// </summary>
    private void Movement() {
        transform.position += _direction * Time.deltaTime * Speed;
    }

    /// <summary>
    /// Rotates the debris
    /// </summary>
    private void Rotation() {

        transform.forward = Quaternion.AngleAxis(rotationSpeed, Vector3.up) * transform.forward;
    }

    /// <summary>
    /// Chooses next direction to travel out of player vision
    /// </summary>
    /// <param name="toPlayer"></param>
    private void ChooseDirection(bool toPlayer) {
        if (toPlayer) {
            _direction = PlayerController.instance.transform.position - transform.position;
        } else {

            _direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            while (_direction.magnitude < 0.2f) {
                _direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            }
        }

        _direction = _direction.normalized;
        Speed = speed + Random.Range(0, speedRange);
    }

}
