/**
 * FitCore Hub - Master Member Classes Engine
 */
const API_BASE_CLASSES = '/api/Classes'; 
const API_BASE_TRAINERS = '/api/Trainers';
const CURRENT_MEMBER_ID = 1; 

// تخزين محلي للبيانات لضمان تصفية (Filtering) فائقة السرعة وبدون إعادة تحميل خادمة
let allOccurrences = [];
let activeDiscipline = 'All';
let currentView = 'grid';

document.addEventListener('DOMContentLoaded', async () => {
    setDefaultDates();           
    await loadTrainersDropdown(); 
    initializeComponentEvents();  
    loadDashboardPipeline();      
});

async function loadTrainersDropdown() {
    try {
        const response = await fetch(`${API_BASE_TRAINERS}?Page_Size=50&Page=1`);
        if (!response.ok) throw new Error('Failed to fetch trainers');

        const result = await response.json();
        const trainerSelect = document.getElementById('trainerFilter');
        
     
        trainerSelect.innerHTML = '<option value="">Filter by Trainer</option>';

       
        result.data.forEach(trainer => {
            console.log(trainer);
            const option = document.createElement('option');
            option.innerHTML= trainer.fullName;
            option.value = trainer.fullName; 
            trainerSelect.appendChild(option);
        });
    } catch (err) {
        console.error("Error populating trainers filter:", err);
    }
}

function setDefaultDates() {
    const today = new Date();
    const nextTwoWeeks = new Date();
    nextTwoWeeks.setDate(today.getDate() + 14);

    document.getElementById('fromDateInput').value = today.toISOString().split('T')[0];
    document.getElementById('toDateInput').value = nextTwoWeeks.toISOString().split('T')[0];
}

function initializeComponentEvents() {
   
    const pills = document.querySelectorAll('#disciplinePills .btn-pill');
    pills.forEach(pill => {
        pill.addEventListener('click', (e) => {
            pills.forEach(p => p.classList.remove('active'));
            e.currentTarget.classList.add('active');
            activeDiscipline = e.currentTarget.getAttribute('data-filter');
            applyUIFilters();
        });
    });

  
    
    document.getElementById('gridViewBtn').addEventListener('click', () => toggleViewMode('grid'));
    document.getElementById('listViewBtn').addEventListener('click', () => toggleViewMode('list'));

    
    document.getElementById('fromDateInput').addEventListener('change', loadDashboardPipeline);
    document.getElementById('toDateInput').addEventListener('change', loadDashboardPipeline);

    
    document.getElementById('trainerFilter').addEventListener('change', applyUIFilters);
    document.getElementById('searchInput').addEventListener('input', applyUIFilters);

    document.getElementById('loadMoreBtn').addEventListener('click', () => {
        showNotification('All synced classes are fully loaded.', 'info');
    });
}


async function loadDashboardPipeline() {

    const fromDate = document.getElementById('fromDateInput').value;
    const toDate = document.getElementById('toDateInput').value;

    if (!fromDate || !toDate) return;

    try {

        const resBrowse = await fetch(`${API_BASE_CLASSES}/browse?fromDate=${fromDate}&toDate=${toDate}&Page_Size=30&Page=1`);
        if (resBrowse.ok) {
            const results = await resBrowse.json()
            allOccurrences = results.data;
            applyUIFilters();
        }

        await renderMemberBookingsTable();
    } catch (err) {
        console.error("Pipeline synchronization fault:", err);
        showNotification("Error syncing with FitCore database.", "danger");
    }
}

