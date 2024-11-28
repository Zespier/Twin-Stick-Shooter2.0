using UnityEngine;

public interface IDamageable {
    public void TakeDamage(Vector3 position, float damage, bool crit, DamageType damageType);
}

//Don't think I need to summary an interface =_=