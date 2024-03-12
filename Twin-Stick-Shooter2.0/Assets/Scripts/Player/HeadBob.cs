using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public bool _enable = true;

    [Range(0, 0.1f)] public float amplitude = 0.002f;
    [Range(0, 30)] public float frecuency = 16f;
    [HideInInspector] public float _currentAmplitude;
    [HideInInspector] public float _currentFrecuency;

    [Tooltip("Porcentaje de decrecimiento de headbob")] public float lessBobWhileShooting = 90f;

    [SerializeField] private Transform _camera = null;
    //[SerializeField] private Transform _cameraHolder = null;

    public float toggleSpeed = 3;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private CharacterController _controller;
    //[SerializeField] private PlayerMovement _movement;
    //Referencias al crouchController para usar la variable LerpSpeed, para controlar la velocidad del lerp en ResetPosition
    //[SerializeField] private CrouchController crouchC;
    //private RunController runC;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPos = _camera.localPosition;
        //_movement = GetComponent<PlayerMovement>();
        //runC = GetComponent<RunController>();
        //crouchC = GetComponent<CrouchController>();
    }
    private void Start()
    {
        _currentAmplitude = amplitude;
        _currentFrecuency = frecuency;
    }

    private void Update()
    {
        if (!_enable) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            _currentAmplitude *= ((100 - lessBobWhileShooting) / 100);
        }
        if (Input.GetMouseButtonUp(0))
        {
            _currentAmplitude = amplitude;
        }


        CheckMotion();
        ResetPosition();
    }

    private Vector3 FootStepMotion()
    {
        //Iniciamos la posicion a cero, ya que vamos a añadir una posicion nueva que depende del tiempo y no de donde estuvieramos
        Vector3 pos = Vector3.zero;
        //Segun el seno y coseno, usamos las frecuencias y amplitudes determinadas para conseguir un movimiento gustoso
        pos.y += Mathf.Sin(Time.time * (_currentFrecuency /*+ runC.runFrecuency*/)) * (_currentAmplitude /*+ runC.runAmplitude*/);
        pos.x += Mathf.Cos(Time.time * (_currentFrecuency /*+ runC.runFrecuency*/) / 2) * (_currentAmplitude /*+ runC.runAmplitude*/) * 2;
        return pos;
    }

    private void CheckMotion()
    {
        //float speed = new Vector3(_movement.velocityDebug.x, 0, _movement.velocityDebug.z).magnitude;

        //if (speed < toggleSpeed) { return; }
        if (!_controller.isGrounded) { return; }

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    private void ResetPosition()
    {
        if (_camera.localPosition == _startPos) { return; }
        //_camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, crouchC.lerpSpeed);
    }
}
