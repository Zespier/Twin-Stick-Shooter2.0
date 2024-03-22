using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Debris : MonoBehaviour {

    public float speed = 2f;
    public float speedRange = 1f;

    private Vector3 _direction;
    private float _timer;

    public float Speed { get; set; }

    private void Update() {

        if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) > 40) {
            ChooseDirection(true);

        } else if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) > 20 && _timer >= 4) {
            _timer = 0;
            ChooseDirection(false);
        }

        Movement();

        _timer += Time.deltaTime;
    }

    private void Movement() {
        transform.position += _direction * Time.deltaTime * Speed;
    }

    private void ChooseDirection(bool toPlayer) {
        _direction = PlayerController.instance.transform.position - transform.position;
        Speed = speed + Random.Range(-speedRange, speedRange);
    }

}
