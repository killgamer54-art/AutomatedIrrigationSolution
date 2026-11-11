namespace AutomatedIrrigationSystem
{
    // --- АБСТРАКТНИЙ БАЗОВИЙ КЛАС СЕНСОРА ---
    // Спільні властивості та поведінка для всіх датчиків системи зрошення.
    public abstract class Sensor
    {
        public string SensorId { get; private set; }
        public string Name { get; private set; }
        public string Unit { get; private set; }

        // Останнє виміряне значення (у симуляції задається програмно)
        protected double Value { get; set; }

        protected Sensor(string sensorId, string name, string unit, double initialValue)
        {
            SensorId = sensorId;
            Name = name;
            Unit = unit;
            Value = initialValue;
        }

        // Зчитування поточного значення з датчика
        public virtual double Read()
        {
            return Value;
        }

        public override string ToString()
        {
            return $"{Name} [{SensorId}]: {Value:F1} {Unit}";
        }
    }
}
