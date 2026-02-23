using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkLauncher : MonoBehaviour {

    private bool _hosting;
    private bool _clienting;

    void OnGUI() {

        if (NetworkManager.Singleton == null) { return; }

        if (!NetworkManager.Singleton.IsClient &&
            !NetworkManager.Singleton.IsServer) {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Host")) {
                StartCoroutine(C_StartHostWaitForServicesInitialization());
            }

            if (GUI.Button(new Rect(10, 60, 200, 40), "Client")) {
                StartCoroutine(C_StartClientWaitForServicesInitialization());
            }
        }
    }

    private IEnumerator C_StartHostWaitForServicesInitialization() {
        _hosting = true;
        while (GameBootstrap.instance == null) {
            yield return null;
        }

        NetworkManager.Singleton.StartHost();
        _hosting = false;
    }

    private IEnumerator C_StartClientWaitForServicesInitialization() {
        _clienting = true;
        while (GameBootstrap.instance == null) {
            yield return null;
        }

        NetworkManager.Singleton.StartClient();
        _clienting = false;
    }
}