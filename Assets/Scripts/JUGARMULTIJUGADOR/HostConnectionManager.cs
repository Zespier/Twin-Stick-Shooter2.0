using System.Threading;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;

public class HostConnectionManager : MonoBehaviour {

    private CancellationTokenSource _cts;
    private bool _isCreating;

    private void Awake() {
        _cts = new CancellationTokenSource(15000); //15 seconds timeout
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

            Debug.Log("Creating relay allocation...");

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Join Code: {joinCode}");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            var relayServerData = new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.ConnectionData, // host uses its own connection data here
                                           //allocation.IsSecure
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
