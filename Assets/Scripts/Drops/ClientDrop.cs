using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientDrop : MonoBehaviour {

    public float dropTime = 0.5f;
    public float reachPlayerTime = 1;
    public float distanceForPlayerToReachThisDrop = 5f;
    public bool _playerReached;
    public List<MineralDrop> mineralDrops;

    private Coroutine c_Drop;
    private bool _droping;
    private Coroutine c_GoToPlayer;
    private bool _goingToPlayer;

    private void Awake() {
        c_Drop = StartCoroutine(C_Drop());
    }

    private void OnDisable() {
        StopCoroutine(c_Drop);
        StopCoroutine(c_GoToPlayer);
    }

    private void Update() {
        if (PlayerController.instance == null) {
            return;
        }

        if ((PlayerController.instance.transform.position - transform.position).sqrMagnitude < distanceForPlayerToReachThisDrop * distanceForPlayerToReachThisDrop) {
            _playerReached = true;
        }

        if (_playerReached && !_droping && !_goingToPlayer) {
            c_GoToPlayer = StartCoroutine(C_GoToPlayer());
        }
    }

    private IEnumerator C_Drop() {
        _droping = true;

        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = transform.position + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));

        float timer = Time.time;
        while (Time.time - timer < dropTime) {
            transform.position = Vector3.Slerp(initialPosition, finalPosition, (Time.time - timer) / dropTime);
            yield return null;
        }

        _droping = false;
    }

    private IEnumerator C_GoToPlayer() {
        _goingToPlayer = true;

        Vector3 initialPosition = transform.position;
        float timer = Time.time;
        while (Time.time - timer < reachPlayerTime) {
            transform.position = Vector3.Slerp(initialPosition, PlayerController.instance.transform.position, (Time.time - timer) / reachPlayerTime);
            yield return null;
        }

        Ship ship = (PlayerController.instance) as Ship;
        for (int i = 0; i < mineralDrops.Count; i++) {
            ship.AddMineral((int)mineralDrops[i].tier, mineralDrops[i].amount);
        }
        PlayerController.instance.mineralInventoryUI.UpdateInventoryBars();
        Destroy(gameObject);

        _goingToPlayer = false;
    }
}
