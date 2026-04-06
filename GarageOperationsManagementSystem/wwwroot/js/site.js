(function () {
    var btn = document.getElementById('dark-mode-toggle');
    if (!btn) return;

    function applyTheme(dark) {
        if (dark) {
            document.documentElement.classList.add('dark-mode');
            btn.textContent = '☀ Light';
        } else {
            document.documentElement.classList.remove('dark-mode');
            btn.textContent = '☾ Dark';
        }
    }

    var saved = localStorage.getItem('darkMode');
    applyTheme(saved === 'true');

    btn.addEventListener('click', function () {
        var isDark = document.documentElement.classList.contains('dark-mode');
        localStorage.setItem('darkMode', String(!isDark));
        applyTheme(!isDark);
    });
})();
