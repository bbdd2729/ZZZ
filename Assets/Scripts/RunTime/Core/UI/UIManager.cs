using Loxodon.Framework.Views;
using RunTime.Core.UI;
using UnityEngine;

public class UIManager : IUIManager
{
    private IUIViewLocator locator = new DefaultUIViewLocator();

    public UIManager()
    {
        Debug.Log("已创建UIManager实例");
    }
}