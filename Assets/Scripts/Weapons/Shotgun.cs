using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public override void PlayWeaponAnimation()
    {
        Debug.Log("Shotgun Fired");
    }
}
