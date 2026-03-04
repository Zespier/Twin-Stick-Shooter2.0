using System;
using System.Drawing;
using UnityEngine;

[Serializable]
public class Weapon : InventoryItem {
    public Tier tier;
    public float damage;
    public byte sizee = 1;
    public MadnessChant madnessChant;
    public bool equipped;
    public string shipEquipped;
    public byte initialIndex;

    public byte EndIndex => (byte)(initialIndex + sizee - 1);
}