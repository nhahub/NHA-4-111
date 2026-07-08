// ===============================
// FitCore Classes
// classes.js
// ===============================

const API_BASE = "/api/Classes";

// مؤقتًا إلى أن يتم ربط Authentication
const MEMBER_USER_ID = 1;

let CLASSES = [];

let activeFilter = "All";
let activeTrainer = "";
let activeSearch = "";
let activeView = "grid";

document.addEventListener("DOMContentLoaded", () => {

    loadClasses();

    initFilters();

    initSearch();

    initView();

});


// ==========================================
// Load Classes From API
// ==========================================

async function loadClasses() {

    try {

        const from = new Date();

        const to = new Date();

        to.setDate(to.getDate() + 14);

        const fromDate = from.toISOString().split("T")[0];

        const toDate = to.toISOString().split("T")[0];

        const response = await fetch(
            `${API_BASE}/browse?Page_Size=20&Page=1`
        );

        if (!response.ok)
            throw new Error("Unable to load classes");

        const result = await response.json();
        console.log(result);
        CLASSES = result.data || result.Data || [];

        loadTrainerFilter();

        renderClasses();

    }
    catch (err) {

        console.error(err);

        document.getElementById("classesGrid").innerHTML = `
            <div class="empty-state">
                <i class='bx bx-error'></i>
                <h3>Unable to load classes.</h3>
            </div>
        `;

    }

}



// ==========================================
// Discipline Filter
// ==========================================

function initFilters() {

    document
        .getElementById("disciplinePills")
        .addEventListener("click", function (e) {

            const btn = e.target.closest(".pill");

            if (!btn) return;

            document
                .querySelectorAll(".pill")
                .forEach(x => x.classList.remove("active"));

            btn.classList.add("active");

            activeFilter = btn.dataset.filter;

            renderClasses();

        });


    document
        .getElementById("trainerFilter")
        .addEventListener("change", function () {

            activeTrainer = this.value;

            renderClasses();

        });

}



// ==========================================
// Search
// ==========================================

function initSearch() {

    document
        .getElementById("searchInput")
        .addEventListener("input", function () {

            activeSearch = this.value
                .trim()
                .toLowerCase();

            renderClasses();

        });

}



// ==========================================
// Grid / List
// ==========================================

function initView() {

    document
        .querySelectorAll(".view-btn")
        .forEach(btn => {

            btn.addEventListener("click", () => {

                document
                    .querySelectorAll(".view-btn")
                    .forEach(x => x.classList.remove("active"));

                btn.classList.add("active");

                activeView = btn.dataset.view;

                document
                    .getElementById("classesGrid")
                    .classList.toggle(
                        "list-view",
                        activeView === "list"
                    );

            });

        });

}



// ==========================================
// Trainer Dropdown
// ==========================================

function loadTrainerFilter() {

    const select =
        document.getElementById("trainerFilter");

    const trainers = [
        ...new Set(
            CLASSES.map(c => c.trainerName)
        )
    ];

    select.innerHTML =
        `<option value="">All Trainers</option>`;

    trainers.forEach(t => {

        select.innerHTML +=
            `<option value="${t}">${t}</option>`;

    });

}



// ==========================================
// Filter Data
// ==========================================

function getFilteredClasses() {

    return CLASSES.filter(c => {

        if (
            activeFilter !== "All" &&
            c.className !== activeFilter
        )
            return false;

        if (
            activeTrainer &&
            c.trainerName !== activeTrainer
        )
            return false;

        if (activeSearch) {

            const text =
                (
                    c.className +
                    " " +
                    c.trainerName
                ).toLowerCase();

            if (!text.includes(activeSearch))
                return false;

        }

        return true;

    });

}



// ==========================================
// Helpers
// ==========================================

function escapeHtml(text) {

    const div = document.createElement("div");

    div.textContent = text ?? "";

    return div.innerHTML;

}

// ==========================================
// Render Classes
// ==========================================

function renderClasses() {

    const grid = document.getElementById("classesGrid");

    const classes = getFilteredClasses();

    if (classes.length === 0) {

        grid.innerHTML = `
            <div class="empty-state">
                <i class='bx bx-calendar-x'></i>
                <h3>No Classes Found</h3>
                <p>Try changing your filters.</p>
            </div>
        `;

        return;
    }

    grid.innerHTML = classes
        .map(renderCard)
        .join("");

    document
        .querySelectorAll(".book-btn")
        .forEach(btn => {

            btn.addEventListener("click", () => {

                bookClass(
                    btn.dataset.schedule,
                    btn.dataset.date
                );

            });

        });

}



// ==========================================
// Render Single Card
// ==========================================

function renderCard(c) {

    const available =
        c.availableSpots ?? 0;

    const capacity =
        c.capacity ?? 0;

    const full =
        available <= 0;

    const start =
        (c.startTime || "")
            .toString()
            .substring(0, 5);

    const end =
        (c.endTime || "")
            .toString()
            .substring(0, 5);

    const date =
        (c.sessionDate || "")
            .toString()
            .substring(0, 10);

    return `

    <article class="card regular">

        <div class="card-top">

            <span class="level-badge">

                ${available}/${capacity} Spots

            </span>

        </div>

        <div class="class-name">

            ${escapeHtml(c.className)}

        </div>

        <div class="meta-row">

            <i class='bx bx-user'></i>

            ${escapeHtml(c.trainerName)}

        </div>

        <div class="meta-row">

            <i class='bx bx-calendar'></i>

            ${date}

        </div>

        <div class="meta-row">

            <i class='bx bx-time-five'></i>

            ${start}
            -
            ${end}

        </div>

        <div class="meta-row">

            <i class='bx bx-group'></i>

            ${available}
            /
            ${capacity}
            Available

        </div>

        <button

            class="btn btn-solid book-btn"

            data-schedule="${c.classScheduleID}"

            data-date="${date}"

            ${full ? "disabled" : ""}

        >

            ${full ? "Class Full" : "Book Now"}

        </button>

    </article>

    `;

}

// ==========================================
// Book Class
// ==========================================

async function bookClass(classScheduleId, sessionDate) {

    try {

        const response = await fetch(

            `${API_BASE}/book?memberUserId=${MEMBER_USER_ID}`,

            {
                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify({

                    classScheduleID: Number(classScheduleId),

                    sessionDate: sessionDate

                })

            }

        );

        const result = await response.json();

        if (!response.ok) {

            throw new Error(

                result.message ||

                result.Message ||

                "Booking failed."

            );

        }

        showToast(

            "Class booked successfully.",

            "success"

        );

        // Reload latest availability
        await loadClasses();

    }
    catch (err) {

        console.error(err);

        showToast(

            err.message ||

            "Unable to complete booking.",

            "error"

        );

    }

}



// ==========================================
// Toast Message
// ==========================================

function showToast(message, type = "success") {

    const toast = document.getElementById("toast");

    toast.textContent = message;

    toast.className = "toast show";

    if (type === "error") {

        toast.classList.add("error");

    }

    clearTimeout(showToast.timer);

    showToast.timer = setTimeout(() => {

        toast.classList.remove("show");

    }, 3000);

}



// ==========================================
// Format Date
// ==========================================

function formatDate(dateString) {

    const date = new Date(dateString);

    return date.toLocaleDateString(

        "en-GB",

        {

            day: "2-digit",

            month: "short",

            year: "numeric"

        }

    );

}



// ==========================================
// Format Time
// ==========================================

function formatTime(time) {

    if (!time)

        return "";

    return time.toString().substring(0, 5);

}



// ==========================================
// Refresh Every Minute
// ==========================================

setInterval(() => {

    loadClasses();

}, 60000);
