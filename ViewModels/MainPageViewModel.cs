using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MetronomeApp.ViewModels;

class MainPageViewModel : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
