using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : BaseWeapon
{
    private void Start()
    {
        fireRate = 0.3f;
        reloadTime = 0.6f;
        magSize = 16;
        bulletCount = magSize;
        damage = 10;
    }
}