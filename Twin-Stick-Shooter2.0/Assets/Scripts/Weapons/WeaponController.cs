using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour {

    public GameObject bullet;
    public GameObject shotgunBullet;
    public Transform bulletParent;
    public List<Transform> shootPoints = new List<Transform>();
    public Queue<Bullet> _generatedBullets = new Queue<Bullet>();
    public Queue<ShotgunBullet> _generatedShotgunBullets = new Queue<ShotgunBullet>();
    public int _debugSize;

    private bool _shooting;
    private float _lastShoot;
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
        if (PlayerController.instance._dead) {
            return;
        }
        _debugSize = _generatedBullets.Count;

        SetBulletLivingArea();

        TryToShoot();
    }

    /// <summary>
    /// Checks if the player can shoot taking fireRate in consideration
    /// </summary>
    private void TryToShoot() {
        if (_shooting && _lastShoot + 1f / _playerController.Stats.FireRate < Time.time) {
            _lastShoot = Time.time;
            Shoot();
        }
    }

    /// <summary>
    /// Shoots bullet preferably from the pool
    /// If there is no bullets on the pool, it will instantiate another one
    /// </summary>
    public void Shoot() {
        if (PlayerController.instance._dead) {
            return;
        }

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
                Bullet newBullet = Instantiate(bullet, shootPoints[i].position, Quaternion.identity, bulletParent).GetComponent<Bullet>();
                newBullet.Shoot(_playerController.body.forward, _playerController.Stats.DesviationAngle, _playerController.Stats);
                newBullet.weaponController = this;
                _generatedBullets.Enqueue(newBullet);
            }
        }
    }

    /// <summary>
    /// Shoots the shotgun
    /// </summary>
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

    /// <summary>
    /// Calculates the living area of bullets based on the screen size
    /// </summary>
    private void SetBulletLivingArea() {
        bulletLivingArea = new Rect(_cam.transform.position - _offset, _screenSize);
    }

    #region InputActions

    /// <summary>
    /// Sets _shooting true or false depending on the state of context
    /// </summary>
    /// <param name="context"></param>
    public void OnShootButton(InputAction.CallbackContext context) {
        if (context.started) {
            _shooting = true;
        } else if (context.canceled) {
            _shooting = false;
        }
    }

    /// <summary>
    /// Shoots shotgun
    /// </summary>
    /// <param name="context"></param>
    public void OnShotgunButton(InputAction.CallbackContext context) {
        if (context.started) {
            ShootShotgun();
        }
    }

    #endregion
}
