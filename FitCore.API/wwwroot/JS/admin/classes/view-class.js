const API_URL = "/api/Classes";

const params = new URLSearchParams(window.location.search);

const classId = params.get("id");

document.addEventListener("DOMContentLoaded", () => {

    if (!classId) {

        alert("Class Id is missing.");

        window.location.href = "/html/admin/classes/admin-classes.html";

        return;
    }

    loadClass();

});

async function loadClass() {

    try {

        const response = await fetch(`${API_URL}/${classId}`);

        if (!response.ok)
            throw new Error("Failed to load class.");

        const cls = await response.json();

        fillClassData(cls);

    }

    catch (err) {

        alert(err.message);

    }

}

function fillClassData(cls) {

    document.getElementById("className").value = cls.className;

    document.getElementById("description").value = cls.description;

    document.getElementById("capacity").value = cls.capacity;

    document.getElementById("trainerName").value = cls.trainerName;

    document.getElementById("status").value =
        cls.status === 1 ? "Active" : "Inactive";

    renderSchedules(cls.schedules);

}

function renderSchedules(schedules) {

    const tbody = document.getElementById("scheduleTable");

    tbody.innerHTML = "";

    if (!schedules || schedules.length === 0) {

        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="text-center text-muted py-5">
                    No schedules found.
                </td>
            </tr>
        `;

        return;
    }

    schedules.forEach((item, index) => {

        tbody.innerHTML += `
            <tr>

                <td>${index + 1}</td>

                <td>${getDayName(item.day)}</td>

                <td>${item.startTime.substring(0, 5)}</td>

                <td>${item.endTime.substring(0, 5)}</td>

            </tr>
        `;

    });

}

function getDayName(day) {

    const days = {

        0: "Sunday",

        1: "Monday",

        2: "Tuesday",

        3: "Wednesday",

        4: "Thursday",

        5: "Friday",

        6: "Saturday"

    };

    return days[day] || "-";

}