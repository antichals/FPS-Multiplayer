using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;


public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField] private List<Weapon> weapons = new List<Weapon>();
    [SerializeField] private Weapon currentWeapon;
    private readonly SyncVar<int> _currentWeaponIndex = new (-1);

    public override void OnStartClient()
    {
        base.OnStartClient();
        _currentWeaponIndex.OnChange += OnCurrentIndexChanged;
        InitializeWeapons();


        if(!IsOwner)
        {
            enabled = false;
            return;
        }

        ServerInitializeWeapon(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            currentWeapon.Fire();

      //DEBUG WEAPONS
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Weapon switched to PISTOL");
            ServerInitializeWeapon(0);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            ServerInitializeWeapon(1);
            Debug.Log("Weapon switched to RIFLE");
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            ServerInitializeWeapon(2);
            Debug.Log("Weapon switched to SHOTGUN");
        }
    }

    public void InitializeWeapons()
    { 
        for(int i = 0; i< weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(false);
        }      
    }
  
    [ServerRpc]
    public void ServerInitializeWeapon(int index)
    {
        _currentWeaponIndex.Value = index;
        Debug.Log("Seting current index to " + index);
    }

    private void OnCurrentIndexChanged(int oldIndex, int newIndex, bool asServer)
    {
        // Deactive previous weapon
        if(currentWeapon != null)
            currentWeapon.gameObject.SetActive(false);

        // Activate new weapon
        weapons[newIndex].gameObject.SetActive(true);
        Debug.Log("Activating weapon in slot " + newIndex);
        currentWeapon = weapons[newIndex];
    }

    private void FireWeapon()
    {
        currentWeapon.Fire();
    }
}
