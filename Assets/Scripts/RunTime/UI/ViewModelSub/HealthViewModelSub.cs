using Loxodon.Framework.Observables;

public class HealthViewModelSub : ObservableObject
{
    private float _health;

    public float Health
    {
        get => _health;
        set => Set(ref _health, value);
    }
}