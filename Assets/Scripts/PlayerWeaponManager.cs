using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerWeaponManager : NetworkBehaviour
{
    [SerializeField] private List<AWeapon> weapons = new List<AWeapon>();
    [SerializeField] private AWeapon currentWeapon;
    private int currentIndex;


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
    }



    // TODO the idea is to take the camera and set the weapons childs of it (to follow rotation)
    public void InitializeWeapons(Transform parentWeapons)
    {
        /*
        for(int i = 0; i< weapons.Count; i++)
        {
            weapons[i].transform.SetParent(parentWeapons);
            weapons[i].gameObject.SetActive(false);
        }
        */

        if(weapons.Count > 0)
        {
            currentWeapon = weapons[0];
            currentIndex = 0;
        }
    }

    private void InitializeWeapon(int index)
    {
        // Deactive previous weapon
        weapons[currentIndex].gameObject.SetActive(false);

        // Activate new weapon
        weapons[index].gameObject.SetActive(true);
        currentIndex = index;
    }


    private void FireWeapon()
    {
        currentWeapon.Fire();
    }
}