function applyUIFilters() {
    const searchKeyword = document.getElementById('searchInput').value.toLowerCase().trim();
    const selectedTrainer = document.getElementById('trainerFilter').value;
    const gridContainer = document.getElementById('classesGrid');

    gridContainer.innerHTML = '';

    const filtered = allOccurrences.filter(item => {
        const matchesDiscipline = (activeDiscipline === 'All' || item.className.toLowerCase().includes(activeDiscipline.toLowerCase()));
        const matchesTrainer = (!selectedTrainer || item.trainerName === selectedTrainer);
        const matchesSearch = (!searchKeyword ||
            item.className.toLowerCase().includes(searchKeyword) ||
            item.trainerName.toLowerCase().includes(searchKeyword));

        return matchesDiscipline && matchesTrainer && matchesSearch;
    });

    if (filtered.length === 0) {
        gridContainer.innerHTML = `
            <div class="text-center py-5 text-muted w-100">
                <i class='bx bx-folder-open fs-1 mb-2 d-block'></i>
                <p class="font-body">No dynamic classes match your current parameters.</p>
            </div>`;
        return;
    }

    // بناء الكروت بناءً على نمط العرض النشط
    filtered.forEach(occ => {
        const cardCol = document.createElement('div');

        const isSoldOut = occ.availableSpots <= 0;
        const spotsText = isSoldOut ? 'Fully Booked' : `${occ.availableSpots} Spots Available`;
        const badgeColor = isSoldOut ? 'bg-danger-subtle text-danger' : 'bg-light text-dark';

        const dateFormatted = new Date(occ.sessionDate).toLocaleDateString('en-US', {
            weekday: 'short', month: 'short', day: 'numeric'
        });

        if (currentView === 'grid') {
            // 🔳 تصميم الـ Grid الافتراضي (3 كروت في الصف)
            cardCol.className = 'col-12 col-md-6 col-lg-4';
            cardCol.innerHTML = `
                <div class="class-card-custom d-flex flex-column h-100 p-4">
                    <div class="d-flex justify-content-between align-items-start mb-3">
                        <span class="badge ${badgeColor} rounded-pill px-3 py-2 fw-semibold" style="font-size: 11px;">${spotsText}</span>
                        <span class="text-primary fw-bold" style="font-size: 12px; letter-spacing: 0.05em;"><i class='bx bx-star me-1'></i>POPULAR</span>
                    </div>
                    
                    <h4 class="font-display fw-bold text-dark mb-2" style="font-size: 22px; letter-spacing: -0.01em;">${occ.className}</h4>
                    <p class="text-muted font-body mb-4 flex-grow-1" style="font-size: 14px; line-height: 1.5;">${occ.description || 'Elevate your core performance standards with our structured blueprint.'}</p>
                    
                    <div class="border-top pt-3 mt-auto" style="font-size: 13px;">
                        <div class="d-flex align-items-center justify-content-between mb-2">
                            <span class="text-muted"><i class='bx bx-user-voice me-2 fs-5 align-middle'></i>Trainer</span>
                            <span class="fw-semibold text-dark">${occ.trainerName}</span>
                        </div>
                        <div class="d-flex align-items-center justify-content-between mb-2">
                            <span class="text-muted"><i class='bx bx-calendar-event me-2 fs-5 align-middle'></i>Date</span>
                            <span class="fw-semibold text-dark">${dateFormatted}</span>
                        </div>
                        <div class="d-flex align-items-center justify-content-between mb-4">
                            <span class="text-muted"><i class='bx bx-time-five me-2 fs-5 align-middle'></i>Time Window</span>
                            <span class="fw-semibold text-dark">${occ.startTime.substring(0, 5)} - ${occ.endTime.substring(0, 5)}</span>
                        </div>
                    </div>

                    <button class="btn btn-primary w-100 rounded-pill py-2.5 font-body fw-semibold shadow-sm text-center align-middle" 
                        ${isSoldOut ? 'disabled style="opacity: 0.4;"' : ''} 
                        onclick="triggerBookingTransaction(${occ.classScheduleID}, '${occ.sessionDate}')">
                        ${isSoldOut ? 'Sold Out' : "Book Slot <i class='bx bx-right-arrow-alt ms-1 fs-5 align-middle'></i>"}
                    </button>
                </div>
            `;
        } else {
            
            cardCol.className = 'col-12';
            cardCol.innerHTML = `
                <div class="class-card-custom d-flex flex-column flex-md-row align-items-md-center justify-content-between p-3 px-4 gap-3 mb-2">
                    <div class="d-flex align-items-center gap-3 flex-grow-1" style="max-width: 50%;">
                        <div class="d-none d-sm-block text-center bg-light rounded px-3 py-2 border" style="min-width: 75px;">
                            <span class="font-display fw-bold text-dark d-block fs-5">${dateFormatted.split(' ')[2] || dateFormatted.split(' ')[1]}</span>
                            <span class="text-muted text-uppercase fw-semibold" style="font-size: 10px;">${dateFormatted.split(' ')[0]}</span>
                        </div>
                        <div>
                            <h4 class="font-display fw-bold text-dark mb-1" style="font-size: 18px; letter-spacing: -0.01em;">${occ.className}</h4>
                            <p class="text-muted font-body mb-0 text-truncate" style="font-size: 13px; max-width: 400px;">${occ.description || 'Elevate your core performance standards.'}</p>
                        </div>
                    </div>
                    
                    <div class="d-flex flex-wrap align-items-center gap-4 text-muted font-body" style="font-size: 13px;">
                        <div><i class='bx bx-user-voice me-1 fs-5 align-middle'></i> <span class="fw-medium text-dark">${occ.trainerName}</span></div>
                        <div><i class='bx bx-time-five me-1 fs-5 align-middle'></i> <span class="fw-medium text-dark">${occ.startTime.substring(0, 5)} - ${occ.endTime.substring(0, 5)}</span></div>
                        <div><span class="badge ${badgeColor} rounded-pill px-3 py-1.5 fw-semibold" style="font-size: 11px;">${spotsText}</span></div>
                    </div>

                    <div class="text-end">
                        <button class="btn btn-primary rounded-pill px-4 py-2 font-body fw-semibold shadow-sm text-center btn-sm" 
                            ${isSoldOut ? 'disabled style="opacity: 0.4;"' : ''} 
                            onclick="triggerBookingTransaction(${occ.classScheduleID}, '${occ.sessionDate}')">
                            ${isSoldOut ? 'Sold Out' : "Book Slot"}
                        </button>
                    </div>
                </div>
            `;
        }

        gridContainer.appendChild(cardCol);
    });
}


