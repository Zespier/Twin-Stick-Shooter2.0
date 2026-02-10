using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Damageable : NetworkBehaviour, IDamageable {

    #region Inheritance

    /// <summary>
    /// Methods that is going to be overwritten, but basically is going to be used when receiving damage and will call Feedback controller for the number of damage feedback
    /// </summary>
    /// <param name="position"></param>
    /// <param name="damage"></param>
    /// <param name="crit"></param>
    /// <param name="damageType"></param>
    public virtual void TakeDamage(Vector3 position, float damage, bool crit, DamageType damageType) {
        FeedbackController.instance.DamageFeedBack(position, damage, crit, damageType);
    }

    #endregion
}
