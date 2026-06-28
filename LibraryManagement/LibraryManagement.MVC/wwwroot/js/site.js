// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const storageKey = 'library-theme';
    const root = document.documentElement;
    const toggle = document.querySelector('[data-theme-toggle]');
    const icon = document.querySelector('[data-theme-icon]');

    if (!toggle || !icon) return;

    function preferredTheme() {
        const savedTheme = localStorage.getItem(storageKey);
        if (savedTheme === 'dark' || savedTheme === 'light') return savedTheme;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    function applyTheme(theme) {
        root.setAttribute('data-theme', theme);
        const isDark = theme === 'dark';
        toggle.setAttribute('aria-pressed', isDark.toString());
        toggle.setAttribute('aria-label', isDark ? 'Tắt chế độ tối' : 'Bật chế độ tối');
        icon.className = `bi ${isDark ? 'bi-moon-stars' : 'bi-brightness-high'} fs-5`;
    }

    applyTheme(preferredTheme());

    toggle?.addEventListener('click', () => {
        const nextTheme = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        localStorage.setItem(storageKey, nextTheme);
        applyTheme(nextTheme);
    });
})();
