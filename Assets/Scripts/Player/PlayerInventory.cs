using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInventory : MonoBehaviour
{
    public PlayerManager player;
    
    public WeaponSlotManager weaponSlotManager;
    public WeaponItem weapon;
    
    [SerializeField] public bool isEquipped;

    private void Awake()
    {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        player = GetComponent<PlayerManager>();
    }

    public void ToggleWeapon()
    {
        RigManager rigManager = RigManager.instance;
        if (isEquipped)
        {
            weaponSlotManager.UnloadWeapon();
            isEquipped = false;
            player.playerAnimationManager.PlayTargetActionAnimation("Sheath",true);
            rigManager.defaultWeights[0]  = 0f;
            rigManager.defaultWeights[1] = 0f;
        }
        else
        {
            weaponSlotManager.LoadWeapon(weapon);
            isEquipped = true;
            player.playerAnimationManager.PlayTargetActionAnimation("Unsheath",true);
            rigManager.defaultWeights[0] = 1f;
            rigManager.defaultWeights[1] = 1f;
        }
        
        player.playerAnimationManager.UpdateAnimatorBoolParameters("isEquipped",isEquipped);
    }
    
    
}
