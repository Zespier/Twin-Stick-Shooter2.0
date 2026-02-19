using UnityEngine;

public class InventoryItem {

    public int currentStacks;

    public virtual Vector2 SizeInUI => new Vector2(600, 200);
    public virtual bool Stackable => false;
    public virtual int MaxStacks => 999;
    public virtual float SellValue => 0;

    public bool AddStacks(int amount) {
        if (currentStacks >= MaxStacks) {
            return false;
        }

        if (currentStacks + amount > MaxStacks) {
            currentStacks = MaxStacks;
            return false;
        } else {
            currentStacks += amount;
            return true;
        }
    }

    public void RemoveStacks(int amount) {
        currentStacks = Mathf.Clamp(currentStacks - amount, 0, MaxStacks);
    }
}

public enum Tier : byte {
    tier1 = 1, tier2 = 2, tier3 = 3, tier4 = 4, tier5 = 5,
}
