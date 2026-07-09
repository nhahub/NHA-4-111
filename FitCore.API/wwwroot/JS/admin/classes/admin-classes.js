/**
 * FitCore Frontend Hub - Admin Control Board Engine
 */
const ADMIN_API_URL = "/api/Classes";
const API_BASE_TRAINERS = '/api/Trainers';
let currentPage = 1;

document.addEventListener('DOMContentLoaded', () => {
    //document.getElementById('createClassForm').addEventListener('submit', handleClassCreation);
    // document.getElementById('addScheduleForm').addEventListener('submit', handleScheduleAppending);
    loadGlobalClassRegistry();
    initializeComponentEvents();
    loadTrainersDropdown();
});

function initializeComponentEvents() {

    document.getElementById('trainerFilter').addEventListener('change', loadGlobalClassRegistry);
    document.getElementById('activeFilter').addEventListener('change', loadGlobalClassRegistry);
    document.getElementById('searchInput').addEventListener('input', loadGlobalClassRegistry);
}


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
            option.innerHTML = trainer.fullName;
            option.value = trainer.fullName;
            trainerSelect.appendChild(option);
        });
    } catch (err) {
        console.error("Error populating trainers filter:", err);
    }
}

async function handleClassCreation(e) {
    e.preventDefault();

    
    const createClassDto = {
        className: document.getElementById('adminClassName').value,
        description: document.getElementById('adminClassDesc').value,
        capacity: parseInt(document.getElementById('adminClassCapacity').value),
        trainerID: parseInt(document.getElementById('adminClassTrainerID').value),
        schedules: [] 
    };
    console.log(createClassDto);
    try {
        const response = await fetch(ADMIN_API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(createClassDto)
        });

        if (!response.ok) throw new Error('Structural model deployment rejected');
        
        const result = await response.json();
        console.log(result);
        alert(`🚀 Class created successfully with System ID: ${result.classID}`);

        // تلقائياً نضع المعرف الجديد بخانة الخطوة الثانية لتسريع العمل الإداري
        document.getElementById('schedTargetClassID').value = result.classID;

        document.getElementById('createClassForm').reset();
        loadGlobalClassRegistry();
    } catch (error) {
        alert('❌ Admin Error: ' + error.message);
    }
}


async function handleScheduleAppending(e) {
    e.preventDefault();

    const classId = document.getElementById('schedTargetClassID').value;

    // مطابقة تامة لخصائص الـ ClassScheduleDto ونوع الـ DayOfWeek الـ Enum بالسي شارب
    const classScheduleDto = {
        day: parseInt(document.getElementById('schedDayOfWeek').value),
        startTime: document.getElementById('schedStartTime').value + ":00", // تهيئة الـ TimeSpan Format (hh:mm:ss)
        endTime: document.getElementById('schedEndTime').value + ":00"
    };

    try {
        const response = await fetch(`${ADMIN_API_URL}/${classId}/schedules`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(classScheduleDto)
        });

        if (!response.ok) throw new Error('Schedule aggregation script failed');

        alert('📅 Weekly schedule attached smoothly.');
        document.getElementById('addScheduleForm').reset();
        loadGlobalClassRegistry();
    } catch (error) {
        alert('❌ Matrix Error: ' + error.message);
    }
}


async function loadGlobalClassRegistry() {
    const tbody = document.getElementById('adminGlobalClassesTable');
    const searchKeyword = document.getElementById('searchInput').value.toLowerCase().trim();
    const selectedTrainer = document.getElementById('trainerFilter').value;
    const selectedActive = document.getElementById('activeFilter').value;
    showLoadingState();

    try {
        const response = await fetch(`${ADMIN_API_URL}?Page_Size=20&Page=1`);
        if (!response.ok) throw new Error('Unable to sync with Global Class Registry');

        const results = await response.json();

        const totalCount = results.totalCount ?? results.TotalCount ?? 0;
        const page = results.currentPage ?? results.CurrentPage ?? currentPage;
        const pageSize = results.pageSize ?? results.PageSize ?? 10;

        const globalClasses = results.data || results.Data;

        if (results.length === 0) {
            showEmptyState();
        }

        tbody.innerHTML = '';

        const filtered = globalClasses.filter(item => {

            const matchesTrainer = (!selectedTrainer || item.trainerName === selectedTrainer);
            const matchesActive = (!selectedActive || item.status == selectedActive);
            const matchesSearch = (!searchKeyword ||
                item.className.toLowerCase().includes(searchKeyword) ||
                item.trainerName.toLowerCase().includes(searchKeyword));

            return matchesTrainer && matchesSearch && matchesActive;
        });

        if (filtered.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="100%" class="text-center py-5 text-muted">
                        <i class="bx bx-folder-open fs-1 mb-2 d-block"></i>
                        <p class="font-body fs-4 mb-0">No dynamic classes match your current parameters.</p>
                    </td>
                </tr>
            `;
            return;
        }

        filtered.forEach(cls => {
            const tr = document.createElement('tr');
            console.log(cls);
            
            let schedulesSummary = cls.schedules && cls.schedules.length > 0
                ? cls.schedules.map(s => `<span class="badge me-2  mb-2 bg-primary text-white">Day ${s.day}: ${s.startTime.substring(0, 5)}-${s.endTime.substring(0, 5)}</span>`).join('')
                : '<span class="badge me-2 mb-2 bg-primary text-secondary" style="color:var(--text-faint)">No schedule mapped yet</span>';

            tr.innerHTML = `
                <td class="fw-bold text-secondary">#${cls.classID}</td>
                <td class="text-black">${cls.className}</td>
                <td>${cls.capacity} Members</td>
                <td>${cls.trainerName || `Trainer ID: ${cls.trainerID}`}</td>
                <td><div class="flex flex-wrap">${schedulesSummary}</div></td>
                <td>${cls.status === 1 ? "Active" : "Inactive"}</td>
                <td>
                    <a class="btn btn-primary w-auto py-1 rounded-4 "href="/html/admin/classes/view-class.html?id=${cls.classID}" role="button"><i class="fa-solid fa-eye"></i> view </a>
                   <a class="btn btn-outline-primary btn-sm rounded-4"
                       href="https://localhost:7186/html/admin/classes/manage-class.html?id=${cls.classID}&role=edit"
                       role="button">
                        <i class="fa-regular fa-pen-to-square"></i> Edit
                    </a>
                </td>
            `;
            tbody.appendChild(tr);
            renderPagination(totalCount, page, pageSize);
        });
    } catch (error) {
        tbody.innerHTML = `<tr><td colspan="5" style="color:var(--status-red)">${error.message}</td></tr>`;
    }
}

function showLoadingState() {
    const tbody = document.getElementById('adminGlobalClassesTable');
    tbody.innerHTML = `
        <tr >
            <td colspan="100%" class="text-center py-5">
                <i class="fa-solid fa-spinner fa-spin text-primary fs-1"></i>
            </td>
        </tr>
        `;
}

function renderPagination(totalCount, currPage, pageSize) {
    const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
    const paginationContainer = document.getElementById('paginationControls');
    paginationContainer.innerHTML = '';

    if (totalCount === 0) return;

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.innerText = i;
        if (i === currPage) btn.classList.add('active');

        btn.addEventListener('click', () => {
            currentPage = i;
            loadGlobalClassRegistry();
        });

        paginationContainer.appendChild(btn);
    }
}