const API_URL = '/api/GymServices';
let page = 1;
const pageSize = 6;
let searchTerm = '';
let category = '';

const categories = { 0: 'Memberships', 1: 'Personal Training', 2: 'Spa & Recovery', 3: 'Special Workshops' };

document.addEventListener('DOMContentLoaded', () => {
    loadUserServices();

    document.getElementById('userSearchInput').addEventListener('input', (e) => {
        searchTerm = e.target.value.trim();
        page = 1;
        loadUserServices();
    });

    document.getElementById('userCategoryFilter').addEventListener('change', (e) => {
        category = e.target.value;
        page = 1;
        loadUserServices();
    });

    document.getElementById('userBtnPrev').addEventListener('click', () => { if (page > 1) { page--; loadUserServices(); } });
    document.getElementById('userBtnNext').addEventListener('click', () => { page++; loadUserServices(); });
});

async function loadUserServices() {
    let url = `${API_URL}?page=${page}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (category !== '') url += `&category=${category}`;

    try {
        const response = await fetch(url);
        const result = await response.json();

        const data = result.data || result;
        const totalCount = result.totalCount || data.length;
        const totalPages = result.totalPages || Math.ceil(totalCount / pageSize);

        renderCards(data);

        document.getElementById('userPaginationText').textContent = `Page ${page} of ${totalPages} (${totalCount} Available Options)`;
        document.getElementById('userBtnPrev').disabled = (page === 1);
        document.getElementById('userBtnNext').disabled = (page >= totalPages || totalPages === 0);
    } catch (err) {
        console.error("Failed to stream user workspace:", err);
    }
}

function renderCards(services) {
    const container = document.getElementById('userCardsContainer');
    container.innerHTML = '';

    if (!services || services.length === 0) {
        container.innerHTML = `<div class="text-center text-muted py-5 w-100"><i class='bx bx-layer-minus fs-2 d-block mb-2'></i>No service tiers available under this scope.</div>`;
        return;
    }

    services.forEach((service, index) => {
        const col = document.createElement('div');
        const isFirst = index === 0;
        col.className = `${isFirst ? "col-12 col-md-6 col-lg-7" :"col-12 col-md-6 col-lg-4"} `;


        col.innerHTML = `
        ${isFirst ? `
            <div class="d-flex gap-4 border p-2 rounded border-primary">
                <div class="position-relative">
                    <img src="/Images/vip.png" alt="Gym Image" class="img-fluid rounded" style="max-height:350px;"/>
                </div>
                <div class=" ${isFirst ? 'featured' : ''}">
                    <span class="card-badge">${categories[service.category] || 'General'}</span>
                    <h3 class="fw-bold m-0 text-dark">${service.name}</h3>
                    <div class="card-price">
                        ${parseFloat(service.price).toFixed(0)} <span class="text-muted">EGP</span>
                    </div>
                    <ul class="features-list">
                        <li><i class='bx bx-check-circle'></i> Membership cycle valid for <strong>${service.durationInDays} days</strong></li>
                        <li><i class='bx bx-check-circle'></i> Grants access to <strong>${service.allowedSessionsCount} sessions</strong></li>
                        <li><i class='bx bx-check-circle'></i> Instant check-in activation pipeline</li>
                    </ul>
                    <button class="btn ${isFirst ? 'btn-primary' : 'btn-outline-primary'} w-100 rounded-3 py-2 fw-semibold">
                        Purchase Plan
                    </button>
                </div>
            </div>
        `: `<div class="border px-4 pt-4 pb-2 rounded">
                <span class="card-badge">${categories[service.category] || 'General'}</span>
                <h3 class="fw-bold m-0 text-dark">${service.name}</h3>
                <div class="card-price">
                    ${parseFloat(service.price).toFixed(0)} <span class="text-muted">EGP</span>
                </div>
                <ul class="features-list">
                    <li><i class='bx bx-check-circle'></i> Membership cycle valid for <strong>${service.durationInDays} days</strong></li>
                    <li><i class='bx bx-check-circle'></i> Grants access to <strong>${service.allowedSessionsCount} sessions</strong></li>
                    <li><i class='bx bx-check-circle'></i> Instant check-in activation pipeline</li>
                </ul>
                <button class="btn ${isFirst ? 'btn-primary' : 'btn-outline-primary'} w-100 rounded-3 py-2 fw-semibold">
                    Purchase Plan
                </button>
            </div>`}
          
        `;
        container.appendChild(col);
    });
}

async function bookOccurrence(classID, btn) {
    const memberUserId = window.CURRENT_MEMBER_USER_ID || 1;
    const originalText = btn.textContent;
    btn.disabled = true;
    btn.textContent = 'Booking…';

    try {
        await FitCoreApi.post(`/api/Classes/book?memberUserId=${memberUserId}&classId=${parseInt(classID, 1)}`, {       
            classID: parseInt(classID, 10),
            // sessionDate,
        });
        showToast('You are booked in! Check My Bookings for details.');
        await loadOccurrences(true);
    } catch (error) {
        showMessage(error.message, 'error');
        btn.disabled = false;
        btn.textContent = originalText;
    }
}
