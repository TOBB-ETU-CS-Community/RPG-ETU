using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitboxManager : MonoBehaviour
{
   private Collider hitboxCollider;

   public void EnableCollider()
   {
      print("COLLIDER ENABLED");
      print(hitboxCollider.GetHashCode());
      hitboxCollider.enabled = true;
   }
   
   public void DisableCollider()
   {
      print("COLLIDER DISABLED");
      hitboxCollider.enabled = false;
   }

   public void SetHitboxCollider(Collider collider)
   {
      hitboxCollider = collider;
   }


}
