using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    private WeaponHolderSlot weaponSlot;

    private void Awake()
    {
       weaponSlot = GetComponent<WeaponHolderSlot>();
    }

    public void LoadWeapon(WeaponItem weaponItem)
    {
        weaponSlot.LoadWeaponModel(weaponItem);
    }
    
    public void UnloadWeapon()
    {
        weaponSlot.UnloadWeaponModel();
    }
}
