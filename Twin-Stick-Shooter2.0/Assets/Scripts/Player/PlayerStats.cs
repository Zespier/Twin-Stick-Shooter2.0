using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStats : MonoBehaviour {

    public List<float> bulletDamageBuffs = new List<float>();
    public float totalBulletDamageBuff;
    //ESTABA HACIENDO QUE EL BUFO TOTAL SE SUME CUANDO SE AÑADE UNO PARA NO HACER CALCULOS ETERNOS, Y QUE ESE VALOR LO COJA EL BULLET

    public List<float> baseDamages;
    public List<float> damagePercentages;
    public List<float> flatDamages;

    public static PlayerStats instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }
        baseAtk = 100;
    }


    [HideInInspector] public float baseAtk;
    public float Atk { get => (baseAtk + BaseDamages) * (DamagePercentages / 100f) + FlatDamages; }
    public float BaseDamages { get => GetAllBuffs("BaseDamages"); }
    public float DamagePercentages { get => GetAllBuffs("DamagePercentages"); }
    public float FlatDamages { get => GetAllBuffs("FlatDamages"); }

    /// <summary>
    /// Gets all the buffs
    /// </summary>
    /// <param name="propName"></param>
    /// <returns></returns>
    public float GetAllBuffs(string propName) {
        switch (propName) {

            case "BaseDamages":
                return baseDamages.Sum();

            case "DamagePercentages":
                float result = damagePercentages.Sum();
                if (result == 0) { result = 1; }
                return result;

            case "FlatDamages":
                return flatDamages.Sum();

            default:
                return 1;
        }
    }

    /// <summary>
    /// Ads a buff
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public void AddBuff(UpgradeType type, float amount) {
        switch (type) {
            case UpgradeType.baseDamage:
                baseDamages.Add(amount);
                break;
            case UpgradeType.damagePercentage:
                damagePercentages.Add(amount);
                break;
            case UpgradeType.flatDamage:
                flatDamages.Add(amount);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// removes a buff
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public void RemoveBuff(UpgradeType type, float amount) {
        switch (type) {
            case UpgradeType.baseDamage:
                baseDamages.Remove(amount);
                break;
            case UpgradeType.damagePercentage:
                damagePercentages.Remove(amount);
                break;
            case UpgradeType.flatDamage:
                flatDamages.Remove(amount);
                break;
            default:
                break;
        }
    }


    /* Possible upgrades
     * 
     * 
     * 10% of not shooting a bullet
     * 100% damage increase
     * 
     * +40-400% bullet dispersion and fireRate
     * 
     * 5% damage reduction
     * 
     * 1 damage reduction
     * 
     * 10% health
     * 
     * 1% hp recovery per second
     * 
     * enemy receives 50% of the reduced damage
     * 
     * recovers 0.5% of damage dealt
     * 
     * 1 base damage
     * 
     * 10% damage
     * 
     * Increases 1-100% damage based on currentHeat
     * 
     * Cooling down reduces 10% health, but restores 10% health per enemy killed
     * 
     * +5 shotgun projectiles
     * 
     * 
     * SHIPS:
     * 
     * -100% crit
     * Transforms excessive crit into x4 crit damage
     * 
     * Starts with 50% reduced damage
     * Can't vamp damage
     * 
     * 40% speed and damage
     */
}
