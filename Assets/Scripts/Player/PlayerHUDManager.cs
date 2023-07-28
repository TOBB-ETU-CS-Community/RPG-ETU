using System;
using Player;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    private PlayerManager player;
    
    public void Awake()
    {
        player = GetComponent<PlayerManager>();
    }
    
    
    public void HandleHUDMaxValues()
    {
        UIManager.instance.SetHealthBarMaxValue(player.playerStatManager.maxHealth);
        UIManager.instance.SetStaminaBarMaxValue(player.playerStatManager.maxStamina);
    }
    
    public void HandleHUDCurrentValues()
    {
        UIManager.instance.UpdateHealthBar(player.playerStatManager.currentHealth);
        UIManager.instance.UpdateStaminaBar(player.playerStatManager.currentStamina);
    }

    public void HandleHUD()
    {
        if (player.playerStatManager.statChanged)
        {
            HandleHUDMaxValues();
            player.playerStatManager.statChanged = false;
        }
        
        HandleHUDCurrentValues();
    }
    
}