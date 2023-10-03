public partial class App : Application
{
    public App()
    {
        InitializeComponent(); 

        MainPage = new AppShell();
        
        var timer = new System.Timers.Timer(5 * 1000);
        timer.Elapsed += (sender, e) =>
        {
            Current.Dispatcher.Dispatch(async () =>
            {
                await Shell.Current.DisplayAlert("Alert", "This is an alert", "OK");
            });
        };

        timer.AutoReset = true;
        timer.Enabled = true;
    }
}
