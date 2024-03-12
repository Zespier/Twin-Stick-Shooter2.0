using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour {

    public float initialMoney = 1000f;
    public float moneyPerSecond = 100f;
    public float timeBetweenWaves = 3f;
    public List<Wave> waves = new List<Wave>();
    public BoxCollider2D boxCollider2D;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Events.OnEnterSpawnArea?.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Events.OnExitSpawnArea?.Invoke();
        }
    }

}
