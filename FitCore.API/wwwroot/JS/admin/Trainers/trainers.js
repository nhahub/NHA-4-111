// trainers.js

const DAYS = [
    { label: 'Sunday', value: 0 },
    { label: 'Monday', value: 1 },
    { label: 'Tuesday', value: 2 },
    { label: 'Wednesday', value: 3 },
    { label: 'Thursday', value: 4 },
    { label: 'Friday', value: 5 },
    { label: 'Saturday', value: 6 },
];

const ROLE_VALUES = { Trainer: 1, Receptionist: 3 };

let currentPage = 1;
let currentSearch = '';
let allTrainers = [];

document.addEventListener('DOMContentLoaded', () => {
    fetchTrainers();

    document.getElementById('staffRole').addEventListener('change', toggleTrainerOnlyFields);
    document.getElementById('createStaffBtn').addEventListener('click', createStaff);
    document.getElementById('addWorkingHourRowBtn').addEventListener('click', () => addWorkingHourRow());
    document.getElementById('saveWorkingHoursBtn').addEventListener('click', saveWorkingHours);
    document.getElementById('applyBtn').addEventListener('click', () => {
        currentSearch = document.getElementById('searchInput').value.trim().toLowerCase();
        currentPage = 1;
        renderTrainersTable();
    });
    document.getElementById('workingHoursTrainerSelect').addEventListener('change', loadWorkingHoursForSelectedTrainer);

    toggleTrainerOnlyFields();
    addWorkingHourRow();
});

function toggleTrainerOnlyFields() {
    const role = document.getElementById('staffRole').value;
    document.getElementById('trainerOnlyFields').style.display = role === 'Trainer' ? 'grid' : 'none';
}

function showMessage(text, type) {
    const banner = document.getElementById('msgBanner');
    banner.textContent = text;
    banner.className = `msg-banner show ${type}`;
    setTimeout(() => banner.classList.remove('show'), 4000);
}

