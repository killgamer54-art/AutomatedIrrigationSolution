namespace AutomatedIrrigationSystem
{
    // --- КЛАС ДАТЧИКА ТИСКУ ---
    public class PressureSensor
    {
        public double PressureBar { get; private set; }

        public double ReadPressure()
        {
            return PressureBar;
        }

        public void SetPressure(double pressure)
        {
            PressureBar = pressure;
        }
    }
}
