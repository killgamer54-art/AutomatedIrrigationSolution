namespace IrrigationDomainLibrary.Actuators;

/// <summary>
/// Насос насосної станції зрошувальної системи.
/// </summary>
public class WaterPump
{
    public string Id { get; }
    public bool IsRunning { get; private set; }
    public bool IsFault { get; private set; }
    public TimeSpan TotalRunTime { get; private set; }

    public event Action<WaterPump>? StateChanged;

    public WaterPump(string id)
    {
        Id = id;
    }

    public void Start()
    {
        if (IsRunning || IsFault) return;
        IsRunning = true;
        StateChanged?.Invoke(this);
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        StateChanged?.Invoke(this);
    }

    /// <summary>Аварійна зупинка насоса (наприклад, через надмірний тиск).</summary>
    public void Trip()
    {
        IsFault = true;
        Stop();
    }

    public void ResetFault() => IsFault = false;

    /// <summary>Облік напрацювання за цикл сканування.</summary>
    public void Tick(TimeSpan cycle)
    {
        if (IsRunning) TotalRunTime += cycle;
    }

    public override string ToString() =>
        $"{Id}: {(IsFault ? "АВАРІЯ" : IsRunning ? "ПРАЦЮЄ" : "зупинено")}, напрацювання {TotalRunTime.TotalSeconds:F0} с";
}
