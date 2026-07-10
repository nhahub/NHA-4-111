// classes-schedule.js

const DAY_LABELS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const CLASS_ICONS = [
    { match: /hiit|sprint|inferno/i, icon: 'bx-bolt', color: 'orange', category: 'HIIT' },
    { match: /yoga|flow|yin|zen/i, icon: 'bx-leaf', color: 'indigo', category: 'Yoga' },
    { match: /spin|cycle|cycling/i, icon: 'bx-cycling', color: 'orange', category: 'Spin' },
    { match: /lift|strength|power|core/i, icon: 'bx-transfer-alt', color: 'indigo', category: 'Strength' },
];

const FEATURED_SVG = `<svg viewBox="0 0 260 230" xmlns="http://www.w3.org/2000/svg">
    <ellipse cx="130" cy="212" rx="90" ry="10" fill="rgba(0,0,0,0.12)"/>
    <g fill="none" stroke="rgba(20,22,31,0.8)" stroke-width="6" stroke-linecap="round" stroke-linejoin="round">
        <path d="M130 60 L130 120"/><path d="M130 120 L95 200"/><path d="M130 120 L165 200"/>
        <path d="M130 85 L75 55"/><path d="M130 85 L185 55"/><path d="M75 55 L60 20"/><path d="M185 55 L200 20"/>
    </g>
    <circle cx="130" cy="42" r="18" fill="rgba(20,22,31,0.8)"/>
</svg>`;

let occurrences = [];
let currentPage = 1;
let totalCount = 0;
const PAGE_SIZE = 9;

let activeCategory = '';
let activeTrainer = '';

document.addEventListener('DOMContentLoaded', () => {
    loadOccurrences(true);

    document.getElementById('rangeFilter').addEventListener('change', () => loadOccurrences(true));
    document.getElementById('trainerFilter').addEventListener('change', (e) => { activeTrainer = e.target.value; renderGrid(); });
    document.getElementById('loadMoreBtn').addEventListener('click', () => loadOccurrences(false));

    document.querySelectorAll('.view-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.view-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            document.getElementById('classesGrid').classList.toggle('list-view', btn.dataset.view === 'list');
        });
    });
});

function showMessage(text, type) {
    const banner = document.getElementById('msgBanner');
    banner.textContent = text;
    banner.className = `msg-banner show ${type}`;
    setTimeout(() => banner.classList.remove('show'), 4000);
}

function showToast(message) {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.classList.add('show');
    clearTimeout(showToast._t);
    showToast._t = setTimeout(() => toast.classList.remove('show'), 2800);
}

function categoryFor(name) {
    const found = CLASS_ICONS.find(c => c.match.test(name));
    return found || { icon: 'bx-body', color: 'indigo', category: 'General' };
}

async function loadOccurrences(reset) {
    if (reset) { currentPage = 1; occurrences = []; }

    const rangeDays = parseInt(document.getElementById('rangeFilter').value, 10);
    const from = new Date();
    const to = new Date(from.getTime() + rangeDays * 24 * 60 * 60 * 1000);

    const grid = document.getElementById('classesGrid');
    if (reset) grid.innerHTML = `<div class="state-empty">Loading classes…</div>`;

    try {
        const url = `/api/Classes/browse?fromDate=${toDateInput(from)}&toDate=${toDateInput(to)}&Page=${currentPage}&Page_Size=${PAGE_SIZE}`;
        const data = await FitCoreApi.get(url);
        const pageItems = data.data || data.Data || [];
        totalCount = data.totalCount ?? data.TotalCount ?? pageItems.length;

        occurrences = reset ? pageItems : occurrences.concat(pageItems);

        populateFilterOptions();
        renderGrid();

        const loadMoreBtn = document.getElementById('loadMoreBtn');
        loadMoreBtn.style.display = occurrences.length < totalCount ? 'inline-flex' : 'none';
        if (occurrences.length < totalCount) currentPage++;
    } catch (error) {
        grid.innerHTML = `<div class="state-empty">Couldn't load classes: ${escapeHtml(error.message)}</div>`;
    }
}

function populateFilterOptions() {
    const categories = new Set();
    const trainers = new Set();
    occurrences.forEach(o => {
        categories.add(categoryFor(pick(o, 'className', 'ClassName')).category);
        trainers.add(pick(o, 'trainerName', 'TrainerName'));
    });

    const pillsContainer = document.getElementById('disciplinePills');
    const existingButtons = new Set(Array.from(pillsContainer.querySelectorAll('.pill')).map(b => b.dataset.filter));
    categories.forEach(cat => {
        if (!existingButtons.has(cat)) {
            const btn = document.createElement('button');
            btn.className = 'pill';
            btn.dataset.filter = cat;
            btn.textContent = cat;
            btn.addEventListener('click', () => {
                document.querySelectorAll('.discipline-pills .pill').forEach(p => p.classList.remove('active'));
                btn.classList.add('active');
                activeCategory = cat;
                renderGrid();
            });
            pillsContainer.appendChild(btn);
        }
    });
    pillsContainer.querySelector('[data-filter=""]').onclick = () => {
        document.querySelectorAll('.discipline-pills .pill').forEach(p => p.classList.remove('active'));
        pillsContainer.querySelector('[data-filter=""]').classList.add('active');
        activeCategory = '';
        renderGrid();
    };

    const trainerSelect = document.getElementById('trainerFilter');
    const currentValue = trainerSelect.value;
    trainerSelect.innerHTML = '<option value="">Filter by Trainer</option>' +
        Array.from(trainers).sort().map(t => `<option value="${escapeHtml(t)}">${escapeHtml(t)}</option>`).join('');
    trainerSelect.value = currentValue;
}

