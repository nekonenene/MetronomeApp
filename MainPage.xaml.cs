using Microsoft.Extensions.Logging;

using MetronomeApp.ViewModels;

namespace MetronomeApp;

public partial class MainPage : ContentPage {
    #region Members

    private readonly ILogger _logger;

    #endregion

    int count = 0;

    public MainPage(ILoggerFactory loggerFactory, MainPageViewModel mainPageViewModel) {
        InitializeComponent();

        _logger = loggerFactory.CreateLogger<MainPage>();

        BindingContext = mainPageViewModel;

        _logger.LogInformation("End of init MainPage");
    }

    private void OnCounterClicked(object sender, EventArgs e) {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}
