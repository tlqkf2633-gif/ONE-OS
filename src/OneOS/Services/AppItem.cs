using System.ComponentModel;
using System.Windows.Media;
using OneOS.Core;

namespace OneOS.Services;

public sealed class AppItem : INotifyPropertyChanged
{
    public AppDefinition Definition { get; }
    public string Id => Definition.Id;
    public string Name => Definition.Name;
    public string Description => Definition.Description;
    public string Icon => Definition.Icon;
    public Brush Color { get; }
    private bool isOpen;
    private bool isActive;
    public bool IsOpen { get => isOpen; set { if (isOpen != value) { isOpen = value; OnChanged(nameof(IsOpen)); } } }
    public bool IsActive { get => isActive; set { if (isActive != value) { isActive = value; OnChanged(nameof(IsActive)); } } }
    public AppItem(AppDefinition definition)
    {
        Definition = definition;
        Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(definition.Color));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
