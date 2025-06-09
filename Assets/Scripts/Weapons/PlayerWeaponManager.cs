using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerWeaponManager : NetworkBehaviour
{
    [SerializeField] private List<AWeapon> weapons = new List<AWeapon>();
    [SerializeField] private AWeapon currentWeapon;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!base.IsOwner)
        {
            enabled = false;
            return;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            currentWeapon.Fire();

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
    }

    // TODO quitar parentWeapons if not needed
    public void InitializeWeapons(Transform parentWeapons)
    {
        
        for(int i = 0; i< weapons.Count; i++)
        {
            //weapons[i].transform.SetParent(parentWeapons);
            weapons[i].gameObject.SetActive(false);
        }
        

        InitializeWeapon(0);
    }

    private void InitializeWeapon(int index)
    {
        // Deactive previous weapon
        currentWeapon.gameObject.SetActive(false);

        // Activate new weapon
        weapons[index].gameObject.SetActive(true);
        currentWeapon = weapons[index];
    }


    private void FireWeapon()
    {
        currentWeapon.Fire();
    }
}
