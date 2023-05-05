using Microsoft.Extensions.Logging;

namespace MetronomeApp;

public partial class MainPage : ContentPage
{
	int count = 0;
    private readonly ILogger logger;

	public MainPage(ILoggerFactory loggerFactory)
	{
		InitializeComponent();

        logger = loggerFactory.CreateLogger<MainPage>();
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

    private void OnPlaySoundButtonClicked(object sender, EventArgs e)
    {
        logger.LogDebug("aaaDebug");
        logger.LogInformation("aaaInfo");
        logger.LogWarning("aaaWarn");
        logger.LogError("aaaError");
    }
}
