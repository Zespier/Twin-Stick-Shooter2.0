using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : NetworkBehaviour {

    public Bullet bullet;
    public GameObject shotgunBullet;
    public Transform bulletParent;
    public List<Transform> shootPoints = new List<Transform>();
    public Queue<Bullet> _generatedBullets = new Queue<Bullet>();
    public Queue<ShotgunBullet> _generatedShotgunBullets = new Queue<ShotgunBullet>();
    public int _debugSize;

    private bool _shooting;
    private bool _lastFrameWasShooting;
    private float _timer;
    public PlayerController _playerController;
    private Camera _cam;
    [HideInInspector] public Rect bulletLivingArea = new Rect();

    private Vector2 _screenSize;
    private Vector3 _offset;
    private Bullet _auxBullet;
    private ShotgunBullet _auxShotgunBullet;

    private void Awake() {
        _playerController = GetComponent<PlayerController>();
        _cam = Camera.main;

        _screenSize = new Vector2(_cam.orthographicSize * 2.4f * ((float)_cam.pixelWidth / _cam.pixelHeight), _cam.orthographicSize * 2.4f);

        _offset = new Vector2(_screenSize.x / 2f, _screenSize.y / 2f);
    }

    private void Update() {
        if (!IsServer) { return; }

        if (PlayerController.instance._dead) {
            return;
        }
        _debugSize = _generatedBullets.Count;

        SetBulletLivingArea();

        if (_shooting && Time.time > _timer + 1 / _playerController.Stats.FireRate) {
            Shoot();
        }

        _lastFrameWasShooting = _shooting;
    }

    public void Shoot() {
        if (PlayerController.instance._dead) { return; }

        SetTimer();

        PrepareProjectile();

        AttackAgainIfPossible();
    }

    private void SetTimer() {
        _timer = !_lastFrameWasShooting ? Time.time : _timer + 1 / _playerController.Stats.FireRate;
    }

    private void PrepareProjectile() {

        AudioManager.instance.ShootSound();

        for (int i = 0; i < shootPoints.Count; i++) {

            if (_generatedBullets != null && _generatedBullets.Count > 0) {
                _auxBullet = _generatedBullets.Dequeue();
            }

            if (_auxBullet != null && !_auxBullet.gameObject.activeSelf) {

                _auxBullet.gameObject.SetActive(true);
                _auxBullet.transform.position = shootPoints[i].position;
                _auxBullet.Shoot(_playerController.body.forward, _playerController.Stats.DesviationAngle, _playerController.Stats);
                _generatedBullets.Enqueue(_auxBullet);

            } else {
                if (_auxBullet != null) {
                    _generatedBullets.Enqueue(_auxBullet);
                }
                Bullet newBullet = Instantiate(bullet, shootPoints[i].position, Quaternion.identity, bulletParent);
                newBullet.Shoot(_playerController.body.forward, _playerController.Stats.DesviationAngle, _playerController.Stats);
                newBullet.weaponController = this;
                _generatedBullets.Enqueue(newBullet);
            }
        }
    }

    private void AttackAgainIfPossible() {
        //It is possible to shoot so fast you need 2 bullets in one frame
        if (Time.time > _timer + 1 / _playerController.Stats.FireRate) {
            Shoot();
        }
    }

    public void ShootShotgun() {

        if (PlayerController.instance._dead) {
            return;
        }

        if (shootPoints.Count != 2) {
            return;
        }

        Vector3 shootPoint = shootPoints[0].position + (shootPoints[1].root.position - shootPoints[0].position) / 2f;

        //CameraBehaviour.instance.CameraShake();

        for (int i = 0; i < 9; i++) {

            if (_generatedShotgunBullets != null && _generatedShotgunBullets.Count > 0) {
                _auxShotgunBullet = _generatedShotgunBullets.Dequeue();
            }

            if (_auxShotgunBullet != null && !_auxShotgunBullet.gameObject.activeSelf) {

                _auxShotgunBullet.gameObject.SetActive(true);
                _auxShotgunBullet.transform.position = shootPoint;
                _auxShotgunBullet.Shoot(_playerController.body.forward, 15, _playerController.Stats);
                _generatedShotgunBullets.Enqueue(_auxShotgunBullet);

            } else {
                if (_auxShotgunBullet != null) {
                    _generatedShotgunBullets.Enqueue(_auxShotgunBullet);
                }

                ShotgunBullet newShotgunBullet = Instantiate(shotgunBullet, shootPoint, Quaternion.identity, bulletParent).GetComponent<ShotgunBullet>();
                newShotgunBullet.Shoot(_playerController.body.forward, 15, _playerController.Stats);
                newShotgunBullet.weaponController = this;
                _generatedShotgunBullets.Enqueue(newShotgunBullet);
            }
        }
    }

    private void SetBulletLivingArea() {
        bulletLivingArea = new Rect(_cam.transform.position - _offset, _screenSize);
    }

    #region InputActions

    [ServerRpc]
    public void SendShotInputServerRpc(InputAction.CallbackContext context) {
        if (context.started) {
            _shooting = true;
        } else if (context.canceled) {
            _shooting = false;
        }
    }

    public void OnShootButton(InputAction.CallbackContext context) {
        if (!IsOwner) { return; }
        SendShotInputServerRpc(context);
    }

    public void OnShotgunButton(InputAction.CallbackContext context) {
        if (context.started) {
            ShootShotgun();
        }
    }

    #endregion
}
