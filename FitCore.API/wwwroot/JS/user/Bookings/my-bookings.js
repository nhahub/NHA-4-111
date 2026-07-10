// my-bookings.js

const STATUS_LABELS = ['booked', 'cancelled', 'completed', 'missed']; // BookingStatus: Booked, Cancelled, Attended, NoShow
const CLASS_ICONS = [
    { match: /hiit|sprint|inferno/i, icon: 'bx-bolt' },
    { match: /yoga|flow|yin|zen/i, icon: 'bx-leaf' },
    { match: /spin|cycle|cycling/i, icon: 'bx-cycling' },
    { match: /lift|strength|power|core/i, icon: 'bx-transfer-alt' },
];

let allBookings = [];

document.addEventListener('DOMContentLoaded', () => {
    loadBookings();

    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            document.getElementById('upcomingTab').style.display = btn.dataset.tab === 'upcoming' ? 'block' : 'none';
            document.getElementById('pastTab').style.display = btn.dataset.tab === 'past' ? 'block' : 'none';
        });
    });

    document.getElementById('pastRangeFilter').addEventListener('change', renderPast);
});

function showMessage(text, type) {
    const banner = document.getElementById('msgBanner');
    banner.textContent = text;
    banner.className = `msg-banner show ${type}`;
    setTimeout(() => banner.classList.remove('show'), 4000);
}

function classIconFor(name) {
    const found = CLASS_ICONS.find(c => c.match.test(name || ''));
    return found ? found.icon : 'bx-body';
}

async function loadBookings() {
    const memberUserId = window.CURRENT_MEMBER_USER_ID;
    try {
        const data = await FitCoreApi.get(`/api/Classes/my-bookings?memberUserId=${memberUserId}`);
        allBookings = Array.isArray(data) ? data : (data.data || data.Data || []);
        renderStats();
        renderUpcoming();
        renderPast();
    } catch (error) {
        showMessage(`Couldn't load your bookings: ${error.message}`, 'error');
    }
}

function renderStats() {
    const totalBookings = allBookings.length;

    // Training hours: genuinely computed from each booking's schedule start/end time.
    const totalMinutes = allBookings.reduce((sum, b) => {
        const start = parseTimeToMinutes(pick(b, 'startTime', 'StartTime'));
        const end = parseTimeToMinutes(pick(b, 'endTime', 'EndTime'));
        return sum + Math.max(0, end - start);
    }, 0);

    const upcomingCount = allBookings.filter(b => isUpcoming(b)).length;

    document.getElementById('totalBookingsValue').textContent = totalBookings;
    document.getElementById('trainingHoursValue').textContent = (totalMinutes / 60).toFixed(1);
    document.getElementById('upcomingCountValue').textContent = upcomingCount;
}

function parseTimeToMinutes(t) {
    if (!t) return 0;
    const parts = t.toString().split(':');
    return (parseInt(parts[0], 10) || 0) * 60 + (parseInt(parts[1], 10) || 0);
}

function isUpcoming(b) {
    const status = Number(pick(b, 'status', 'Status'));
    const sessionDate = new Date((pick(b, 'sessionDate', 'SessionDate') || '').toString().substring(0, 10) + 'T00:00:00');
    const today = new Date(); today.setHours(0, 0, 0, 0);
    return status === 0 && sessionDate >= today; // Booked and not in the past
}

