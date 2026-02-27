using System.Threading;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class ClientConnectionManager : MonoBehaviour {

    public TMP_InputField joinCodeInputField;

    private CancellationTokenSource _cts;
    private bool _isCreating;

    private void Awake() {
        _cts = new CancellationTokenSource(15000); //15 seconds timeout
    }

    private void OnDestroy() {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void OnClientButtonPressed() {
        _ = CreateRelayHost(joinCodeInputField.text);
    }

    public async Task CreateRelayHost(string joinCode) {
        if (_isCreating) return;
        _isCreating = true;

        while (!GameBootstrap.Initialized) {
            await Task.Delay(100);
        }

        try {

            Debug.Log("Creating relay allocation...");

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            Debug.Log($"Join Code: {joinCode}");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            var relayServerData = new RelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData, // THIS is different for client
                joinAllocation.Key,
                //joinAllocation.IsSecure
                true
            );

            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

        } catch (System.Exception e) {
            Debug.LogError($"Relay Host Failed: {e}");
        } finally {
            _isCreating = false;
        }
    }
}
