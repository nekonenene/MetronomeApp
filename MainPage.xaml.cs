using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace MetronomeApp;

public partial class MainPage : ContentPage
{
	int count = 0;
    private readonly ILogger logger;
    private readonly IAudioManager audioManager;
    private IAudioPlayer audioPlayer;

    public MainPage(ILoggerFactory loggerFactory, IAudioManager audioManager)
	{
		InitializeComponent();

        logger = loggerFactory.CreateLogger<MainPage>();
        this.audioManager = audioManager;
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
        count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    private async void PlaySound()
    {
        if (audioPlayer == null)
        {
            audioPlayer = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("move_cursor1.mp3"));
        }

        audioPlayer.Play();
    }

    private void OnPlaySoundButtonClicked(object sender, EventArgs e)
    {
        logger.LogDebug("aaaDebug");
        logger.LogInformation("aaaInfo");
        logger.LogWarning("aaaWarn");
        logger.LogError("aaaError");

        PlaySound();
    }
}
