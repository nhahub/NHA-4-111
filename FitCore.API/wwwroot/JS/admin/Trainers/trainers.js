// trainers-management.js

const DAY_LABELS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const DAY_LABELS_FULL = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

let allTrainers = [];

document.addEventListener('DOMContentLoaded', () => {
    loadTrainers();

    document.getElementById('createStaffBtn').addEventListener('click', createStaff);
    document.getElementById('staffRole').addEventListener('change', toggleTrainerOnlyFields);
    document.getElementById('addWorkingHourRowBtn').addEventListener('click', () => addWorkingHourRow());
    document.getElementById('saveWorkingHoursBtn').addEventListener('click', saveWorkingHours);
    document.getElementById('workingHoursTrainerSelect').addEventListener('change', loadWorkingHoursForSelected);

    toggleTrainerOnlyFields();
    addWorkingHourRow();
});

function showMessage(text, type) {
    const banner = document.getElementById('msgBanner');
    banner.textContent = text;
    banner.className = `msg-banner show ${type}`;
    setTimeout(() => banner.classList.remove('show'), 4000);
}

function toggleTrainerOnlyFields() {
    document.getElementById('trainerOnlyFields').style.display = document.getElementById('staffRole').value === '1' ? 'grid' : 'none';
}

async function createStaff() {
    const dto = {
        fullName: document.getElementById('staffFullName').value,
        email: document.getElementById('staffEmail').value,
        phoneNumber: document.getElementById('staffPhone').value,
        password: document.getElementById('staffPassword').value,
        role: parseInt(document.getElementById('staffRole').value, 10),
        specialization: document.getElementById('staffSpecialization').value,
        bio: document.getElementById('staffBio').value,
    };

    try {
        await FitCoreApi.post('/api/Trainers/staff', dto);
        showMessage('Profile created.', 'success');
        ['staffFullName', 'staffEmail', 'staffPhone', 'staffPassword', 'staffSpecialization', 'staffBio'].forEach(id => document.getElementById(id).value = '');
        await loadTrainers();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

async function loadTrainers() {
    const tbody = document.getElementById('trainersTableBody');
    tbody.innerHTML = `<tr class="state-row"><td colspan="5">Loading trainers…</td></tr>`;

    try {
        const data = await FitCoreApi.get('/api/Trainers?Page=1&Page_Size=50');
        allTrainers = data.data || data.Data || [];

        populateTrainerSelect();
        renderTrainersTable();
    } catch (error) {
        tbody.innerHTML = `<tr class="state-row is-error"><td colspan="5">Couldn't load trainers: ${escapeHtml(error.message)}</td></tr>`;
    }
}

function populateTrainerSelect() {
    const select = document.getElementById('workingHoursTrainerSelect');
    const previous = select.value;
    select.innerHTML = allTrainers.map(t => {
        const id = pick(t, 'trainerID', 'TrainerID');
        const name = pick(t, 'fullName', 'FullName') || `Trainer #${id}`;
        return `<option value="${id}">${escapeHtml(name)}</option>`;
    }).join('');
    if (previous) select.value = previous;
    if (select.value) loadWorkingHoursForSelected();
}

function renderTrainersTable() {
    const tbody = document.getElementById('trainersTableBody');
    if (allTrainers.length === 0) {
        tbody.innerHTML = `<tr class="state-row"><td colspan="5">No trainers yet.</td></tr>`;
        return;
    }

    tbody.innerHTML = allTrainers.map(t => {
        const id = pick(t, 'trainerID', 'TrainerID');
        const name = pick(t, 'fullName', 'FullName') || '—';
        const email = pick(t, 'email', 'Email') || '—';
        const spec = pick(t, 'specialization', 'Specialization') || '—';
        const hours = pick(t, 'workingHours', 'WorkingHours') || [];
        const hoursText = hours.length
            ? hours.map(h => `${DAY_LABELS[Number(pick(h, 'day', 'Day'))]} ${(pick(h, 'startTime', 'StartTime') || '').toString().substring(0, 5)}-${(pick(h, 'endTime', 'EndTime') || '').toString().substring(0, 5)}`).join(', ')
            : 'Not set';

        return `<tr><td>#${id}</td><td>${escapeHtml(name)}</td><td>${escapeHtml(email)}</td><td>${escapeHtml(spec)}</td><td>${escapeHtml(hoursText)}</td></tr>`;
    }).join('');
}

async function loadWorkingHoursForSelected() {
    const trainerId = document.getElementById('workingHoursTrainerSelect').value;
    if (!trainerId) return;

    document.getElementById('workingHoursRows').innerHTML = '';
    try {
        const hours = await FitCoreApi.get(`/api/Trainers/${trainerId}/working-hours`);
        if (!hours || hours.length === 0) {
            addWorkingHourRow();
        } else {
            hours.forEach(h => addWorkingHourRow(
                Number(pick(h, 'day', 'Day')),
                (pick(h, 'startTime', 'StartTime') || '').toString().substring(0, 5),
                (pick(h, 'endTime', 'EndTime') || '').toString().substring(0, 5),
            ));
        }
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function addWorkingHourRow(day, start, end) {
    const container = document.getElementById('workingHoursRows');
    const row = document.createElement('div');
    row.className = 'schedule-row';
    const dayOptions = DAY_LABELS_FULL.map((label, value) => `<option value="${value}" ${value === day ? 'selected' : ''}>${label}</option>`).join('');

    row.innerHTML = `
        <div class="form-group"><label>Day</label><select class="wh-day">${dayOptions}</select></div>
        <div class="form-group"><label>Start</label><input type="time" class="text-input wh-start" value="${start || '09:00'}"></div>
        <div class="form-group"><label>End</label><input type="time" class="text-input wh-end" value="${end || '17:00'}"></div>
        <button class="btn-outline btn-sm remove-row">Remove</button>
    `;
    row.querySelector('.remove-row').addEventListener('click', () => row.remove());
    container.appendChild(row);
}

async function saveWorkingHours() {
    const trainerId = document.getElementById('workingHoursTrainerSelect').value;
    if (!trainerId) { showMessage('Select a trainer first.', 'error'); return; }

    const rows = document.querySelectorAll('#workingHoursRows .schedule-row');
    const workingHours = Array.from(rows).map(row => ({
        day: parseInt(row.querySelector('.wh-day').value, 10),
        startTime: row.querySelector('.wh-start').value + ':00',
        endTime: row.querySelector('.wh-end').value + ':00',
    }));

    try {
        await FitCoreApi.put(`/api/Trainers/${trainerId}/working-hours`, { workingHours });
        showMessage('Working hours saved.', 'success');
        await loadTrainers();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str ?? '';
    return div.innerHTML;
}
