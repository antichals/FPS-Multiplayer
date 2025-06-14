using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public abstract void Fire();
}
