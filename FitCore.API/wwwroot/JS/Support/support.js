// support.js

document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('faqSearchInput');
    input.addEventListener('input', () => {
        const term = input.value.trim().toLowerCase();
        const items = document.querySelectorAll('.faq-item');
        let visibleCount = 0;

        items.forEach(item => {
            const keywords = item.dataset.keywords.toLowerCase();
            const text = item.textContent.toLowerCase();
            const matches = !term || keywords.includes(term) || text.includes(term);
            item.style.display = matches ? '' : 'none';
            if (matches) visibleCount++;
        });

        document.getElementById('noResultsMsg').classList.toggle('d-none', visibleCount > 0);
    });
});
