using Loxodon.Framework.ViewModels;
using UnityEngine;

public class HealthViewModel : ViewModelBase
{
    private HealthViewModelSub _healthViewModelSub;
    public HealthViewModelSub HealthViewModelSub
    {
        get => _healthViewModelSub;
        set => Set<HealthViewModelSub>(ref _healthViewModelSub, value, "HealthViewModelSub");
    }
    
    public void OnHealthValueChanged(float value)
    {
        
        Debug.LogFormat("Health ValueChanged:{0}", value);
    }
}