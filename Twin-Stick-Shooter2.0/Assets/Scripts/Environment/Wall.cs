using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Damageable {

    private void OnTriggerEnter(Collider collision) {
        if (collision.GetComponent<Collider>().TryGetComponent(out IBullet bullet)) {
            bullet.Deactivate();
        }
    }

}
