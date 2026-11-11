using System;

namespace AutomatedIrrigationSystem
{
    // --- СИСТЕМА ФЕРТИГАЦІЇ (ДОЗУВАННЯ РІДКИХ ДОБРИВ У ПОЛИВНИЙ ПОТІК) ---
    public class FertigationSystem
    {
        public string UnitId { get; private set; }
        public FertigationState State { get; private set; }
        public double DoseRateLitersPerHour { get; set; } // норма внесення розчину

        public FertigationSystem(string unitId, double doseRateLitersPerHour)
        {
            UnitId = unitId;
            DoseRateLitersPerHour = doseRateLitersPerHour;
            State = FertigationState.Idle;
        }

        public void StartDosing()
        {
            State = FertigationState.Dosing;
            Console.WriteLine($"[Фертигація {UnitId}] Дозування добрив: {DoseRateLitersPerHour:F1} л/год.");
        }

        public void StopDosing()
        {
            State = FertigationState.Idle;
            Console.WriteLine($"[Фертигація {UnitId}] Дозування зупинено.");
        }
    }
}
