using System;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{

    public GameObject currentWeaponModel;
    public Transform parentOverride;

    public void LoadWeaponModel(WeaponItem weaponItem)
   {
       if (weaponItem == null)
           return;

       var model = Instantiate(weaponItem.modelPrefab);

       if (model != null)
       {
           if(parentOverride != null)
               model.transform.parent = parentOverride;
           else
               model.transform.parent = transform;
       }
       model.transform.localPosition = Vector3.zero;
       model.transform.localRotation = Quaternion.identity;
       model.transform.localScale = Vector3.one;
       currentWeaponModel = model;
       
   }
   
    public void UnloadWeaponModel()
    {
         if (currentWeaponModel != null)
         {
              Destroy(currentWeaponModel);
              currentWeaponModel = null;
         }
    }
}
