using Mirror;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();


    public GameObject PlayerPrefab;
    public bool PlayerItemCreated = true;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Logger.Log("SERVER ADD PLAYER", this);
        PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
        GamePlayerInstance.ConnectionID = conn.connectionId;
        GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
        GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.instance.CurrentLobbyId, GamePlayers.Count);
        NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }


    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
            CreateHostPlayerItem();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerPrefab) as GameObject;
            PlayerObjectController NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerObjectController>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.SetPlayerValues();

            GamePlayers.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayer()
    {
        foreach (PlayerObjectController player in GamePlayers)
        {
            if (!GamePlayers.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerPrefab) as GameObject;
                PlayerObjectController NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerObjectController>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.SetPlayerValues();

                GamePlayers.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayer()
    {
        foreach (PlayerObjectController player in GamePlayers)
        {
            foreach (PlayerObjectController PlayerScript in GamePlayers)
            {
                if (PlayerScript.ConnectionID == player.ConnectionID)
                {
                    PlayerScript.PlayerName = player.PlayerName;
                    PlayerScript.SetPlayerValues();
                }
            }
        }
    }

    public void RemovePlayer()
    {
        List<PlayerObjectController> playerListItemsToRemove = new List<PlayerObjectController>();

        foreach (PlayerObjectController playerListItem in GamePlayers)
        {
            if (!GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerObjectController toRemove in playerListItemsToRemove)
            {
                GameObject objectToRemove = toRemove.gameObject;
                GamePlayers.Remove(toRemove);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }

}
