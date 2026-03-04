using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Damageable : NetworkBehaviour, IDamageable {

    #region Inheritance

    public virtual void TakeDamage(Vector3 position, float damage, bool crit, DamageType damageType) {
        FeedbackController.instance.DamageFeedBack(position, damage, crit, damageType);
    }

    #endregion
}
