using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MetronomeApp.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged {
    #region Members

    private readonly ILogger _logger;
    private readonly IAudioManager _audioManager;
    private IAudioPlayer _audioPlayer;
    private Stream _moveCursorSound;
    private Boolean _isPlaying = false;

    #endregion

    public ICommand PlayOrStopSoundCommand { get; private set; }


    public Boolean IsPlaying {
        get => _isPlaying;
        private set {
            if (_isPlaying != value) {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }
    }

    #region Constructor

    public MainPageViewModel(ILoggerFactory loggerFactory, IAudioManager audioManager) {
        _logger = loggerFactory.CreateLogger<MainPageViewModel>();
        _audioManager = audioManager;

        PlayOrStopSoundCommand = new Command(PlayOrStopSound);

        Task.Run(async () => {
            _moveCursorSound = await FileSystem.OpenAppPackageFileAsync("move_cursor1.mp3");
        });

        _logger.LogInformation("End of init MainPageViewModel");
    }

    #endregion

    private async void StartPlayLoop() {
        if (_audioPlayer == null) {
            _audioPlayer = _audioManager.CreatePlayer(_moveCursorSound);
        }

        while (IsPlaying) {
            if (_audioPlayer.IsPlaying) {
                _audioPlayer.Stop();
            }

            _audioPlayer.Play();

            await Task.Delay(500);
        }
    }

    private void PlayOrStopSound() {
        if (IsPlaying) {
            IsPlaying = false;
        } else {
            IsPlaying = true;
            StartPlayLoop();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
