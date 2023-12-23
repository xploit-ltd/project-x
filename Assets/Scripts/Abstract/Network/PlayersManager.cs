using Xploit.Abstract.Core.Singleton;
using Unity.Netcode;
using UnityEngine;
using DilmerGames.Core.Singletons;

public class PlayersManager : NetworkSingleton<PlayersManager>
{
    NetworkVariable<int> playersInGame = new();

    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log($"{id} just connected...");
            if (NetworkManager.Singleton.IsServer)
                playersInGame.Value++;
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            Debug.Log($"{id} just disconnected...");
            if (NetworkManager.Singleton.IsServer)
                playersInGame.Value--;
        };
    }
}