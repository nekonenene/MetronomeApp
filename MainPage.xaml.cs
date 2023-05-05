using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace MetronomeApp;

public partial class MainPage : ContentPage {
    #region Members

    private readonly ILogger _logger;
    private readonly IAudioManager _audioManager;
    private IAudioPlayer _audioPlayer;
    private Stream _moveCursorSound;

    #endregion

    int count = 0;

    public MainPage(ILoggerFactory loggerFactory, IAudioManager audioManager) {
        InitializeComponent();

        _logger = loggerFactory.CreateLogger<MainPage>();
        _audioManager = audioManager;

        Task.Run(async () => {
            _moveCursorSound = await FileSystem.OpenAppPackageFileAsync("move_cursor1.mp3");
        });
    }

    private void OnCounterClicked(object sender, EventArgs e) {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void PlayOrStopSound() {
        if (_audioPlayer == null) {
            _audioPlayer = _audioManager.CreatePlayer(_moveCursorSound);
        }

        if (_audioPlayer.IsPlaying) {
            _audioPlayer.Stop();
        }

        _audioPlayer.Play();
    }

    private void OnPlaySoundButtonClicked(object sender, EventArgs e) {
        _logger.LogDebug("aaaDebug");
        _logger.LogInformation("aaaInfo");
        _logger.LogWarning("aaaWarn");
        _logger.LogError("aaaError");

        PlayOrStopSound();
    }
}
