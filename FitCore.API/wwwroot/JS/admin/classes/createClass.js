const headerTitle = document.getElementById("headerTitle");
const SubmitBtn = document.getElementById("SubmitBtn");

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('createClassForm').addEventListener('submit', handleClassCreation);
    initializeComponent();
    //document.getElementById('addScheduleForm').addEventListener('submit', handleScheduleAppending);
});
const params = new URLSearchParams(window.location.search);

const id = params.get("id");
const role = params.get("role");

console.log(id);       
console.log(role);


function initializeComponent() {
    if (role === "edit") {
        headerTitle.innerHTML = "Edit Class";
        SubmitBtn.innerHTML = `<i class="fa-regular fa-pen-to-square"></i> Edit Class`;
    } else {
        headerTitle.innerHTML = "Create Class"; 
        SubmitBtn.innerHTML = `<i class="fa-solid fa-plus"></i> Add New Class`;
    }
    
}

async function handleClassCreation(e) {
    // e.preventDefault();


    // const createClassDto = {
    //     className: document.getElementById('adminClassName').value,
    //     description: document.getElementById('adminClassDesc').value,
    //     capacity: parseInt(document.getElementById('adminClassCapacity').value),
    //     trainerID: parseInt(document.getElementById('adminClassTrainerID').value),
    //     schedules: []
    // };
    // console.log(createClassDto);
     try {
    //     const response = await fetch(ADMIN_API_URL, {
    //         method: 'POST',
    //         headers: { 'Content-Type': 'application/json' },
    //         body: JSON.stringify(createClassDto)
    //     });

    //     if (!response.ok) throw new Error('Structural model deployment rejected');

    //     const result = await response.json();
    //     console.log(result);
    //     alert(`🚀 Class created successfully with System ID: ${result.classID}`);
    console.log("result");
        window.location.href = "/html/admin/classes/admin-classes.html";
    } catch (error) {
        alert('❌ Admin Error: ' + error.message);
    }
}
