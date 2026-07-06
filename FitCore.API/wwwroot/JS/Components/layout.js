async function loadComponent(elementId, componentPath) {
    try {
        const response = await fetch(componentPath);
        const html = await response.text();
        document.getElementById(elementId).innerHTML = html;
    } catch (error) {
        console.error(`Error loading ${componentPath}:`, error);
    }
}

function setActiveSidebarLink() {
    const currentPage = document.body.dataset.page;
    if (!currentPage) return;

    // بنحول اسم الصفحة لحروف صغيرة
    const currentPageLower = currentPage.toLowerCase();

    document.querySelectorAll('.sidebar-nav ul li').forEach(item => {
        // بنحول اسم الزرار كمان لحروف صغيرة ونقارنهم ببعض
        if (item.dataset.page) {
            const itemPageLower = item.dataset.page.toLowerCase();
            item.classList.toggle('active', itemPageLower === currentPageLower);
        }
    });
}

document.addEventListener("DOMContentLoaded", () => {
    fetch('/HTML/Components/sidebar.html')
        .then(response => response.text())
        .then(html => {
            document.getElementById('sidebar-container').innerHTML = html;
            setActiveSidebarLink();
        })
        .catch(error => console.error('Error loading sidebar:', error));

    loadComponent('header-container', '/HTML/Components/header.html');
});