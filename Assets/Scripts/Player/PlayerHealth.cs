using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour 
{
    [SerializeField] public int _maxHealth = 100;
    private int _currentHealth;

    public event Action<int> OnHealthChanged;  // Event for scripts to subscribe

    public void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if(!IsOwner)
        {
            enabled = false;
            return;
        }

        UIManager._instance.SubscribeToHealthChange(this); // Pass PlayerHealth component reference to UIManager
    }

    public void TakeDamage(int damage, int attackerID)
    {
        ServerTakeDamage(damage, attackerID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerTakeDamage(int damage, int attackerID)
    {
        int newHealth = _currentHealth - damage;
        _currentHealth = newHealth;

        // DEBUG
        Debug.Log("[PlayerHealth.ServerTakeDamage] Current health: " +  _currentHealth);

        // Send new currentHealth to the local player


        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            HandlePlayerDead(attackerID);
        }

        TargetChangeHealth(Owner, _currentHealth);
    }

    [TargetRpc]
    public void TargetChangeHealth(NetworkConnection connection, int newHealth)
    {
        _currentHealth = newHealth;

        // We only need to invoke in local
        OnHealthChanged?.Invoke(_currentHealth);
    }

    [Server]
    public void ServerUpdateHealth(int newHealth)
    {
        // Change health in server
        _currentHealth = _maxHealth;

        // Send to owning client only
        TargetChangeHealth(Owner, _currentHealth);
    }

    private void HandlePlayerDead(int attackerID)
    {
        // DEBUG
        //Debug.Log("[PlayerHealth.HandlePlayerDead] Toogling player off");

        PlayerManager.Instance?.HandlePlayerDead(OwnerId, attackerID);
        
        //PlayerManager.Instance?.DisablePlayer(NetworkObject);
    }

    private void InvokeHealthChanged()
    {
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
