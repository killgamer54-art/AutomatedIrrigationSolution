using System;

namespace AutomatedIrrigationSystem
{
    // --- КЛАС ВИКОНАВЧОГО МЕХАНІЗМУ: ЕЛЕКТРОМАГНІТНИЙ КЛАПАН ---
    public class IrrigationValve
    {
        public string ZoneId { get; private set; }
        public ValveState State { get; private set; }

        public IrrigationValve(string zoneId)
        {
            ZoneId = zoneId;
            State = ValveState.Closed;
        }

        public void Open()
        {
            State = ValveState.Open;
            Console.WriteLine($"[Клапан {ZoneId}] Відкрито подачу води.");
        }

        public void Close()
        {
            State = ValveState.Closed;
            Console.WriteLine($"[Клапан {ZoneId}] Закрито.");
        }
    }
}
