
using FlyingWormConsole3.LiteNetLibEditor;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using MessagePipe;
using UnityEngine;
using VContainer;

public abstract class BaseView : MonoBehaviour, IView
{
    protected IBindingContext bindingContext;
    protected IModel model;

    public bool IsActive => gameObject.activeSelf;

    
    public virtual void SetModel(IModel model)
    {
        this.model = model;
        OnModelChanged();
    }

    protected virtual void OnModelChanged()
    {
        bindingContext?.Clear();
        bindingContext = new BindingContext((Loxodon.Framework.Binding.Binders.IBinder)model);
    }
    
    public virtual void SetContext(IBindingContext context)
    {
        bindingContext = context;
        //bindingContext.Add("View", this);

    }

    public virtual void OnOpen()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
    public virtual void OnDestory()
    {
        Dispose();
    }
    
    public virtual void Dispose()
    {
        bindingContext?.Clear();
        bindingContext = null;
        model = null;
    }
}