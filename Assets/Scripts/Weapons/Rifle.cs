using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    public override void PlayWeaponAnimation()
    {
        Debug.Log("Rifle Fired");  
    }
}
