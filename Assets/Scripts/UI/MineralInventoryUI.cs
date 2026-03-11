using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineralInventoryUI : MonoBehaviour {

    public List<Image> fills = new();

    public void Update() {
        UpdateInventoryBars();
    }

    public void UpdateInventoryBars() {

        float cumulative = 0f;

        // We go from highest tier DOWN
        for (int i = 0; i < fills.Count; i++) {
            cumulative += Ship.instanceOfClient.minerals[i];

            float fill = cumulative / Ship.instanceOfClient.maxMineralCapacity;

            fills[i].fillAmount = fill;
        }
    }
}
