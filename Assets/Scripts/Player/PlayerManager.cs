using System;
using System.Collections.Generic;
using FishNet.Example.Scened;
using FishNet.Object;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }
    private Dictionary<int, PlayerController> _players = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void RegisterPlayer(int ownerId, PlayerController controller)
    {
        if (!_players.ContainsKey(ownerId))
            _players.Add(ownerId, controller);
    }

    public void UnregisterPlayer(int ownerId)
    {
        if (_players.ContainsKey(ownerId))
            _players.Remove(ownerId);
    }

    public PlayerController GetPlayer(int ownerId)
    {
        _players.TryGetValue(ownerId, out var controller);
        return controller;
    }

    public void TogglePlayer(NetworkObject netObj, bool state)
    {
        PlayerController pc = netObj.GetComponent<PlayerController>();
        if (pc == null)
        {
            Debug.LogError("PlayerController component not found in" +  netObj.ToString());
            return;
        }

        // DEBUG
        Debug.Log("[PlayerManager.ServerTogglePlayer] Disabling player in server");
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
        //pc.characterController.enabled = state;
    }

    public void DisablePlayer(NetworkObject netObj)
    {

        if (!IsServerInitialized) return;
        
        // DEBUG
        Debug.Log("[PlayerManager.DisablePlayer] DisablePlayer method called on server");

        TogglePlayer(netObj, false);
    }

    public void EnablePlayer(NetworkObject netObj)
    {
        if (!IsServerInitialized) return;

        TogglePlayer(netObj, true);
    }
}
