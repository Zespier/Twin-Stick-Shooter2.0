using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : NetworkBehaviour {

    public Bullet bullet;
    public GameObject shotgunBullet;
    public List<Transform> shootPoints = new List<Transform>();
    public Queue<Bullet> _generatedBullets = new Queue<Bullet>();
    public Queue<ShotgunBullet> _generatedShotgunBullets = new Queue<ShotgunBullet>();
    public int _debugSize;

    private bool _shooting;
    private bool _lastFrameWasShooting;
    private float _timer;
    private Camera _cam;
    [HideInInspector] public Rect bulletLivingArea = new Rect();

    private Vector2 _screenSize;
    private Vector3 _offset;
    private Bullet _auxBullet;
    private ShotgunBullet _auxShotgunBullet;

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
        if (PlayerController.instance._dead) {
            return;
        }

        _debugSize = _generatedBullets.Count;

        SetBulletLivingArea();

        //TODO: I need to change this, the server should be the one to calculate all these things form all the players right? I should only read the inputs on the clients, not decide wheter they can shoot or not, that is hackeable
        if (_shooting && Time.time > _timer + 1 / PlayerController.instance.Stats.FireRate) {
            Shoot();
        }

        _lastFrameWasShooting = _shooting;
    }

    public void Shoot() {
        if (PlayerController.instance._dead) { return; }

        Ship ship = (PlayerController.instance) as Ship;
        ship.RemoveLaserAmmo();

        SetTimer();

        PrepareProjectile();

        AttackAgainIfPossible();
    }

    private void SetTimer() {

        _timer = !_lastFrameWasShooting ? Time.time : _timer + 1 / PlayerController.instance.Stats.FireRate;

    }

    private void PrepareProjectile() {

        AudioManager.instance.ShootSound();

        for (int i = 0; i < shootPoints.Count; i++) {

            if (_generatedBullets != null && _generatedBullets.Count > 0) {
                _auxBullet = _generatedBullets.Dequeue();
            }

            if (_auxBullet != null && !_auxBullet.gameObject.activeSelf) {

                BytesUsedCounter.AddBytesUsed(4); //bytes used by int
                SendUsePoolBulletServerRpc(i);

            } else {
                if (_auxBullet != null) {
                    _generatedBullets.Enqueue(_auxBullet);
                }
                BytesUsedCounter.AddBytesUsed(4); //bytes used by int
                SendSpawnBulletServerRpc(i);
            }
        }
    }

    [ServerRpc]
    public void SendSpawnBulletServerRpc(int index) {

        Bullet newBullet = Instantiate(bullet, shootPoints[index].position, Quaternion.identity, BulletContainer.instance.transform);
        newBullet.Shoot(PlayerController.instance.body.forward, PlayerController.instance.Stats.DesviationAngle, PlayerController.instance.Stats, spawnNetworkObject: true);
        _generatedBullets.Enqueue(newBullet);
    }

    [ServerRpc]
    public void SendUsePoolBulletServerRpc(int index) {

        _auxBullet.gameObject.SetActive(true);
        _auxBullet.transform.position = shootPoints[index].position;
        _auxBullet.Shoot(PlayerController.instance.body.forward, PlayerController.instance.Stats.DesviationAngle, PlayerController.instance.Stats);
        _generatedBullets.Enqueue(_auxBullet);
    }

    private void AttackAgainIfPossible() {
        //It is possible to shoot so fast you need 2 bullets in one frame
        if (Time.time > _timer + 1 / PlayerController.instance.Stats.FireRate) {
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
                _auxShotgunBullet.Shoot(PlayerController.instance.body.forward, 15, PlayerController.instance.Stats);
                _generatedShotgunBullets.Enqueue(_auxShotgunBullet);

            } else {
                if (_auxShotgunBullet != null) {
                    _generatedShotgunBullets.Enqueue(_auxShotgunBullet);
                }

                ShotgunBullet newShotgunBullet = Instantiate(shotgunBullet, shootPoint, Quaternion.identity, BulletContainer.instance.transform).GetComponent<ShotgunBullet>();
                newShotgunBullet.Shoot(PlayerController.instance.body.forward, 15, PlayerController.instance.Stats, true);
                _generatedShotgunBullets.Enqueue(newShotgunBullet);
            }
        }
    }

    private void SetBulletLivingArea() {
        bulletLivingArea = new Rect(_cam.transform.position - _offset, _screenSize);
    }

    #region InputActions

    //public void SendShotInputServerRpc(InputAction.CallbackContext context) {
    //    if (context.started) {
    //        _shooting = true;
    //    } else if (context.canceled) {
    //        _shooting = false;
    //    }
    //}

    public void OnShootButton(InputAction.CallbackContext context) {
        Debug.Log("Se llama al método");
        if (context.started) {
            Debug.Log("Started");
            _shooting = true;
        } else if (context.canceled) {
            Debug.Log("Canceled");
            _shooting = false;
        }
        //SendShotInputServerRpc(context);
    }

    public void OnShotgunButton(InputAction.CallbackContext context) {
        if (context.started) {
            ShootShotgun();
        }
    }

    #endregion
}