function getFilteredOccurrences() {
    return occurrences.filter(o => {
        if (activeCategory && categoryFor(pick(o, 'className', 'ClassName')).category !== activeCategory) return false;
        if (activeTrainer && pick(o, 'trainerName', 'TrainerName') !== activeTrainer) return false;
        return true;
    });
}

function renderGrid() {
    const grid = document.getElementById('classesGrid');
    const filtered = getFilteredOccurrences();

    if (filtered.length === 0) {
        grid.innerHTML = `<div class="state-empty"><i class='bx bx-calendar-x' style="font-size:28px;display:block;margin-bottom:8px;"></i>No sessions match your filters in this range.</div>`;
        return;
    }

    grid.innerHTML = filtered.map((o, index) => renderCard(o, index === 0)).join('');

    grid.querySelectorAll('[data-book]').forEach(btn => {
        btn.addEventListener('click', () => bookOccurrence(btn.dataset.scheduleId, btn.dataset.sessionDate, btn));
    });
}

function renderCard(o, featured) {
    const scheduleId = pick(o, 'classScheduleID', 'ClassScheduleID');
    const className = pick(o, 'className', 'ClassName');
    const description = pick(o, 'description', 'Description') || '';
    const trainerName = pick(o, 'trainerName', 'TrainerName') || '—';
    const sessionDate = (pick(o, 'sessionDate', 'SessionDate') || '').toString().substring(0, 10);
    const start = (pick(o, 'startTime', 'StartTime') || '').toString().substring(0, 5);
    const end = (pick(o, 'endTime', 'EndTime') || '').toString().substring(0, 5);
    const available = Number(pick(o, 'availableSpots', 'AvailableSpots'));
    const capacity = Number(pick(o, 'capacity', 'Capacity'));
    const isFull = available <= 0;
    const cat = categoryFor(className);
    const whenLabel = dayLabelFor(sessionDate);

    if (featured) {
        return `
        <article class="card featured">
            <div class="featured-media">
                <div class="featured-badges">
                    <span class="tag-badge">${escapeHtml(cat.category)}</span>
                    <span class="tag-badge blue">Featured</span>
                </div>
                ${FEATURED_SVG}
            </div>
            <div class="featured-body">
                <div class="class-name">${escapeHtml(className)}</div>
                <div class="tagline">${escapeHtml(description)}</div>
                <div class="featured-footer">
                    <div class="instructor"><span class="avatar-sm">${initials(trainerName)}</span>${escapeHtml(trainerName)}</div>
                    <div class="time-tag"><span class="time">${start}</span><span class="when">${whenLabel}</span></div>
                </div>
                <button class="btn-primary" data-book data-schedule-id="${scheduleId}" data-session-date="${sessionDate}" ${isFull ? 'disabled' : ''}>
                    ${isFull ? 'Full' : 'Book Now'}
                </button>
            </div>
        </article>`;
    }

    return `
    <article class="card regular">
        <div class="card-top">
            <div class="icon-chip ${cat.color}"><i class='bx ${cat.icon}'></i></div>
            <span class="level-badge">${available}/${capacity} open</span>
        </div>
        <div class="class-name">${escapeHtml(className)}</div>
        <div class="meta-row"><i class='bx bx-calendar'></i> ${escapeHtml(whenLabel)}, ${start} - ${end}</div>
        <div class="meta-row"><i class='bx bx-user'></i> ${escapeHtml(trainerName)}</div>
        <button class="btn-outline" data-book data-schedule-id="${scheduleId}" data-session-date="${sessionDate}" ${isFull ? 'disabled' : ''}>
            ${isFull ? 'Full' : 'Book Spot'}
        </button>
    </article>`;
}

function dayLabelFor(isoDate) {
    const today = new Date(); today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today.getTime() + 86400000);
    const target = new Date(isoDate + 'T00:00:00');

    if (target.getTime() === today.getTime()) return 'Today';
    if (target.getTime() === tomorrow.getTime()) return 'Tomorrow';
    return DAY_LABELS[target.getDay()] + ' ' + target.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

async function bookOccurrence(classScheduleId, sessionDate, btn) {
    const memberUserId = window.CURRENT_MEMBER_USER_ID;
    const originalText = btn.textContent;
    btn.disabled = true;
    btn.textContent = 'Booking…';

    try {
        await FitCoreApi.post(`/api/Classes/book?memberUserId=${memberUserId}`, {
            classScheduleID: parseInt(classScheduleId, 10),
            sessionDate,
        });
        showToast('You are booked in! Check My Bookings for details.');
        await loadOccurrences(true);
    } catch (error) {
        showMessage(error.message, 'error');
        btn.disabled = false;
        btn.textContent = originalText;
    }
}

function toDateInput(date) { return date.toISOString().substring(0, 10); }

function initials(name) {
    return (name || '').split(' ').map(p => p[0]).join('').substring(0, 2).toUpperCase();
}

function escapeHtml(str) {
    const div = document.createElement('div');
    div.textContent = str ?? '';
    return div.innerHTML;
}
