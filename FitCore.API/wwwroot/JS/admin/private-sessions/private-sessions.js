// private-sessions-admin.js

const STATUS_LABELS = ['scheduled', 'completed', 'cancelled'];

let allTrainers = [];
let allSessions = []; // flattened across trainers, each tagged with trainerName

document.addEventListener('DOMContentLoaded', () => {
    init();
    document.getElementById('createSessionBtn').addEventListener('click', createSession);
    document.getElementById('filterTrainer').addEventListener('change', renderTable);
    document.getElementById('filterStatus').addEventListener('change', renderTable);
});

function showMessage(text, type) {
    const banner = document.getElementById('msgBanner');
    banner.textContent = text;
    banner.className = `msg-banner show ${type}`;
    setTimeout(() => banner.classList.remove('show'), 4000);
}

async function init() {
    try {
        const data = await FitCoreApi.get('/api/Trainers?Page=1&Page_Size=50');
        allTrainers = data.data || data.Data || [];

        const options = allTrainers.map(t => {
            const id = pick(t, 'trainerID', 'TrainerID');
            const name = pick(t, 'fullName', 'FullName') || `Trainer #${id}`;
            return { id, name };
        });

        document.getElementById('trainerSelect').innerHTML = options.map(o => `<option value="${o.id}">${escapeHtml(o.name)}</option>`).join('');
        document.getElementById('filterTrainer').innerHTML = '<option value="">All Trainers</option>' + options.map(o => `<option value="${o.id}">${escapeHtml(o.name)}</option>`).join('');

        await loadAllSessions();
    } catch (error) {
        showMessage(`Couldn't load trainers: ${error.message}`, 'error');
    }
}

// There's no single "get all private sessions" endpoint, so we aggregate real per-trainer
// results (GET /api/PrivateSessions/trainer/{id}) across every trainer.
async function loadAllSessions() {
    const tbody = document.getElementById('sessionsTableBody');
    tbody.innerHTML = `<tr class="state-row"><td colspan="6">Loading sessions…</td></tr>`;

    try {
        const results = await Promise.all(allTrainers.map(async t => {
            const id = pick(t, 'trainerID', 'TrainerID');
            const name = pick(t, 'fullName', 'FullName') || `Trainer #${id}`;
            try {
                const sessions = await FitCoreApi.get(`/api/PrivateSessions/trainer/${id}`);
                return (sessions || []).map(s => ({ ...s, __trainerName: name, __trainerId: id }));
            } catch {
                return [];
            }
        }));

        allSessions = results.flat();
        renderTable();
    } catch (error) {
        tbody.innerHTML = `<tr class="state-row is-error"><td colspan="6">Couldn't load sessions: ${escapeHtml(error.message)}</td></tr>`;
    }
}

function renderTable() {
    const tbody = document.getElementById('sessionsTableBody');
    const trainerFilter = document.getElementById('filterTrainer').value;
    const statusFilter = document.getElementById('filterStatus').value;

    const filtered = allSessions.filter(s => {
        if (trainerFilter && String(s.__trainerId) !== trainerFilter) return false;
        if (statusFilter !== '' && String(Number(pick(s, 'status', 'Status'))) !== statusFilter) return false;
        return true;
    }).sort((a, b) => new Date(pick(b, 'sessionDate', 'SessionDate')) - new Date(pick(a, 'sessionDate', 'SessionDate')));

    if (filtered.length === 0) {
        tbody.innerHTML = `<tr class="state-row"><td colspan="6">No private sessions found.</td></tr>`;
        return;
    }

    tbody.innerHTML = filtered.map(renderRow).join('');
    wireActions();
}

function renderRow(s) {
    const id = pick(s, 'privateSessionID', 'PrivateSessionID');
    const memberName = pick(s, 'memberName', 'MemberName') || `Member #${pick(s, 'memberUserId', 'MemberUserId')}`;
    const sessionDate = (pick(s, 'sessionDate', 'SessionDate') || '').toString().substring(0, 10);
    const start = (pick(s, 'startTime', 'StartTime') || '').toString().substring(0, 5);
    const end = (pick(s, 'endTime', 'EndTime') || '').toString().substring(0, 5);
    const status = Number(pick(s, 'status', 'Status'));
    const statusLabel = STATUS_LABELS[status] || 'unknown';

    return `
    <tr>
        <td>${escapeHtml(s.__trainerName)}</td>
        <td>${escapeHtml(memberName)}</td>
        <td>${sessionDate}</td>
        <td>${start} - ${end}</td>
        <td><span class="pill ${statusLabel}">${statusLabel.charAt(0).toUpperCase() + statusLabel.slice(1)}</span></td>
        <td class="row-actions">
            ${status === 0 ? `
                <button class="btn-outline btn-sm" data-complete="${id}">Complete</button>
                <button class="btn-outline btn-sm" data-cancel="${id}">Cancel</button>
            ` : '—'}
        </td>
    </tr>`;
}

function wireActions() {
    document.querySelectorAll('[data-complete]').forEach(btn => btn.addEventListener('click', () => updateSession(btn.dataset.complete, 'complete')));
    document.querySelectorAll('[data-cancel]').forEach(btn => btn.addEventListener('click', () => updateSession(btn.dataset.cancel, 'cancel')));
}

async function updateSession(id, action) {
    try {
        await FitCoreApi.patch(`/api/PrivateSessions/${id}/${action}`);
        showMessage(`Session marked as ${action === 'complete' ? 'completed' : 'cancelled'}.`, 'success');
        await loadAllSessions();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

async function createSession() {
    const dto = {
        trainerID: parseInt(document.getElementById('trainerSelect').value, 10),
        memberUserId: parseInt(document.getElementById('memberUserId').value, 10),
        sessionDate: document.getElementById('sessionDate').value,
        startTime: document.getElementById('startTime').value + ':00',
        endTime: document.getElementById('endTime').value + ':00',
        notes: document.getElementById('notes').value,
    };

    try {
        await FitCoreApi.post('/api/PrivateSessions', dto);
        showMessage('Private session scheduled.', 'success');
        document.getElementById('memberUserId').value = '';
        document.getElementById('notes').value = '';
        await loadAllSessions();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str ?? '';
    return div.innerHTML;
}
