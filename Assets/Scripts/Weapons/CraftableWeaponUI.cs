using System.Collections.Generic;
using UnityEngine;

public class CraftableWeaponUI : SelectableItemForController {

    public List<MineralDrop> mineralsNeeded = new();
    public Weapon weaponReward;

    public override void Use() {
        base.Use();

        Ship ship = (PlayerController.instance) as Ship;

        bool hasEveryMaterialAvailable = true;
        for (int i = 0; i < mineralsNeeded.Count; i++) {
            for (int m = 0; m < ship.minerals.Length; m++) {

                MineralDrop mineralNeeded = mineralsNeeded[i];
                if (m == (int)mineralNeeded.tier && ship.minerals[m] < mineralNeeded.amount) {
                    //If there is not enough mineral, it can't be upgraded
                    hasEveryMaterialAvailable = false;
                }
            }
        }

        if (hasEveryMaterialAvailable) {
            for (int i = 0; i < mineralsNeeded.Count; i++) {
                for (int m = 0; m < ship.minerals.Length; m++) {

                    MineralDrop mineralNeeded = mineralsNeeded[i];
                    if (m == (int)mineralNeeded.tier) {
                        ship.minerals[m] -= mineralNeeded.amount;
                    }
                }
            }

            ship.AddWeapon(weaponReward);
        }
    }
}
