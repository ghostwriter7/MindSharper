using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MindSharper.Presentation.App.State;

public class LayoutState : INotifyPropertyChanged
{
    private bool _isDarkMode = true;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode == value) return;
            _isDarkMode = value;
            OnPropertyChanged(nameof(IsDarkMode));
        }
    }

    private bool _isDrawerOpen;

    public bool IsDrawerOpen
    {
        get => _isDrawerOpen;
        set
        {
            if (_isDrawerOpen == value) return;
            _isDrawerOpen = value;
            OnPropertyChanged(nameof(IsDrawerOpen));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}