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

    [HideInInspector] public float baseHp;
    public float coreHp;
    public float _increasedHp;
    public float hpPerCoreLevel;
    public float AtkHp { get { return baseHp + coreLevel[0] * hpPerCoreLevel; } }
    public float DefHp { get { return baseHp + coreLevel[1] * hpPerCoreLevel; } }
    public float SurvivalHp { get { return (baseHp + (coreLevel[2] * hpPerCoreLevel) + coreHp) * _increasedHp; } }

    [HideInInspector] public float baseAtk;
    public float coreAtk;
    public float atkPerCoreLevel;
    public float infusionAtk;
    public float Atk { get => (baseAtk + BaseDamages) * (DamagePercentages / 100f) + FlatDamages; }
    public float BaseDamages { get => GetAllBuffs("BaseDamages"); }
    public float DamagePercentages { get => GetAllBuffs("DamagePercentages"); }
    public float FlatDamages { get => GetAllBuffs("FlatDamages"); }

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



    [HideInInspector] public float baseDef;
    public float coreDef;
    public float _increasedDef;
    public float defPerCoreLevel;
    public float AtkDef { get { return baseDef + coreLevel[0] * defPerCoreLevel; } }
    public float DefDef { get { return (baseDef + (coreLevel[1] * defPerCoreLevel) + coreDef) * _increasedDef; } }
    public float SurvivalDef { get { return baseDef + coreLevel[2] * defPerCoreLevel; } }

    [HideInInspector] public float baseCrit;
    public float coreCrit;
    public float Crit { get { return baseCrit + coreCrit; } }

    [HideInInspector] public float baseCritDamage;
    public float coreCritDamage;
    public float CritDamage { get { return baseCritDamage + coreCritDamage; } }

    public int[] coreLevel = new int[3];

    public PlayerStats(float hp, float atk, float def, float crit, float critDamage) {
        baseHp = hp;
        baseAtk = atk;
        baseDef = def;
        baseCrit = crit;
        baseCritDamage = critDamage;

        _increasedDef = 1;
        _increasedHp = 1;
        hpPerCoreLevel = 10f;
        defPerCoreLevel = 1f;
        atkPerCoreLevel = 0.5f;
    }

    /// <summary>
    /// True if hits crit, and returns the multiplied damage.
    /// False if doesn't hit crit, and return normal damage.
    /// </summary>
    /// <returns></returns>
    public (bool, float) CritAttack() {
        (bool, float) result;

        float random = Random.Range(0, 100f);
        result.Item1 = random < Crit ? true : false;
        result.Item2 = random < Crit ? Atk * (1 + CritDamage / 100f) : Atk;

        return result;
    }
    /// <summary>
    /// True if hits crit, and returns the multiplied damage.
    /// False if doesn't hit crit, and return normal damage.
    /// </summary>
    /// <returns></returns>
    public (bool, float) CritHp() {
        (bool, float) result;

        float random = Random.Range(0, 100f);
        result.Item1 = random < Crit ? true : false;
        result.Item2 = random < Crit ? SurvivalHp * (1 + CritDamage / 100f) : SurvivalHp;

        return result;
    }
    /// <summary>
    /// True if hits crit, and returns the multiplied damage.
    /// False if doesn't hit crit, and return normal damage.
    /// </summary>
    /// <returns></returns>
    public (bool, float) CritDef() {
        (bool, float) result;

        float random = Random.Range(0, 100f);
        result.Item1 = random < Crit ? true : false;
        result.Item2 = random < Crit ? DefDef * (1 + CritDamage / 100f) : DefDef;

        return result;
    }

    private List<(Coroutine, Enemy)> buffsDef = new List<(Coroutine, Enemy)>();
    public void UpdateIncreaseDef(float percentage, float percentagePerStats, List<(Coroutine, Enemy)> buffsDef) {
        this.buffsDef = buffsDef; //Safe the reference to know how many buffs we have right now.

        float multiplier = percentage / 100f;
        float multiplierPerStat = percentagePerStats / 100f;
        _increasedDef = 1 + ((multiplier + (Atk * multiplierPerStat)) * this.buffsDef.Count);
        //The Def increases proportional to the Atk of the Attack configuration
    }

    private List<(Coroutine, Enemy)> buffsHp = new List<(Coroutine, Enemy)>();
    public void UpdateIncreaseHp(float percentage, float percentagePerStats, List<(Coroutine, Enemy)> buffsHp) {
        this.buffsHp = buffsHp; //Safe the reference to know how many buffs we have right now.

        float multiplier = percentage / 100f;
        float multiplierPerStat = percentagePerStats / 100f;
        _increasedHp = 1 + ((multiplier + (DefDef * multiplierPerStat)) * this.buffsHp.Count);
        //The Hp increases proportional to the Defense of the defense configuration
    }

}