async function createStaff() {
    const role = document.getElementById('staffRole').value;
    const dto = {
        fullName: document.getElementById('staffFullName').value,
        email: document.getElementById('staffEmail').value,
        phoneNumber: document.getElementById('staffPhone').value,
        password: document.getElementById('staffPassword').value,
        role: ROLE_VALUES[role],
        specialization: document.getElementById('staffSpecialization').value,
        bio: document.getElementById('staffBio').value,
    };

    try {
        const response = await fetch('/api/Trainers/staff', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });

        if (!response.ok) {
            const err = await response.json().catch(() => ({}));
            throw new Error(err.message || err.Message || `Request failed (${response.status})`);
        }

        showMessage(`${role} profile created successfully.`, 'success');
        document.getElementById('staffFullName').value = '';
        document.getElementById('staffEmail').value = '';
        document.getElementById('staffPhone').value = '';
        document.getElementById('staffPassword').value = '';
        document.getElementById('staffSpecialization').value = '';
        document.getElementById('staffBio').value = '';

        fetchTrainers();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function addWorkingHourRow(day, start, end) {
    const container = document.getElementById('workingHoursRows');
    const row = document.createElement('div');
    row.className = 'schedule-row';

    const dayOptions = DAYS.map(d => `<option value="${d.value}" ${d.value === day ? 'selected' : ''}>${d.label}</option>`).join('');

    row.innerHTML = `
        <div class="form-group">
            <label>Day</label>
            <select class="wh-day">${dayOptions}</select>
        </div>
        <div class="form-group">
            <label>Start Time</label>
            <input type="time" class="text-input wh-start" value="${start || '09:00'}">
        </div>
        <div class="form-group">
            <label>End Time</label>
            <input type="time" class="text-input wh-end" value="${end || '17:00'}">
        </div>
        <button class="btn-outline btn-sm remove-row">Remove</button>
    `;

    row.querySelector('.remove-row').addEventListener('click', () => row.remove());
    container.appendChild(row);
}

async function loadWorkingHoursForSelectedTrainer() {
    const trainerId = document.getElementById('workingHoursTrainerSelect').value;
    if (!trainerId) return;

    document.getElementById('workingHoursRows').innerHTML = '';

    try {
        const response = await fetch(`/api/Trainers/${trainerId}/working-hours`);
        if (!response.ok) throw new Error('Failed to load working hours');
        const hours = await response.json();

        if (!hours || hours.length === 0) {
            addWorkingHourRow();
            return;
        }

        hours.forEach(h => {
            const day = h.day ?? h.Day;
            const start = (h.startTime ?? h.StartTime || '').toString().substring(0, 5);
            const end = (h.endTime ?? h.EndTime || '').toString().substring(0, 5);
            addWorkingHourRow(day, start, end);
        });
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

async function saveWorkingHours() {
    const trainerId = document.getElementById('workingHoursTrainerSelect').value;
    if (!trainerId) {
        showMessage('Please select a trainer first.', 'error');
        return;
    }

    const rows = document.querySelectorAll('#workingHoursRows .schedule-row');
    const workingHours = Array.from(rows).map(row => ({
        day: parseInt(row.querySelector('.wh-day').value, 10),
        startTime: row.querySelector('.wh-start').value + ':00',
        endTime: row.querySelector('.wh-end').value + ':00',
    }));

    try {
        const response = await fetch(`/api/Trainers/${trainerId}/working-hours`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ workingHours })
        });

        if (!response.ok) {
            const err = await response.json().catch(() => ({}));
            throw new Error(err.message || err.Message || `Request failed (${response.status})`);
        }

        showMessage('Working hours saved.', 'success');
        fetchTrainers();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

async function fetchTrainers() {
    const tbody = document.getElementById('tableBody');
    tbody.innerHTML = `<tr class="state-row"><td colspan="5"><span class="state-title">Loading trainers…</span></td></tr>`;

    try {
        const response = await fetch('/api/Trainers?Page=1&Page_Size=50');
        if (!response.ok) throw new Error(`Request failed with status ${response.status}`);

        const data = await response.json();
        allTrainers = data.data || data.Data || [];

        populateTrainerSelect();
        renderTrainersTable();
    } catch (error) {
        tbody.innerHTML = `<tr class="state-row is-error"><td colspan="5"><span class="state-title">Couldn't load trainers</span></td></tr>`;
        console.error(error);
    }
}

function populateTrainerSelect() {
    const select = document.getElementById('workingHoursTrainerSelect');
    const previousValue = select.value;
    select.innerHTML = allTrainers.map(t => {
        const name = t.fullName || t.FullName || `Trainer #${t.trainerID || t.TrainerID}`;
        const id = t.trainerID ?? t.TrainerID;
        return `<option value="${id}">${escapeHtml(name)}</option>`;
    }).join('');

    if (previousValue) select.value = previousValue;
    if (select.value) loadWorkingHoursForSelectedTrainer();
}

function renderTrainersTable() {
    const tbody = document.getElementById('tableBody');
    const pageSize = 10;

    const filtered = allTrainers.filter(t => {
        if (!currentSearch) return true;
        const name = (t.fullName || t.FullName || '').toLowerCase();
        const spec = (t.specialization || t.Specialization || '').toLowerCase();
        return name.includes(currentSearch) || spec.includes(currentSearch);
    });

    if (filtered.length === 0) {
        tbody.innerHTML = `<tr class="state-row"><td colspan="5"><span class="state-title">No trainers found</span></td></tr>`;
        document.getElementById('paginationControls').innerHTML = '';
        return;
    }

    const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
    if (currentPage > totalPages) currentPage = totalPages;

    const pageItems = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize);

    tbody.innerHTML = pageItems.map(t => {
        const id = t.trainerID ?? t.TrainerID;
        const name = t.fullName || t.FullName || '—';
        const email = t.email || t.Email || '—';
        const spec = t.specialization || t.Specialization || '—';
        const hours = (t.workingHours || t.WorkingHours || []);
        const hoursText = hours.length
            ? hours.map(h => `${DAYS[h.day ?? h.Day]?.label.substring(0, 3)} ${(h.startTime ?? h.StartTime || '').toString().substring(0, 5)}-${(h.endTime ?? h.EndTime || '').toString().substring(0, 5)}`).join(', ')
            : 'Not set';

        return `<tr>
            <td>#${id}</td>
            <td>${escapeHtml(name)}</td>
            <td>${escapeHtml(email)}</td>
            <td>${escapeHtml(spec)}</td>
            <td>${escapeHtml(hoursText)}</td>
        </tr>`;
    }).join('');

    renderPagination(totalPages);
}

function renderPagination(totalPages) {
    const container = document.getElementById('paginationControls');
    container.innerHTML = '';
    if (totalPages <= 1) return;

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.innerText = i;
        if (i === currentPage) btn.classList.add('active');
        btn.addEventListener('click', () => { currentPage = i; renderTrainersTable(); });
        container.appendChild(btn);
    }
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str ?? '';
    return div.innerHTML;
}
