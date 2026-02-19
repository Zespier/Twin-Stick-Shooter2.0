using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineralInventoryUI : MonoBehaviour {

    public List<Image> fills = new();

    public void UpdateInventoryBars() {

        Ship ship = (PlayerController.instance) as Ship;

        float cumulative = 0f;

        // We go from highest tier DOWN
        for (int i = 0; i < fills.Count; i++) {
            cumulative += ship.minerals[i];

            float fill = cumulative / ship.maxMineralCapacity;

            fills[i].fillAmount = fill;
        }
    }
}
