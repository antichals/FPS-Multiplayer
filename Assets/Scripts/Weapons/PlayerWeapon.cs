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
        InitializeWeapons();
        _currentWeaponIndex.OnChange += OnCurrentIndexChanged;



        if(!IsOwner)
        {
                enabled = false;
            return;
        }

        //currentWeapon = weapons[0];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            currentWeapon.Fire();

    /*  DEBUG WEAPONS
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Weapon switched to PISTOL");
            InitializeWeapon(0);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            InitializeWeapon(1);
            Debug.Log("Weapon switched to RIFLE");
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            InitializeWeapon(2);
            Debug.Log("Weapon switched to SHOTGUN");
        }
    */
    
    }

    public void InitializeWeapons()
    {
        
        for(int i = 0; i< weapons.Count; i++)
        {
            //weapons[i].transform.SetParent(parentWeapons);
            weapons[i].gameObject.SetActive(false);
        }
        
        ServerInitializeWeapon(0);
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
