using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class UIEnemy : MonoBehaviour
{
    [SerializeField] private UIStatBar healthBar;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform enemyTransform;

    public void UpdateHealthBar(float currentValue)
    {
        healthBar.UpdateStatBar(currentValue);
    }
        
    public void SetHealthBarMaxValue(float maxValue)
    {
        healthBar.SetMaxValue(maxValue);
    }
    
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
    
    public void HideUI()
    {
        gameObject.SetActive(false);
    }
    
    public void RotateUI()
    {
        transform.LookAt(playerCamera.transform);
    }
    
    public void MoveUI()
    {
        transform.position = enemyTransform.position + new Vector3(0, 1, 0);
    }
       
}