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
    public RotationType RotationType { get; set; }

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

    private void Movement() {
        transform.position += _direction * Time.deltaTime * Speed;
    }

    private void Rotation() {
        switch (RotationType) {
            case RotationType._1:
            case RotationType._2:
            case RotationType._3:
            case RotationType._4:
            case RotationType._5:
            case RotationType._6:
                transform.forward = Quaternion.AngleAxis(rotationSpeed, transform.right) * transform.forward;
                break;
                transform.forward = Quaternion.AngleAxis(rotationSpeed, transform.up) * transform.forward;
                break;
                transform.up = Quaternion.AngleAxis(rotationSpeed, transform.right) * transform.forward;
                break;
                transform.up = Quaternion.AngleAxis(rotationSpeed, transform.forward) * transform.forward;
                break;
                transform.right = Quaternion.AngleAxis(rotationSpeed, transform.up) * transform.forward;
                break;
                transform.right = Quaternion.AngleAxis(rotationSpeed, transform.forward) * transform.forward;
                break;
            default:
                break;
        }
    }

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

        RotationType = (RotationType)Random.Range(0, 7);
    }

}

public enum RotationType {
    _1, _2, _3, _4, _5, _6,
}