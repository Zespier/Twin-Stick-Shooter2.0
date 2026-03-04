using System.Threading;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using TMPro;

public class HostConnectionManager : MonoBehaviour {

    public TMP_Text friendCodeText;
    private CancellationTokenSource _cts;
    private bool _isCreating;

    private void Awake() {
        _cts = new CancellationTokenSource(15000); //15 seconds timeout
    }

    private void Start() {
        NetworkManager.Singleton.OnTransportFailure += HandleTransportFailure;
    }

    private void HandleTransportFailure() {
        Debug.LogWarning("Transport failed. Restarting Relay...");

        NetworkManager.Singleton.Shutdown();

        // Optional delay to let shutdown complete
        StartCoroutine(RestartHostCoroutine());
    }

    private IEnumerator RestartHostCoroutine() {
        yield return new WaitForSeconds(1f);

        // Recreate Relay allocation
        yield return CreateRelayHost().AsIEnumerator();
    }

    private void OnDestroy() {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void OnHostButtonPressed() {
        _ = CreateRelayHost();
    }

    public async Task CreateRelayHost() {
        if (_isCreating) return;
        _isCreating = true;

        while (!GameBootstrap.Initialized) {
            await Task.Delay(100);
        }

        try {

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Join Code: {joinCode}");
            friendCodeText.text = joinCode;

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            var relayServerData = new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,        // connectionData
                allocation.ConnectionData,        // hostConnectionData (host uses same)
                allocation.Key,                   // key (MUST be here)
                true
            );

            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

        } catch (System.Exception e) {
            Debug.LogError($"Relay Host Failed: {e}");
        } finally {
            _isCreating = false;
        }
    }
}

public static class TaskExtensions {
    public static IEnumerator AsIEnumerator(this Task task) {
        while (!task.IsCompleted)
            yield return null;

        if (task.IsFaulted)
            throw task.Exception;
    }
}