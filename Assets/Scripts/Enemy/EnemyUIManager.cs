using System;
using Player;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    private EnemyManager enemy;
    public UIEnemy uiEnemy;
    
    public void Awake()
    {
        enemy = GetComponent<EnemyManager>();
        uiEnemy = GetComponentInChildren<UIEnemy>();
    }

    public void Start()
    {
        uiEnemy.SetHealthBarMaxValue(enemy.enemyStatManager.maxHealth);
        uiEnemy.ShowUI();
    }

    public void HandleUI()
    {
        uiEnemy.MoveUI();
        uiEnemy.RotateUI();
        uiEnemy.UpdateHealthBar(enemy.enemyStatManager.currentHealth);
    }
    
}