function renderUpcoming() {
    const upcoming = allBookings.filter(isUpcoming)
        .sort((a, b) => new Date(pick(a, 'sessionDate', 'SessionDate')) - new Date(pick(b, 'sessionDate', 'SessionDate')));

    const nextCard = document.getElementById('nextClassCard');
    const list = document.getElementById('upcomingList');

    if (upcoming.length === 0) {
        nextCard.innerHTML = `<div class="state-empty">No upcoming classes booked yet. <br><a href="/html/user/classes/classes-schedule.html">Browse classes →</a></div>`;
        list.innerHTML = '';
        return;
    }

    const next = upcoming[0];
    const rest = upcoming.slice(1);

    const sessionDate = new Date((pick(next, 'sessionDate', 'SessionDate') || '').toString().substring(0, 10) + 'T00:00:00');
    const className = pick(next, 'className', 'ClassName');
    const start = (pick(next, 'startTime', 'StartTime') || '').toString().substring(0, 5);
    const end = (pick(next, 'endTime', 'EndTime') || '').toString().substring(0, 5);

    nextCard.innerHTML = `
        <div class="next-class-media">
            <span class="next-class-tag">Next Class</span>
            <i class='bx ${classIconFor(className)}'></i>
        </div>
        <div class="next-class-body">
            <div class="next-class-title-row">
                <div class="next-class-title">${escapeHtml(className)}</div>
                <div class="next-class-date-badge">
                    <span class="month">${sessionDate.toLocaleDateString(undefined, { month: 'short' })}</span>
                    <span class="day">${sessionDate.getDate()}</span>
                </div>
            </div>
            <div class="next-class-meta">
                <span><i class='bx bx-time-five'></i> ${start} - ${end}</span>
            </div>
            <div class="next-class-actions">
                <button class="btn-primary" onclick="addToCalendar('${escapeHtml(className)}','${start}')">Add to Calendar</button>
                <button class="icon-btn-danger" title="Cancel booking" data-cancel="${pick(next, 'bookingID', 'BookingID')}"><i class='bx bx-x'></i></button>
            </div>
        </div>`;

    list.innerHTML = rest.map(b => {
        const bSessionDate = new Date((pick(b, 'sessionDate', 'SessionDate') || '').toString().substring(0, 10) + 'T00:00:00');
        const bStart = (pick(b, 'startTime', 'StartTime') || '').toString().substring(0, 5);
        const bClassName = pick(b, 'className', 'ClassName');
        return `
        <div class="upcoming-item">
            <div class="upcoming-item-icon"><i class='bx ${classIconFor(bClassName)}'></i></div>
            <div class="upcoming-item-info">
                <div class="upcoming-item-title">${escapeHtml(bClassName)}</div>
                <div class="upcoming-item-sub">${bSessionDate.toLocaleDateString(undefined, { weekday: 'long', month: 'short', day: 'numeric' })} • ${bStart}</div>
            </div>
            <button class="cancel-link" data-cancel="${pick(b, 'bookingID', 'BookingID')}">Cancel</button>
        </div>`;
    }).join('');

    document.querySelectorAll('[data-cancel]').forEach(btn => {
        btn.addEventListener('click', () => cancelBooking(btn.dataset.cancel));
    });
}

function renderPast() {
    const rangeDays = parseInt(document.getElementById('pastRangeFilter').value, 10);
    const cutoff = new Date(); cutoff.setDate(cutoff.getDate() - rangeDays);

    const past = allBookings.filter(b => !isUpcoming(b))
        .filter(b => new Date(pick(b, 'sessionDate', 'SessionDate')) >= cutoff)
        .sort((a, b) => new Date(pick(b, 'sessionDate', 'SessionDate')) - new Date(pick(a, 'sessionDate', 'SessionDate')));

    const tbody = document.getElementById('pastTableBody');
    if (past.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" class="state-empty">No past sessions in this range.</td></tr>`;
        return;
    }

    tbody.innerHTML = past.map(b => {
        const className = pick(b, 'className', 'ClassName');
        const sessionDate = new Date((pick(b, 'sessionDate', 'SessionDate') || '').toString().substring(0, 10) + 'T00:00:00');
        const status = Number(pick(b, 'status', 'Status'));
        const statusLabel = STATUS_LABELS[status] || 'unknown';
        const trainerName = pick(b, 'trainerName', 'TrainerName') || '—'; // may be absent depending on API version

        return `
        <tr>
            <td>
                <div class="class-details-cell">
                    <div class="class-details-icon"><i class='bx ${classIconFor(className)}'></i></div>
                    <div>
                        <div class="class-details-name">${escapeHtml(className)}</div>
                        <div class="class-details-date">${sessionDate.toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' })}</div>
                    </div>
                </div>
            </td>
            <td>${escapeHtml(trainerName)}</td>
            <td>${sessionDate.toLocaleDateString()}</td>
            <td><span class="status-dot ${statusLabel}">${statusLabel.charAt(0).toUpperCase() + statusLabel.slice(1)}</span></td>
            <td>—</td>
        </tr>`;
    }).join('');
}

async function cancelBooking(bookingId) {
    const memberUserId = window.CURRENT_MEMBER_USER_ID;
    try {
        await FitCoreApi.patch(`/api/Classes/bookings/${bookingId}/cancel?memberUserId=${memberUserId}`);
        showMessage('Booking cancelled.', 'success');
        await loadBookings();
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function addToCalendar(className, startTime) {
    showMessage(`"${className}" (${startTime}) — calendar file download isn't wired up yet.`, 'success');
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str ?? '';
    return div.innerHTML;
}
