using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Client.Avalonia.ViewModels;

namespace Client.Avalonia
{
  public class ViewLocator : IDataTemplate
  {
    public IControl Build(object data)
    {
      var name = data.GetType().FullName.Replace("ViewModel", "View");
      var type = Type.GetType(name);

        return type is null ? 
          new TextBlock { Text = "Not Found: " + name } : 
          Activator.CreateInstance(type) as Control;
    }

    public bool Match(object data)
    {
      return data is ViewModelBase;
    }
  }
}