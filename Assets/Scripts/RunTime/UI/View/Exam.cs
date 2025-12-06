using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using TMPro;
using UnityEngine.UI;

public class Exam : UIView
{
    public TextMeshProUGUI HealthText;
    
    public Slider HealthSlider;

    
    
    
    protected override void Awake()
    {
        base.Awake();
        //获得应用上下文
        ApplicationContext context = Context.GetApplicationContext();

        //启动数据绑定服务
        BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
        bindingService.Start();
    }


    protected override void Start()
    {
        base.Start();
        HealthViewModelSub healthViewModelSub = new HealthViewModelSub() { Health = 100f };
            
            
        HealthViewModel healthViewModel = new HealthViewModel()
            {
                HealthViewModelSub = healthViewModelSub
            };
            
        
        
        IBindingContext bindingContext = this.BindingContext();
        
        bindingContext.DataContext = healthViewModel;
        
        
        
        BindingSet<Exam, HealthViewModel> bindingSet;
        
        bindingSet = this.CreateBindingSet<Exam, HealthViewModel>();

        bindingSet.Bind(this.HealthText).For(v => v.text).To(vm => vm.HealthViewModelSub.Health).OneWay();

        bindingSet.Bind(this.HealthSlider).For(v => v.value,v => v.onValueChanged).To(vm => vm.HealthViewModelSub.Health).TwoWay();
        
        //bindingSet.Bind(this.HealthSlider).For(v => v.onValueChanged).To<float>(vm => vm.OnHealthValueChanged);
        
        
        bindingSet.Build();
    }
    
}