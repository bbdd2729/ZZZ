

public class UIBaseController : IUIController
{
    public string UIName { private set; get; }
    
    private UIView _view; 
    
    #region 
    public virtual void OnInit()
    {
        DebugX.Intances.Log();
    }
    public virtual void OnOpen()
    {
        DebugX.Intances.Log();              
    }
    public virtual void OnClose()
    {
        DebugX.Intances.Log();        
    }
    public virtual void OnHold()
    {
        DebugX.Intances.Log();                     
    }
    public virtual void OnDestory()
    {
        DebugX.Intances.Log();             
    }
    public virtual void OnShow()
    {
        DebugX.Intances.Log();                 
    }
    public virtual void OnHide()
    {
        DebugX.Intances.Log();               
    }
    
    #endregion
    
    
    
    
    
    
    
}