using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : PlayerController {

    public int maxMineralCapacity = 4000;
    public int[] minerals = new int[7];
    public Ammo[] lasserAmmo = new Ammo[5];
    public int laserBeingUsed;
    public Ammo tier1Laser;
    public List<Weapon> weaponInventory;

    public void AddMineral(int mineralTier, int amount) {
        int currentTotal = CurrentMineralTotal();
        int availableSpace = maxMineralCapacity - currentTotal;

        if (availableSpace >= amount) {
            minerals[mineralTier] += amount;

            return;
        }

        int spaceToFree = amount - availableSpace;

        int spaceFreed = FreeSpace(spaceToFree, mineralTier);

        //If it couldn't free space is because the max tier you are collecting is already the entirety of your mineral Inventory. It means you are stacked and can't take more of any mineral, unless you find a highter tier material.
        if (spaceFreed != 0) {
            minerals[mineralTier] += amount;
        }
    }

    private int FreeSpace(int spaceToFree, int incomingTier) {
        int spaceToFreeAtTheStart = spaceToFree;
        for (int tier = 0; tier < incomingTier; tier++) {

            if (spaceToFree <= 0) { return spaceToFreeAtTheStart - spaceToFree; }

            int availableInTier = minerals[tier];

            if (availableInTier <= 0) { continue; }

            int removeAmount = spaceToFree;
            if (availableInTier < spaceToFree) {
                removeAmount = availableInTier;
            }

            minerals[tier] -= removeAmount;
            spaceToFree -= removeAmount;
        }

        return spaceToFreeAtTheStart - spaceToFree;
    }

    public int CurrentMineralTotal() {
        int total = 0;
        for (int i = 0; i < minerals.Length; i++) {
            total += minerals[i];
        }

        return total;
    }

    public void AddLaserAmmo(Tier tier, int amount) {
        lasserAmmo[(int)tier - 1].amount += amount; //Tiers index go from 1 to 5, not 0 to 4
    }

    public bool RemoveLaserAmmo() {

        if (laserBeingUsed == (int)Tier.tier1) { return true; } //There is infinite tier 1 ammo

        Ammo _currentAmmo = lasserAmmo[laserBeingUsed - 1];

        if (_currentAmmo.amount > 0) {
            _currentAmmo.amount -= 1; //TODO: Anything else needed when removing laser ammo?
            return true;

        } else if (laserBeingUsed == (int)Tier.tier2) { //return to the infinite tier1 ammo
            laserBeingUsed = 1;
            return true;

        } else if (laserBeingUsed >= (int)Tier.tier3) {
            //Down one tier of lasers

            do {
                laserBeingUsed--;
                _currentAmmo = lasserAmmo[laserBeingUsed - 1];

            } while (_currentAmmo.amount <= 0 && laserBeingUsed >= 2);

            //If there is actually any kind of special ammo available, use it
            if (_currentAmmo.amount > 0 && laserBeingUsed >= 2) {
                _currentAmmo.amount -= 1; //TODO: Anything else needed when removing laser ammo?
                return true;
            }
        }

        return false;
    }

    public void AddWeapon(Weapon weapon) {
        weaponInventory.Add(weapon);
    }

    public virtual void StartSpecialHability(InputAction.CallbackContext context) {
    }

    public virtual void EndSpecialHability(InputAction.CallbackContext context) {
    }
}