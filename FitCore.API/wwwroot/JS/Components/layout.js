// layout.js
async function loadComponent(elementId, componentPath) {
    try {
        const response = await fetch(componentPath);
        const html = await response.text();
        document.getElementById(elementId).innerHTML = html;
    } catch (error) {
        console.error(`Error loading ${componentPath}:`, error);
    }
}

// تحميل المكونات أول ما الصفحة تفتح
document.addEventListener('DOMContentLoaded', () => {
    loadComponent('sidebar-container', '/html/Components/sidebar.html');
    loadComponent('header-container', '/html/Components/header.html');
});