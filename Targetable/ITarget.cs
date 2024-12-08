using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    float health { get; set; }
    void OnDamaged(float damage);
    void Death();
}
