using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Damageable {

    /// <summary>
    /// If a bullet impacts, destroy it
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision) {
        if (collision.GetComponent<Collider>().TryGetComponent(out IBullet bullet)) {
            FeedbackController.instance.Particles(ParticleType.smallExplosion, collision.transform.position, Vector3.forward);
            bullet.Deactivate();
        }
    }
}
