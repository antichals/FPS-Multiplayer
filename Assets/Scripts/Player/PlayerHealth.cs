using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour 
{
    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;

    public void Awake()
    {
        _currentHealth = maxHealth;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if(!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    public void TakeDamage(int damage)
    {
        ServerTakeDamage(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerTakeDamage(int damage)
    {
        _currentHealth -= damage;
        Debug.Log("[PlayerHealth.ServerTakeDamage] Current health: " +  _currentHealth);

        // Send new currentHealth to the local player


        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            HandlePlayerDead();
        }

        LocalTakeDamage(Owner, _currentHealth);
    }

    [TargetRpc]
    private void LocalTakeDamage(NetworkConnection connection, int newHealth)
    {
        UIManager.SetHealthText(newHealth.ToString()); 
    }

    private void HandlePlayerDead()
    {

        if (!IsServerInitialized) return;
        // DEBUG
        Debug.Log("[PlayerHealth.HandlePlayerDead] Toogling player off");

        PlayerManager.Instance?.DisablePlayer(GetComponent<NetworkObject>());
    }
}
