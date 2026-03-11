using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Mejoritarecogible : NetworkBehaviour {

    private void Update() {
        if (Ship.instanceOfClient == null) { return; }
        if (!IsServer) { return; }

        Vector3 clampedPosition = transform.position;
        clampedPosition = new Vector3(clampedPosition.x, 0, clampedPosition.z);

        if ((Ship.instanceOfClient.transform.position - clampedPosition).sqrMagnitude < 2 * 2) {
            UpgradeCardManager.instance.Open();
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
