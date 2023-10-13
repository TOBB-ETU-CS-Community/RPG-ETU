using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    private WeaponHolderSlot weaponSlot;
    public WeaponHitboxManager weaponHitboxManager;

    private void Awake()
    {
       weaponSlot = GetComponent<WeaponHolderSlot>();
       weaponHitboxManager = GetComponent<WeaponHitboxManager>();
    }

    public void LoadWeapon(WeaponItem weaponItem)
    {
        weaponSlot.LoadWeaponModel(weaponItem);
        weaponHitboxManager.SetHitboxCollider(weaponSlot.currentWeaponModel.GetComponentInChildren<Collider>());
    }
    
    public void UnloadWeapon()
    {
        weaponSlot.UnloadWeaponModel();
        weaponHitboxManager.SetHitboxCollider(null);
    }
    
}
