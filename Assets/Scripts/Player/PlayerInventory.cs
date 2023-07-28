using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInventory : MonoBehaviour
{
    public PlayerManager player;
    public WeaponSlotManager weaponSlotManager;
    public WeaponItem weapon;
    
    [SerializeField] private bool isEquipped;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        player = GetComponent<PlayerManager>();
    }

    public void ToggleWeapon()
    {
        if (isEquipped)
        {
            weaponSlotManager.UnloadWeapon();
            isEquipped = false;
            player.playerAnimationManager.PlayTargetActionAnimation("Sheath",true);
        }
        else
        {
            weaponSlotManager.LoadWeapon(weapon);
            isEquipped = true;
            player.playerAnimationManager.PlayTargetActionAnimation("Unsheath",true);
        }
        
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isEquipped",isEquipped);
    }
    
    
}
