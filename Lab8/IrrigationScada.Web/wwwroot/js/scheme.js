// Оновлення мнемосхеми: кожні 2 с запитує поточні значення датчиків
// і підсвічує датчик червоним (клас 'alarm'), якщо значення перевищує data-max.

(function () {
    const sensors = document.querySelectorAll('.sensor[data-topic]');

    async function refresh() {
        try {
            const response = await fetch('/Scheme/Values', { cache: 'no-store' });
            if (!response.ok) return;
            const readings = await response.json();

            sensors.forEach(function (el) {
                const r = readings.find(function (x) { return x.topic === el.dataset.topic; });
                if (!r) return;

                el.querySelector('.sensor-value').textContent = Number(r.value).toFixed(2);

                const max = parseFloat(el.dataset.max);
                el.classList.toggle('alarm', r.value > max);
            });
        } catch (e) {
            console.warn('Не вдалося оновити значення', e);
        }
    }

    // Годинник і календар (з ЛР №7)
    function tick() {
        const now = new Date();
        document.getElementById('clock').textContent = now.toLocaleTimeString('uk-UA');
        document.getElementById('calendar').textContent = now.toLocaleDateString('uk-UA', {
            weekday: 'long', day: 'numeric', month: 'long', year: 'numeric'
        });
    }

    tick();
    refresh();
    setInterval(tick, 1000);
    setInterval(refresh, 2000);
})();
