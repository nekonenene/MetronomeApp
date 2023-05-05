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
    private CancellationTokenSource _cancellationTokenSource;
    private Stream _moveCursorSound;
    private Boolean _isPlaying = false;
    private int _tempo = 120;
    private int _milliSecondsPerTick = 500;

    #endregion

    public ICommand PlayOrStopSoundCommand { get; private set; }

    public double MinTempo {
        get => 10;
    }
    public double MaxTempo {
        get => 300; // TODO: 最大テンポはユーザー側で設定できるようにしたい
    }

    public Boolean IsPlaying {
        get => _isPlaying;
        private set {
            if (_isPlaying != value) {
                _isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged("PlayOrStopButtonText");
                OnPropertyChanged("PlayOrStopButtonBgColor");
            }
        }
    }

    public int Tempo {
        get => _tempo;
        set {
            _logger.LogInformation($"Tempo Value: {value}");

            if (value < MinTempo) value = (int)MinTempo;
            if (value > MaxTempo) value = (int)MaxTempo;

            if (_tempo != value) {
                _tempo = value;
                setMilliSecondsPerTickByTempo();
                ResetTickCounter();
                OnPropertyChanged();
            }
        }
    }

    public string PlayOrStopButtonText { get => _isPlaying ? "Stop Sound" : "Play Sound"; }
    public Color PlayOrStopButtonBgColor { get => _isPlaying ? Color.FromArgb("#EC2121") : Color.FromArgb("#172C90"); } // #172C90

    #region Constructor

    public MainPageViewModel(ILoggerFactory loggerFactory, IAudioManager audioManager) {
        _logger = loggerFactory.CreateLogger<MainPageViewModel>();
        _audioManager = audioManager;
        _cancellationTokenSource = new CancellationTokenSource();

        PlayOrStopSoundCommand = new Command(PlayOrStopSound);

        Task.Run(async () => {
            _moveCursorSound = await FileSystem.OpenAppPackageFileAsync("move_cursor1.mp3");
        });

        _logger.LogInformation("End of init MainPageViewModel");
    }

    #endregion

    private void setMilliSecondsPerTickByTempo() {
        _milliSecondsPerTick = (int)(60000.0 / _tempo);
    }

    private void ResetTickCounter() {
        _cancellationTokenSource.Cancel();
    }

    private async void StartPlayLoop() {
        if (_audioPlayer == null) {
            _audioPlayer = _audioManager.CreatePlayer(_moveCursorSound);
        }

        while (IsPlaying) {
            try {
                if (_audioPlayer.IsPlaying) {
                    _audioPlayer.Stop();
                }

                _audioPlayer.Play();

                await Task.Delay(_milliSecondsPerTick, _cancellationTokenSource.Token);
            } catch (TaskCanceledException) {
                _cancellationTokenSource = new CancellationTokenSource();
            }
        }
    }

    private void PlayOrStopSound() {
        if (IsPlaying) {
            IsPlaying = false;
            ResetTickCounter();
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
