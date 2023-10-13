using UnityEngine;

public class EnemyManager : CharacterManager
{
    
    
    [HideInInspector] public EnemyStatManager enemyStatManager;
    [HideInInspector] public EnemyUIManager enemyUIManager;
    [HideInInspector] public EnemyCombatManager enemyCombatManager;
    


    protected override void Awake()
    {
        base.Awake();
        enemyStatManager = GetComponent<EnemyStatManager>();
        enemyUIManager = GetComponent<EnemyUIManager>();
        enemyCombatManager = GetComponent<EnemyCombatManager>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    { 
        base.LateUpdate();
        enemyUIManager.HandleUI();
    }

    public override void ResetActionFlags()
    {
        isPerformingAction = false;
        applyRootMotion = false;
        canRotate = true;
        canMove = true;
        isLightAttacking = false;
    }
    
}