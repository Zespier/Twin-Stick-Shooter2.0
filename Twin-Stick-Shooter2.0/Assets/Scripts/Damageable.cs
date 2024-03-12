using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damageable : MonoBehaviour, IDamageable {

    #region Inheritance

    public virtual void TakeDamage(Vector3 position, float damage, bool crit, string damageType) {
        FeedbackController.instance.DamageFeedBack(position, damage, crit, damageType);
    }

    #endregion
}
