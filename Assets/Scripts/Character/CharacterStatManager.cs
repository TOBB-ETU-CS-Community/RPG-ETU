using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatManager : MonoBehaviour
{
    public void Update()
    {
        HandleAllStatChanges();
        HandleStatConstraints();
    }
    

    protected virtual void HandleAllStatChanges()
    {
       

    }

    protected virtual void HandleStatConstraints()
    { 
       
    }
    

}