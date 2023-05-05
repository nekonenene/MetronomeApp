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

        // スライダーの min, max が Binding で決まる関係上、Value をここで明示的に設定してあげないとスライダーの初期位置がおかしくなる
        TempoSlider.Value = mainPageViewModel.Tempo;

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
