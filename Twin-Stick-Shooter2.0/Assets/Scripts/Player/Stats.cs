using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stats : MonoBehaviour {

    public UpgradeHolder upgradeHolder;

    public List<float> baseDamages;
    public List<float> damagePercentages;
    public List<float> flatDamages;

    public List<float> baseHps;
    public List<float> hpPercentages;
    public List<float> flatHps;

    public List<float> baseSpeeds;
    public List<float> speedPercentages;
    public List<float> flatSpeeds;

    public float baseAtk = 10;
    public virtual float Atk => (baseAtk + BaseDamages) * (DamagePercentages / 100f) + FlatDamages;
    public virtual float BaseDamages => GetAllBuffs(Buff.BaseDamage);
    public virtual float DamagePercentages => GetAllBuffs(Buff.DamagePercentage);
    public virtual float FlatDamages => GetAllBuffs(Buff.FlatDamage);

    public float baseHP = 100;
    public virtual float HP => (baseHP + BaseHps) * (HpPercentages / 100f) + FlatHps;
    public virtual float BaseHps => GetAllBuffs(Buff.BaseHp);
    public virtual float HpPercentages => GetAllBuffs(Buff.HpPercentage);
    public virtual float FlatHps => GetAllBuffs(Buff.FlatHp);

    //No defense, too hard to manage

    public float baseSpeed = 2;
    public virtual float Speed => (baseSpeed + BaseSpeeds) * (SpeedPercentages / 100f) + FlatSpeeds;
    public virtual float BaseSpeeds => GetAllBuffs(Buff.BaseSpeed);
    public virtual float SpeedPercentages => GetAllBuffs(Buff.SpeedPercentage);
    public virtual float FlatSpeeds => GetAllBuffs(Buff.FlatSpeed);

    private List<(Buff buff, float amount, Coroutine coroutine)> buffCoroutines = new List<(Buff buff, float amount, Coroutine coroutine)>();

    #region Upgrade Management

    /// <summary>
    /// Asks the upgrade holder to manage his upgrades
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="amount"></param>
    public virtual void ManageAddUpgrade(Buff buffType, float amount) {
        if (upgradeHolder == null) {
            Debug.LogError("No upgrade holder assigned");
            return;
        }

        upgradeHolder.AddUpgrade(buffType, amount, this);
    }
    public virtual void ManageAddUpgrade(Buff buff, float amount, float time) {
        ManageAddUpgrade(buff, amount);
        buffCoroutines.Add((buff, amount, StartCoroutine(C_WaitToRemoveUpgrade(buff, amount, time))));
    }

    /// <summary>
    /// Asks the upgrade holder to manage his upgrades
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="amount"></param>
    public virtual void ManageRemoveUpgrade(Buff buffType, float amount) {
        if (upgradeHolder == null) {
            Debug.LogError("No upgrade holder assigned");
            return;
        }

        upgradeHolder.RemoveUpgrade(buffType, amount);
    }

    #endregion

    #region Buffs

    /// <summary>
    /// Gets all the buffs
    /// </summary>
    /// <param name="propName"></param>
    /// <returns></returns>
    public virtual float GetAllBuffs(Buff buffType) {

        switch (buffType) {

            case Buff.BaseDamage:
                if (baseDamages == null || baseDamages.Count <= 0) {
                    return 0;
                }
                return baseDamages.Sum();

            case Buff.DamagePercentage:
                if (damagePercentages == null || damagePercentages.Count <= 0) {
                    return 100f; //Is in percentage
                }
                return damagePercentages.Sum() + 100f;

            case Buff.FlatDamage:
                if (flatDamages == null || flatDamages.Count <= 0) {
                    return 0;
                }
                return flatDamages.Sum();

            case Buff.BaseHp:
                if (baseHps == null || baseHps.Count <= 0) {
                    return 0;
                }
                return baseHps.Sum();

            case Buff.HpPercentage:
                if (hpPercentages == null || hpPercentages.Count <= 0) {
                    return 100;
                }
                return hpPercentages.Sum() + 100f;

            case Buff.FlatHp:
                if (flatHps == null || flatHps.Count <= 0) {
                    return 0;
                }
                return flatHps.Sum();

            case Buff.BaseSpeed:
                if (baseSpeeds == null || baseSpeeds.Count <= 0) {
                    return 0;
                }
                return baseSpeeds.Sum();

            case Buff.SpeedPercentage:
                if (speedPercentages == null || speedPercentages.Count <= 0) {
                    return 100;
                }
                return speedPercentages.Sum() + 100f;

            case Buff.FlatSpeed:
                if (flatSpeeds == null || flatSpeeds.Count <= 0) {
                    return 0;
                }
                return flatSpeeds.Sum();

            default:
                Debug.LogError("Buff not defined");
                return 0;

        }

    }

    /// <summary>
    /// Ads a buff
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="amount"></param>
    public virtual void AddBuff(Buff buffType, float amount) {

        switch (buffType) {

            case Buff.BaseDamage:
                baseDamages.Add(amount);
                break;

            case Buff.DamagePercentage:
                damagePercentages.Add(amount);
                break;

            case Buff.FlatDamage:
                flatDamages.Add(amount);
                break;

            case Buff.BaseHp:
                baseHps.Add(amount);
                break;

            case Buff.HpPercentage:
                hpPercentages.Add(amount);
                break;

            case Buff.FlatHp:
                flatHps.Add(amount);
                break;

            case Buff.BaseSpeed:
                baseSpeeds.Add(amount);
                break;

            case Buff.SpeedPercentage:
                speedPercentages.Add(amount);
                break;

            case Buff.FlatSpeed:
                flatSpeeds.Add(amount);
                break;

            default:
                Debug.LogError("Buff not defined");
                break;
        }
    }

    /// <summary>
    /// removes a buff
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="amount"></param>
    public virtual void RemoveBuff(Buff buffType, float amount) {

        switch (buffType) {

            case Buff.BaseDamage:
                baseDamages.Remove(amount);
                break;

            case Buff.DamagePercentage:
                damagePercentages.Remove(amount);
                break;

            case Buff.FlatDamage:
                flatDamages.Remove(amount);
                break;

            case Buff.BaseHp:
                baseHps.Remove(amount);
                break;

            case Buff.HpPercentage:
                hpPercentages.Remove(amount);
                break;

            case Buff.FlatHp:
                flatHps.Remove(amount);
                break;

            case Buff.BaseSpeed:
                baseSpeeds.Remove(amount);
                break;

            case Buff.SpeedPercentage:
                speedPercentages.Remove(amount);
                break;

            case Buff.FlatSpeed:
                flatSpeeds.Remove(amount);
                break;

            default:
                Debug.LogError("Buff not defined");
                break;
        }
    }

    #endregion

    #region Timers

    private IEnumerator C_WaitToRemoveUpgrade(Buff buff, float amount, float time) {

        float timer = time;

        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }

        ManageRemoveUpgrade(buff, amount);
    }

    public void RemoveAllTemporaryBuffs() {
        for (int i = 0; i < buffCoroutines.Count; i++) {
            StopCoroutine(buffCoroutines[i].coroutine);
            ManageRemoveUpgrade(buffCoroutines[i].buff, buffCoroutines[i].amount);
        }
    }

    public void RemoveAllBuffs() {
        for (int i = 0; i < upgradeHolder.upgrades.Count; i++) {
            upgradeHolder.RemoveUpgrade(upgradeHolder.upgrades[i].upgradeType, upgradeHolder.upgrades[i].amount);
        }
    }

    #endregion
}

public enum Buff {
    BaseDamage,
    DamagePercentage,
    FlatDamage,

    BaseHp,
    HpPercentage,
    FlatHp,

    BaseSpeed,
    SpeedPercentage,
    FlatSpeed,
}