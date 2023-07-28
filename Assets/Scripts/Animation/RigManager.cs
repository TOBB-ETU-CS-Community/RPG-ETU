using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigManager : MonoBehaviour
{
    public static RigManager instance;
    [SerializeField] public RigBuilder rigBuilder;
    [SerializeField] public float[] targetWeights;
    [SerializeField] public float[] targetLerpSpeeds;
    
    [SerializeField] public float[] defaultWeights;
    private void Awake()
    {
        rigBuilder = GetComponent<RigBuilder>();
        targetWeights = new float[rigBuilder.layers.Count];
        targetLerpSpeeds = new float[rigBuilder.layers.Count];
        defaultWeights = new float[rigBuilder.layers.Count];
        
    }

    private void Start()
    {
        targetLerpSpeeds[0] = 0.1f;
        targetLerpSpeeds[1] = 0.05f;
        
        targetWeights[0] = 0f;
        targetWeights[1] = 0f;
        
        defaultWeights[0] = 0f;
        defaultWeights[1] = 0f;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        for (int i = 0; i < rigBuilder.layers.Count; i++)
        {
            rigBuilder.layers[i].rig.weight = Mathf.Lerp(rigBuilder.layers[i].rig.weight, targetWeights[i], targetLerpSpeeds[i]);
        }
    }

    public void RigWeight_Arm(float weight)
    {
        targetWeights[1] = weight;
    }
    
    public void RigWeight_Look(float weight)
    {
        targetWeights[0] = weight;
    }

    public void LerpSpeed_Arm(float speed)
    {
        targetLerpSpeeds[1] = speed;
    }
   
    public void LerpSpeed_Look(float speed)
    {
        targetLerpSpeeds[0] = speed;
    }
    
    public void ResetRigWeights()
    {
        targetWeights[0] = defaultWeights[0];
        targetWeights[1] = defaultWeights[1];
    }
}