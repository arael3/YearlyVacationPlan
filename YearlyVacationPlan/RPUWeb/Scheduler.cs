namespace RPU.RPUWeb;

public class Scheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public Scheduler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    // Metoda ExecuteAsync jest wywoływana asynchronicznie w tle. Metoda ta używa obiektu CancellationToken, który umożliwia przerwanie wykonywania metody w dowolnym momencie.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Obiekt PeriodicTimer działa jak zegar. Ustawiony jest na czas co dwie godziny.
        var timer = new PeriodicTimer(TimeSpan.FromHours(2));
        // Pętla while czeka na następne wywołanie metody WaitForNextTickAsync obiektu timer. Ta metoda zwraca wartość true, jeśli upłynął ustawiony czas lub jeśli anulowano token przerwania.
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            // Po każdym wywołaniu WaitForNextTickAsync, pobierana jest obecna godzina i tworzona jest nowa data docelowa z ustawioną godziną na 16:30. Obliczany jest czas różnicy między obecną datą a docelową datą, a następnie jeśli czas ten jest pomiędzy 0 a 120 minut, zostaje uruchomiona metoda RunJob(). Metoda RunJob może być implementacją dowolnego zadania, które ma zostać wykonane w ustalonych godzinach.
            DateTime currentTime = DateTime.Now;
            DateTime targetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 16, 30, 0);
            int difference = (int)(currentTime - targetTime).TotalMinutes;

            if (0 < difference && difference <= 120)
            {
                await RunJob();
            }
        }
    }

    private async Task RunJob()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            try
            {
                var sendEmailJob = scope.ServiceProvider.GetService<ISendEmailJob>();

                // Metoda SendEmailNotification wysyła mailowe przypomienie o nadchodzącym urlopie
                await Task.Run(() => sendEmailJob.SendEmailNotification());
            }
            catch (Exception)
            {
            }
        }
    }
}
