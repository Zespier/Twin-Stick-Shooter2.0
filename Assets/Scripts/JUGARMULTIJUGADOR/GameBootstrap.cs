using System.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class GameBootstrap : MonoBehaviour {

    public static bool Initialized {
        get {
            if (instance == null) {
                return false;
            } else {
                return _initialized;
            }
        }
    }

    private static bool _initialized;

    public static GameBootstrap instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private async void Start() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        _initialized = true;
    }

    public static long BytesSentThisSession;
    public static long BytesSentThisMonth;
}
