using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<LobbyEnter_t> LobbyEnter;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;

    public ulong CurrentLobbyId;
    private CustomNetworkManager manager;

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }
        if (instance == null) { instance = this; }

        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        Logger.Log("SteamMatchMake createlobby", this);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Logger.Log("OnLobbyCreated", this);

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + " Lobby");

        manager.StartGame("Game");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Logger.Log("Request to join lobby", this);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        Logger.Log("Lobby entered", this);
        CurrentLobbyId = callback.m_ulSteamIDLobby;

        if (NetworkServer.active) { return; }

        Logger.Log("Start Client Connection", this);
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new(callback.m_ulSteamIDLobby), "HostAddress");
        manager.StartClient();
    }
}
