using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using System.Diagnostics;
using System.Windows.Input;

namespace MetronomeApp.ViewModels;

public class MainPageViewModel : ObservableObject {
    public class LightColors : ObservableObject {
        readonly IDictionary<int, Color> _lights = new Dictionary<int, Color>();

        public Color this[int index] {
            get { return _lights.ContainsKey(index) ? _lights[index] : Color.FromArgb("#5C4747"); }
            set {
                _lights[index] = value;
                OnPropertyChanged("Item[" + index + "]");
            }
        }
    }

    #region Members

    private readonly ILogger _logger;
    private readonly IAudioManager _audioManager;
    private IAudioPlayer _audioPlayer;
    private CancellationTokenSource _cancellationTokenSource;
    private Stream _moveCursorSound;
    private Boolean _isPlaying = false;
    private int _tempo = 120;
    private int _milliSecondsPerTick = 500;
    private LightColors _tickLights;
    private int _tickCounter = -1;

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

    public LightColors TickLights {
        get { return _tickLights ?? (_tickLights = new LightColors()); }
        set { SetProperty(ref _tickLights, value); }
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

    private void ResetAudioPlayer() {
        _audioPlayer = _audioManager.CreatePlayer(_moveCursorSound);
    }

    private void ResetTickCounter() {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _tickCounter = -1;
    }

    private void setMilliSecondsPerTickByTempo() {
        _milliSecondsPerTick = (int)(60000.0 / _tempo);
    }

    private async void Twinkle(int index) {
        TickLights[index] = Color.FromArgb("#EF462E");
        await Task.Delay(100);
        TickLights[index] = Color.FromArgb("#5C4747");
    }

    // オーディオデバイスが切り替わったり一時的に専有された場合にも再生を続けられるように
    private async void AutoRefreshAudioPlayer() {
        while (IsPlaying) {
            try {
                if (!_audioPlayer.IsPlaying) {
                    ResetAudioPlayer();
                }

                await Task.Delay(1000, _cancellationTokenSource.Token);
            } catch (TaskCanceledException) { // ResetTickCounter メソッドが呼ばれたときに Task.Delay が中断されここに来る
                ResetAudioPlayer();
            }
        }
    }

    private async Task WaitMilliSeconds(int milliSeconds) {
        int elapsedMilliseconds = 0;
        Stopwatch stopWatch = Stopwatch.StartNew();

        while (elapsedMilliseconds < milliSeconds) {
            elapsedMilliseconds = (int)stopWatch.ElapsedMilliseconds;
            _logger.LogInformation("elapsedMilliseconds: " + elapsedMilliseconds);
            await Task.Delay(1, _cancellationTokenSource.Token);
        }

        return;
    }

    private async void StartPlayLoop() {
        if (_audioPlayer == null) {
            ResetAudioPlayer();
        }

        AutoRefreshAudioPlayer();

        while (IsPlaying) {
            try {
                if (_audioPlayer.IsPlaying) {
                    _audioPlayer.Stop();
                }

                _audioPlayer.Play();

                _tickCounter = (_tickCounter + 1) % 4;
                Twinkle(_tickCounter);

                await WaitMilliSeconds(_milliSecondsPerTick);
            } catch (TaskCanceledException) { // ResetTickCounter メソッドが呼ばれたときに Task.Delay が中断されここに来る
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
}