async function triggerBookingTransaction(scheduleId, sessionDate) {
    const bookingPayload = {
        classScheduleID: scheduleId,
        sessionDate: sessionDate
    };

    try {
        const response = await fetch(`${API_BASE}/book?memberUserId=${CURRENT_MEMBER_ID}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(bookingPayload)
        });

        if (!response.ok) throw new Error('Transaction denied by business rules.');

        showNotification('🎉 Slot reserved! Your dynamic calendar was updated.', 'success');
        loadDashboardPipeline(); // تحديث الواجهات فوراً بعد الحجز
    } catch (error) {
        showNotification(`Booking failed: ${error.message}`, 'danger');
    }
}


async function renderMemberBookingsTable() {
    const tableBody = document.getElementById('myBookingsTableBody');
    tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted py-4">Syncing active bookings registry...</td></tr>';

    try {
        const response = await fetch(`${API_BASE}/my-bookings?memberUserId=${CURRENT_MEMBER_ID}`);
        if (!response.ok) throw new Error('Could not parse database response.');

        const bookings = await response.json();
        tableBody.innerHTML = '';

        if (bookings.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-muted py-4"><i class="bx bx-calendar-x fs-4 d-block mb-1"></i>You haven\'t reserved any session slots yet.</td></tr>';
            return;
        }

        bookings.forEach(b => {
            const tr = document.createElement('tr');
            const sessionDateStr = new Date(b.sessionDate).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });

            const isCancelled = b.status === 'Cancelled' || b.status === 1;
            const badgeClass = isCancelled ? 'status-cancelled' : 'status-confirmed';
            const displayStatus = isCancelled ? 'Cancelled' : 'Confirmed';

            tr.innerHTML = `
                <td class="ps-4 py-3 fw-bold text-dark">${b.className}</td>
                <td class="py-3 text-muted">${sessionDateStr}</td>
                <td class="py-3 fw-medium text-dark">${b.startTime.substring(0, 5)} - ${b.endTime.substring(0, 5)}</td>
                <td class="py-3"><span class="booking-status-badge ${badgeClass}">${displayStatus}</span></td>
                <td class="pe-4 py-3 text-end">
                    <button class="btn btn-sm btn-link text-danger p-0 border-0 text-decoration-none fw-semibold" 
                        ${isCancelled ? 'disabled style="display:none;"' : ''} 
                        onclick="abortMemberBooking(${b.bookingID})">
                        <i class='bx bx-trash me-1 fs-6 align-middle'></i>Cancel Slot
                    </button>
                </td>
            `;
            tableBody.appendChild(tr);
        });
    } catch (error) {
        tableBody.innerHTML = `<tr><td colspan="5" class="text-center text-danger py-4">Failed to fetch reserved energies list.</td></tr>`;
    }
}

/**
 * HttpPatch: إلغاء الحجز الفعلي للمشترك
 */
async function abortMemberBooking(bookingId) {
    if (!confirm("Are you sure you want to drop this training spot?")) return;

    try {
        const response = await fetch(`${API_BASE}/bookings/${bookingId}/cancel?memberUserId=${CURRENT_MEMBER_ID}`, {
            method: 'HttpPatch' // أو PATCH حسب إعدادات الـ Route في الـ API Controller الخاص بك
        });

        if (!response.ok) throw new Error('Cancellation request declined.');

        showNotification('Slot cancelled. Your spot was released to other community members.', 'info');
        loadDashboardPipeline();
    } catch (error) {
        showNotification(`Error: ${error.message}`, 'danger');
    }
}

function toggleViewMode(view) {
    if (currentView === view) return; 
    currentView = view;

    const gridBtn = document.getElementById('gridViewBtn');
    const listBtn = document.getElementById('listViewBtn');

    if (view === 'grid') {
        gridBtn.classList.add('btn-light', 'active');
        gridBtn.classList.remove('bg-transparent', 'text-secondary');

        listBtn.classList.add('bg-transparent', 'text-secondary');
        listBtn.classList.remove('btn-light', 'active');
    } else {
        listBtn.classList.add('btn-light', 'active');
        listBtn.classList.remove('bg-transparent', 'text-secondary');

        gridBtn.classList.add('bg-transparent', 'text-secondary');
        gridBtn.classList.remove('btn-light', 'active');
    }

    applyUIFilters();
}

function showNotification(message, type = 'success') {
    const toastElement = document.getElementById('statusToast');
    const toastMsgHolder = document.getElementById('toastMessage');

    toastMsgHolder.textContent = message;

    // تغيير التنسيق البصري للـ Toast مؤقتاً بحسب نوع الحالة لتوفير تجربة مستخدم واضحة
    if (type === 'danger') {
        toastElement.classList.replace('text-bg-dark', 'text-bg-danger');
    } else if (type === 'info') {
        toastElement.classList.replace('text-bg-dark', 'text-bg-secondary');
    } else {
        toastElement.className = "toast align-items-center text-bg-dark border-0 rounded-pill px-3 py-1 shadow-lg";
    }

    const bsToast = new bootstrap.Toast(toastElement, { delay: 4000 });
    bsToast.show();
}