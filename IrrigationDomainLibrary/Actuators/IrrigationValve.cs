namespace IrrigationDomainLibrary.Actuators;

/// <summary>
/// Соленоїдний клапан сектора поливу.
/// </summary>
public class IrrigationValve
{
    public string Id { get; }
    public int SectorNumber { get; }
    public bool IsOpen { get; private set; }
    public DateTime? LastSwitched { get; private set; }

    public event Action<IrrigationValve>? StateChanged;

    public IrrigationValve(string id, int sectorNumber)
    {
        Id = id;
        SectorNumber = sectorNumber;
    }

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        LastSwitched = DateTime.Now;
        StateChanged?.Invoke(this);
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        LastSwitched = DateTime.Now;
        StateChanged?.Invoke(this);
    }

    public override string ToString() => $"{Id} (сектор {SectorNumber}): {(IsOpen ? "ВІДКРИТО" : "закрито")}";
}
