// Оновлення мнемосхеми: кожні 2 с запитує поточні значення датчиків
// з /Scheme/Values (JSON) і виводить їх в індикатори без перезавантаження сторінки.

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
            });
        } catch (e) {
            console.warn('Не вдалося оновити значення', e);
        }
    }

    refresh();
    setInterval(refresh, 2000);
})();
