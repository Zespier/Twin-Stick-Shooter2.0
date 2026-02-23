using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class GameBootstrap : MonoBehaviour {

    public static GameBootstrap instance;
    private void Awake() {
        if (!instance) { instance = this; }

    }

    private async void Start() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
