using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : NetworkBehaviour {

    //public ServerBullet bullet;
    //public GameObject shotgunBullet;
    public List<Transform> shootPoints = new List<Transform>();
    //public Queue<ServerBullet> _generatedBullets = new Queue<ServerBullet>();
    //public int _debugSize;

    private bool _shooting;
    private bool _lastFrameWasShooting;
    private float _timer;
    private Camera _cam;
    //[HideInInspector] public Rect bulletLivingArea = new Rect();

    private Vector2 _screenSize;
    private Vector3 _offset;

    private void Awake() {
        _cam = Camera.main;

        _screenSize = new Vector2(_cam.orthographicSize * 2.4f * ((float)_cam.pixelWidth / _cam.pixelHeight), _cam.orthographicSize * 2.4f);

        _offset = new Vector2(_screenSize.x / 2f, _screenSize.y / 2f);
    }

    private void OnEnable() {
        InputManager.OnCharacterAttack += OnShootButton;
    }

    private void OnDisable() {
        InputManager.OnCharacterAttack -= OnShootButton;
    }

    private void Update() {
        if (NetworkManager.Singleton == null) { return; }
        if (!IsOwner) { return; }
        if (Ship.instance._dead) {
            return;
        }

        //SetBulletLivingArea();

        //TODO: I need to change this, the server should be the one to calculate all these things form all the players right? I should only read the inputs on the clients, not decide wheter they can shoot or not, that is hackeable
        //TODO: Also change this to 120 fps for inputs
        if (_shooting && Time.time > _timer + 1 / Ship.instance.Stats.FireRate) {
            Shoot();
        }

        _lastFrameWasShooting = _shooting;
    }

    public void Shoot() {
        if (Ship.instance._dead) { return; }

        Ship ship = (Ship.instance) as Ship;
        if (ship != null) {

            if (ship.RemoveLaserAmmo()) {
                SetTimer();

                PrepareProjectile();

                AttackAgainIfPossible();
            }

        } else {
            Debug.LogError("Something happened casting as Ship");
        }
    }

    private void SetTimer() {
        _timer = !_lastFrameWasShooting ? Time.time : _timer + 1 / Ship.instance.Stats.FireRate;
    }

    private void PrepareProjectile() {

        AudioManager.instance.ShootSound();

        for (int i = 0; i < shootPoints.Count; i++) {

            float speed = Random.Range(28 - 0.5f, 28 + 0.5f);
            Vector3 direction = BulletFireDesviation.RandomBulletFireDesviation(shootPoints[i], Ship.instance.Stats.DesviationAngle);

            BulletContainer.instance.CreateBullet(shootPoints[i].position, direction, speed, false, (byte)Ship.instance.laserTierBeingUsed);

            //if (_generatedBullets != null && _generatedBullets.Count > 0) {
            //    _auxBullet = _generatedBullets.Dequeue();
            //}

            //if (_auxBullet != null && !_auxBullet.gameObject.activeSelf) {

            //    SendUsePoolBulletServerRpc(i);

            //} else {
            //    if (_auxBullet != null) {
            //        _generatedBullets.Enqueue(_auxBullet);
            //    }
            //    SendSpawnBulletServerRpc(i);
            //}
        }
    }

    //public void SendSpawnBullet(int index) {

    //    ServerBullet newBullet = Instantiate(bullet, shootPoints[index].position, Quaternion.identity, BulletContainer.instance.transform);
    //    newBullet.Shoot(PlayerController.instance.body.forward, PlayerController.instance.Stats.DesviationAngle, PlayerController.instance.Stats, spawnNetworkObject: true);
    //    _generatedBullets.Enqueue(newBullet);
    //}

    //public void SendUsePoolBullet(int index) {

    //    _auxBullet.gameObject.SetActive(true);
    //    _auxBullet.transform.position = shootPoints[index].position;
    //    _auxBullet.Shoot(PlayerController.instance.body.forward, PlayerController.instance.Stats.DesviationAngle, PlayerController.instance.Stats);
    //    _generatedBullets.Enqueue(_auxBullet);
    //}

    private void AttackAgainIfPossible() {
        //It is possible to shoot so fast you need 2 bullets in one frame
        if (Time.time > _timer + 1 / Ship.instance.Stats.FireRate) {
            Shoot();
        }
    }

    //private void SetBulletLivingArea() {
    //    bulletLivingArea = new Rect(_cam.transform.position - _offset, _screenSize);
    //}

    #region InputActions

    //public void SendShotInputServerRpc(InputAction.CallbackContext context) {
    //    if (context.started) {
    //        _shooting = true;
    //    } else if (context.canceled) {
    //        _shooting = false;
    //    }
    //}

    public void OnShootButton(InputAction.CallbackContext context) {
        if (context.started) {
            _shooting = true;
        } else if (context.canceled) {
            _shooting = false;
        }
        //SendShotInputServerRpc(context);
    }
    #endregion
}
