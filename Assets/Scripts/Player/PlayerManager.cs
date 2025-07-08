using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using static PlayerManager;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager _instance { get; private set; }
    private Dictionary<int, PlayerData> _players;   // map<int clientID, class PlayerData> to access all player info
    private List<int> _currentDeadPlayers;
    


    // GameModeManager
    public List<Transform> spawnPoints;


    // TODO Change to GameMode
    [SerializeField] private float respawnTime = 5;


 

    public class PlayerData
    {
        // Info
        public int clientID;
        public NetworkObject netObject;

        // Stats
        public int kills;
        public int deaths;
        public float lastDeathTime;

        // Constructor
        public PlayerData(int clientID, NetworkObject netObject)
        {
            this.clientID = clientID;
            this.netObject = netObject;

            kills = 0;
            deaths = 0;
            lastDeathTime = -99;
        }
    }

    // TODO Change when separate server and client builds logic
    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!IsServerInitialized)
        {
            enabled = false;
            return;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        // Init PlayerManager
        _instance = this;
        _instance._players = new Dictionary<int, PlayerData>();
        _instance._currentDeadPlayers = new List<int>();
    }


    public void RegisterPlayer(int newClientID, NetworkObject netObject)
    {
        // DEBUG

        if (!_instance._players.ContainsKey(newClientID)) 
        {
            PlayerData newPlayerData = new PlayerData(newClientID, netObject);
            _instance._players.Add(newClientID, newPlayerData);
        }        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // If not server or list empty: return
        if (!IsServerInitialized || _currentDeadPlayers.Count == 0) return;


        // Check for players to spawn 
        for (int i = 0; i < _currentDeadPlayers.Count;)
        {
            int clientID = _currentDeadPlayers[i];
            PlayerData playerData = _players[clientID];

            if (Time.time >= playerData.lastDeathTime + respawnTime)
            {
                // Remove from death list
                _currentDeadPlayers.RemoveAt(i); // Since we are removing at [0] we don’t need to increment i

                HandlePlayerRespawn(clientID);


                // TODO Spawn player at another spawn location

            }
            else
            {
                // Because players are stored in death order, we can early exit.
                break;
            }
        }
    }

    public void UnregisterPlayer(int clientID)
    {
        if (!_instance._players.ContainsKey(clientID))
        {
            _instance._players.Remove(clientID);
            ObserversRemoveScore(clientID);
        }
    }

    public PlayerData GetPlayerData(int ownerId)
    {
        _instance._players.TryGetValue(ownerId, out PlayerData playerData);
        return playerData;
    }

    public void HandlePlayerDead(int victimID, int attackerID)
    {
        // DEBUG
        Debug.Log("[PlayerManager.HandlePlayerDead] Method called");

        
        // Get Data from both playes
        PlayerData attackerData = GetPlayerData(attackerID);
        PlayerData victimData = GetPlayerData(victimID);

        // Handle Stats and Data
        attackerData.kills++;
        victimData.deaths++;
        victimData.lastDeathTime = Time.time;
        _currentDeadPlayers.Add(victimID);

        // Handle victim object and components
        NetworkObject netObject = GetPlayerData(victimID).netObject;
        DisablePlayer(netObject);

        // Send score update to all clients
        ObserversSendScore(attackerID, attackerData.netObject.name, attackerData.kills, attackerData.deaths);
        ObserversSendScore(victimID, victimData.netObject.name, victimData.kills, victimData.deaths);

        // DEBUG
        string attackerName = attackerData.netObject.name;
        string victimName = victimData.netObject.name;
        Debug.Log($"[PlayerManager] {attackerName} killed {victimName}.");
        Debug.Log($"[PlayerManager] {attackerName} has {attackerData.kills} kills.");
        Debug.Log($"[PlayerManager] {attackerName} has {attackerData.kills * 10} points!");


        NetworkObject netObj = GetPlayerData(victimID).netObject;
        // Calculate spawnPoint
        Transform spawnLocation;
        if (spawnPoints.Count == 0)
        {
            // If list empty, respawn on death spot
            spawnLocation = netObj.transform;
        }
        else
        {
            // Else get a random spawn point
            spawnLocation = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)]; // choose random point in list
        }

        // Give new location 
        SetPlayerPosition(victimID, spawnLocation);
    }

    public void HandlePlayerRespawn(int clientID)
    {
        NetworkObject netObj = GetPlayerData(clientID).netObject;


        EnablePlayer(netObj);

        // Handle Player health
        PlayerHealth playerHealth = netObj.GetComponent<PlayerHealth>();
        playerHealth.ServerUpdateHealth(playerHealth._maxHealth);
    }

    public void DisablePlayer(NetworkObject netObj)
    {
        // DEBUG
        //Debug.Log("[PlayerManager.DisablePlayer] DisablePlayer method called on server");

        TogglePlayer(netObj, false);
    }

    public void EnablePlayer(NetworkObject netObj)
    {
        TogglePlayer(netObj, true);
    }

    public void TogglePlayer(NetworkObject netObj, bool state)
    {
        // DEBUG
        //Debug.Log("[PlayerManager.ServerTogglePlayer] Disabling player in server");

        PlayerController pc = netObj.GetComponent<PlayerController>();
        if (pc == null)
        {
            Debug.LogError("[PlayerManager.TogglePlayer] PlayerController component not found in" +  netObj.ToString());
            return;
        }

        pc.TogglePlayer(state);
        ObserversTogglePlayer(netObj, state);
    }

    [ObserversRpc]
    private void ObserversTogglePlayer(NetworkObject netObj, bool state)
    {
        // DEBUG
        Debug.Log("[PlayerManager.ObserversTogglePlayer] Disabling player in client");

        PlayerController pc = netObj.GetComponent<PlayerController>();
        if (pc == null)
        {
            Debug.LogError("PlayerController component not found in" + netObj.ToString());
            return;
        }

        pc.TogglePlayer(state);
   }

    public void SetPlayerPosition(int clientID, Transform newLocation)
    {
        // Get player networkobject
        NetworkObject netObj = GetPlayerData(clientID).netObject;

        NetworkConnection netConnection = netObj.Owner;
        // Get Player controller component
        PlayerController playerController = netObj.GetComponent<PlayerController>();


        // Set player location in server
        //netObj.transform.SetPositionAndRotation(newLocation.position, newLocation.rotation);

        // Set player location in client
        playerController.TargetSetPlayerPosition(netConnection, newLocation.position, newLocation.rotation);
    }

    public void SubscribeToPlayerDead(PlayerHealth playerHealth)
    {
        playerHealth.OnPlayerDead += HandlePlayerDead;
    }

    [ObserversRpc]
    public void  ObserversSendScore(int clientID, string nickname, int kills, int deaths)
    {
        // DEBUG
        Debug.Log($"[PlayerManager.ObserversSendScore] Sending score update for player {nickname}");


        UIManager._instance?.UpdateScoreboard(clientID, nickname, kills, deaths);
    }

    [ObserversRpc]
    public void ObserversRemoveScore(int clientId)
    {
        UIManager._instance?.RemoveScoreboardEntry(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendAllScores()
    {
        foreach (var kvp in _players)
        {
            ObserversSendScore(kvp.Key, kvp.Value.netObject.name, kvp.Value.kills, kvp.Value.deaths);
        }
    }










}
