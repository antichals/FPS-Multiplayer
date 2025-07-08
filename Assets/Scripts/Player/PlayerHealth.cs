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

    // Event for scripts to subscribe
    public event Action<int> OnHealthChanged;  
    public event Action<int, int> OnPlayerDead; 

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


        // TODO Crear un event manager o algo
        UIManager._instance.SubscribeToHealthChange(this); // Pass PlayerHealth component reference to UIManager
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        PlayerManager._instance?.SubscribeToPlayerDead(this);
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
            InvokePlayerDead(attackerID);
        }

        TargetChangeHealth(Owner, _currentHealth);
    }

    [TargetRpc]
    public void TargetChangeHealth(NetworkConnection connection, int newHealth)
    {
        _currentHealth = newHealth;

        // We only need to invoke in local
        InvokeHealthChanged();
    }

    [Server]
    public void ServerUpdateHealth(int newHealth)
    {
        // Change health in server
        _currentHealth = _maxHealth;

        // Send to owning client only
        TargetChangeHealth(Owner, _currentHealth);
    }

    private void InvokePlayerDead(int attackerID)
    {

        OnPlayerDead?.Invoke(OwnerId, attackerID); // Call event
        //PlayerManager._instance?.HandlePlayerDead(OwnerId, attackerID);
    }

    private void InvokeHealthChanged()
    {
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
