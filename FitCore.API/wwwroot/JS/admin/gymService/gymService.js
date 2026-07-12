const API_URL = '/api/GymServices';
let page = 1;
const pageSize = 10;
let searchTerm = '';
let category = '';
let modalInst = null;

const categories = { 0: 'Memberships', 1: 'Personal Training', 2: 'Spa & Recovery', 3: 'Special Workshops' };

document.addEventListener('DOMContentLoaded', () => {
    modalInst = new bootstrap.Modal(document.getElementById('adminCrudModal'));
    loadAdminTable();

    document.getElementById('adminSearchInput').addEventListener('input', (e) => {
        searchTerm = e.target.value.trim();
        page = 1;
        loadAdminTable();
    });

    document.getElementById('adminCategoryFilter').addEventListener('change', (e) => {
        category = e.target.value;
        page = 1;
        loadAdminTable();
    });

    document.getElementById('adminBtnPrev').addEventListener('click', () => { if (page > 1) { page--; loadAdminTable(); } });
    document.getElementById('adminBtnNext').addEventListener('click', () => { page++; loadAdminTable(); });

    document.getElementById('adminBlueprintForm').addEventListener('submit', commitFormSubmission);
});

async function loadAdminTable() {
    let url = `${API_URL}?page=${page}&pageSize=${pageSize}`;
    if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    if (category !== '') url += `&category=${category}`;

    try {
        const response = await fetch(url);
        const result = await response.json();

        const data = result.data || result;
        const totalCount = result.totalCount || data.length;
        const totalPages = result.totalPages || Math.ceil(totalCount / pageSize);

        document.getElementById('adminTotalCount').textContent = totalCount;
        renderTableRows(data);

        document.getElementById('adminPaginationText').textContent = `Showing page ${page} of ${totalPages} (${totalCount} records)`;
        document.getElementById('adminBtnPrev').disabled = (page === 1);
        document.getElementById('adminBtnNext').disabled = (page >= totalPages || totalPages === 0);
    } catch (err) {
        console.error("Failed to fetch admin registry:", err);
    }
}

function renderTableRows(services) {
    const tbody = document.getElementById('adminTableBody');
    tbody.innerHTML = '';

    if (!services || services.length === 0) {
        tbody.innerHTML = `<tr><td colspan="7" class="text-center py-4 text-muted">No records matched the system parameters.</td></tr>`;
        return;
    }

    services.forEach(service => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td class="ps-4 text-muted fw-bold">#${service.serviceID}</td>
            <td class="fw-bold">${service.name}</td>
            <td><span class="badge bg-light text-dark border px-2.5 py-1.5">${categories[service.category]}</span></td>
            <td class="fw-bold text-primary">${parseFloat(service.price).toFixed(2)} EGP</td>
            <td>${service.durationInDays} Days</td>
            <td>${service.allowedSessionsCount} Sessions</td>
            <td class="text-end pe-4">
                <button class="btn btn-sm text-primary me-2 border-0" onclick='openCrudModal(true, ${JSON.stringify(service)})'><i class='bx bx-edit fs-5'></i></button>
                <button class="btn btn-sm text-danger border-0" onclick="deleteServiceAsset(${service.serviceID})"><i class='bx bx-trash fs-5'></i></button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function openCrudModal(isEdit = false, data = null) {
    document.getElementById('adminBlueprintForm').reset();

    if (isEdit && data) {
        document.getElementById('modalHeadingTitle').textContent = `Modify Blueprint #${data.serviceID}`;
        document.getElementById('serviceIdField').value = data.serviceID;
        document.getElementById('formName').value = data.name;
        document.getElementById('formCategory').value = data.category;
        document.getElementById('formPrice').value = data.price;
        document.getElementById('formDuration').value = data.durationInDays;
        document.getElementById('formSessions').value = data.allowedSessionsCount;
        document.getElementById('formSubmitBtn').textContent = "Update Structure";
    } else {
        document.getElementById('modalHeadingTitle').textContent = "Deploy New Service Package";
        document.getElementById('serviceIdField').value = '';
        document.getElementById('formSubmitBtn').textContent = "Deploy Package";
    }
    modalInst.show();
}

async function commitFormSubmission(e) {
    e.preventDefault();
    const id = document.getElementById('serviceIdField').value;
    const isEdit = id !== '';

    const payload = {
        name: document.getElementById('formName').value.trim(),
        price: parseFloat(document.getElementById('formPrice').value),
        category: parseInt(document.getElementById('formCategory').value),
        durationInDays: parseInt(document.getElementById('formDuration').value),
        allowedSessionsCount: parseInt(document.getElementById('formSessions').value)
    };

    const url = isEdit ? `${API_URL}/${id}` : API_URL;
    const method = isEdit ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error('API Execution Error');

        modalInst.hide();
        loadAdminTable();
    } catch (err) {
        alert(err.message);
    }
}

async function deleteServiceAsset(id) {
    if (!confirm(`Wipe blueprint asset #${id} from the server?`)) return;
    try {
        const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Deletion request rejected.');
        loadAdminTable();
    } catch (err) {
        alert(err.message);
    }
}