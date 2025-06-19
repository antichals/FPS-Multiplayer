using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using JetBrains.Annotations;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public int damage;
    public float maxRange = 20f;
    public LayerMask weaponHitLayer;
    private Transform _cameraTransform;


    private void Awake()
    {
        _cameraTransform = transform.parent;
    }

    public void Fire()
    {
        PlayWeaponAnimation();

        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, maxRange))
        {
            Debug.Log("[Weapon.Fire] Nothing Hit");
            return;
        }

        // If weapon hits player
        if (hit.transform.TryGetComponent(out PlayerHealth health))
        {
            Debug.Log("[Weapon.Fire] Hit a player!");
            health.TakeDamage(damage);
        }
    }

    public abstract void PlayWeaponAnimation();
}
