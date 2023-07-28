using System;
using UnityEngine;

namespace Player
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;
        [SerializeField] UIStatBar healthBar;
        [SerializeField] UIStatBar staminaBar;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        
        public void UpdateStaminaBar(float currentValue)
        {
            staminaBar.UpdateStatBar(currentValue);
        }
        
        public void SetStaminaBarMaxValue(float maxValue)
        {
            staminaBar.SetMaxValue(maxValue);
        }
        
        public void UpdateHealthBar(float currentValue)
        {
            healthBar.UpdateStatBar(currentValue);
        }
        
        public void SetHealthBarMaxValue(float maxValue)
        {
            healthBar.SetMaxValue(maxValue);
        }

       
    }
}