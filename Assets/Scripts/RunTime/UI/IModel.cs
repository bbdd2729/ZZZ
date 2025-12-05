

using System;
using System.ComponentModel;

public class IModel
{
    event Action<IModel> OnDataChanged;
}