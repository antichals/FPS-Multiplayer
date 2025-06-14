using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class GroundWeapon : NetworkBehaviour
{
    public int index = -1;

    public int PickupWeapon()
    {
        ServerDespawnWeapon();
        return index;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerDespawnWeapon()
    {
        ServerManager.Despawn(gameObject);
    }
}
