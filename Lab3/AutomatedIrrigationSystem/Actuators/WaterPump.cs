using System;

namespace AutomatedIrrigationSystem
{
    // --- КЛАС НАСОСНОЇ СТАНЦІЇ ---
    public class WaterPump
    {
        public string PumpId { get; private set; }
        public PumpState State { get; private set; }

        public WaterPump(string pumpId)
        {
            PumpId = pumpId;
            State = PumpState.Off;
        }

        public void Start()
        {
            State = PumpState.Running;
            Console.WriteLine($"[Насос {PumpId}] УВІМКНЕНО. Підтримання тиску в магістралі.");
        }

        public void Stop()
        {
            State = PumpState.Off;
            Console.WriteLine($"[Насос {PumpId}] ВИМКНЕНО.");
        }

        // Аварійна зупинка: насос переходить у стан Fault до ручного скидання
        public void EmergencyStop(string reason)
        {
            State = PumpState.Fault;
            Console.WriteLine($"[Насос {PumpId}] АВАРІЙНА ЗУПИНКА: {reason}");
        }

        public void ResetFault()
        {
            if (State == PumpState.Fault)
            {
                State = PumpState.Off;
                Console.WriteLine($"[Насос {PumpId}] Аварію скинуто оператором.");
            }
        }
    }
}
