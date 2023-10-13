using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyParentalControl : MonoBehaviour
{
   
   public static EnemyParentalControl instance;
   public GameObject[] enemies;

   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }

   public void Update()
   {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");
   }
   
   public void GetAllEnemies()
   {
      
   }
   
   public void DisableAllEnemies()
   {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");
      foreach (var enemy in enemies)
      {
         enemy.SetActive(false);
      }
   }
   
   public void EnableAllEnemies()
   {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");
      foreach (var enemy in enemies)
      {
         enemy.SetActive(true);
      }
   }
}
