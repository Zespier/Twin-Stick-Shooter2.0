using System;
using UnityEngine;

public class MineralsDropManager : MonoBehaviour {

    public ClientDrop clientDropPrefab;

    public static MineralsDropManager instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    public void DropMinerals(Enemy enemy) {
        for (int i = 0; i < enemy.mineralDrops.Count; i++) {
            ClientDrop clientDrop = Instantiate(clientDropPrefab, enemy.transform.position, Quaternion.identity);
            clientDrop.mineralDrops = enemy.mineralDrops;
        }
    }
}

[Serializable]
public struct MineralDrop {
    public MineralTier tier;
    public int amount;
}
