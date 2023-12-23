using Unity.Netcode.Transports.UTP;
using Xploit.Abstract.Core.Singleton;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class RelayManager : Singleton<RelayManager>
{
    [SerializeField]
    private string environment = "dev";

    [SerializeField]
    private int maxConnection = 10;

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public async Task<RelayHostData> SetupRelay()
    {
        Debug.Log($"Relay Server starting with max connections {maxConnection}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnection);

        RelayHostData relayHostData = new()
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);

        Transport.SetRelayServerData(
            relayHostData.IPv4Address,
            relayHostData.Port,
            relayHostData.AllocationIDBytes,
            relayHostData.Key, 
            relayHostData.ConnectionData);

        Debug.Log($"Relay Server generate a join code {relayHostData.JoinCode}");

        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        RelayJoinData relayJoinData = new()
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            IPv4Address = allocation.RelayServer.IpV4,
            JoinCode = joinCode
        };

        Transport.SetRelayServerData(
            relayJoinData.IPv4Address,
            relayJoinData.Port,
            relayJoinData.AllocationIDBytes,
            relayJoinData.Key,
            relayJoinData.ConnectionData,
            relayJoinData.HostConnectionData);

        Debug.Log($"Client joined game with join code {relayJoinData.JoinCode}");

        return relayJoinData;
    }
}
