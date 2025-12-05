using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
public interface IView : IDisposable
{
    public bool IsActive{ get; }
    public void SetModel(IModel model);
    public void OnOpen();
    public void OnClose();

}