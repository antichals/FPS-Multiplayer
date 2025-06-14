using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class PlayerPickUp : NetworkBehaviour
{
    [SerializeField] private float pickupRange = 4f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private LayerMask pickupLayers;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private PlayerWeapon _playerWeapon;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!base.IsOwner)
        {
            enabled = false;
            return;
        }

        //_cameraTransform = GetComponentInChildren<Camera>().transform;
        //_playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void Update()
    {

        Debug.DrawRay(_cameraTransform.position, _cameraTransform.forward * pickupRange, Color.red, 1f);

        if (Input.GetKeyDown(pickupKey))
        {
            // Debug.Log("[PlayerPickup] Key E pressed");
            Pickup();
        }
    }

    private void Pickup()
    {
        // Check for hit
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, pickupRange, pickupLayers))
        {
            Debug.Log("Hit NOTHING");
            return;
        }

        if(hit.transform.TryGetComponent(out GroundWeapon groundedWeapon))
        {
            // Debug.Log("PlayerPickup.Pickup] Ground weapon hit");
            _playerWeapon.ServerInitializeWeapon(groundedWeapon.PickupWeapon());

        }
        else
        {
            Debug.Log("GroundWeapon Component not found");
        }
            
    }
}
