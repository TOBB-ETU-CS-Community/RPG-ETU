using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Items/Weapon")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;

    [Header("Animations")]
    public string Light_Attack_1;
    public string Light_Attack_2;
    public string Light_Attack_3;
    public string Heavy_Attack_1;
    public string Heavy_Attack_2;
    public string Heavy_Attack_3;
    public string Idle;
    
}
