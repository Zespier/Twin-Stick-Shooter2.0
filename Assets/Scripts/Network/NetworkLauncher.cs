using Unity.Netcode;
using UnityEngine;

public class NetworkLauncher : MonoBehaviour {

    void OnGUI() {

        if(NetworkManager.Singleton == null) { return; }

        if (!NetworkManager.Singleton.IsClient &&
            !NetworkManager.Singleton.IsServer) {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Host"))
                NetworkManager.Singleton.StartHost();

            if (GUI.Button(new Rect(10, 60, 200, 40), "Client"))
                NetworkManager.Singleton.StartClient();
        }
    }
}