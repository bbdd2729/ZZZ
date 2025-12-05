using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;


public abstract class BaseModel : IModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    
    
    
    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public virtual void Load(object args = null)
    {

    }    
    
    public virtual void Dispose()
    {
        PropertyChanged = null;
    }
}