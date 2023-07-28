using UnityEngine;
using UnityEngine.UI;

public class UIStatBar : MonoBehaviour
{
    [SerializeField] public Slider slider;
    
    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
    }
    
    public virtual void UpdateStatBar(float currentValue)
    {
        slider.value = currentValue;
    }
    
    public virtual void SetMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
    }
    
}
