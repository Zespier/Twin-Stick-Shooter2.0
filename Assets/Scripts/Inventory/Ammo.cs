using System;

[Serializable]
public class Ammo {

    public AmmoType type;
    public Tier tier;
    public int amount;

}

public enum AmmoType {
    Laser,
    Missile,
}