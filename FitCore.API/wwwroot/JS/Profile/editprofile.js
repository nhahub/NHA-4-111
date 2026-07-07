document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("editProfileForm");
    const saveBtn = document.getElementById("saveBtn");
    const goBackBtn = document.getElementById("goBackBtn");
    const errorContainer = document.getElementById("errorContainer");

    const isTrainerCheck = document.getElementById("isTrainerCheck");
    const trainerSection = document.getElementById("trainerSection");

    async function loadCurrentProfileData() {
        try {
            const response = await fetch("/api/Profile", { method: "GET" });

            if (response.ok) {
                const data = await response.json();

                document.getElementById("fullName").value = data.fullName || data.FullName || '';
                document.getElementById("email").value = data.email || data.Email || '';
                document.getElementById("phoneNumber").value = data.phoneNumber || data.PhoneNumber || '';

                const trainerData = data.trainerDto || data.TrainerDto;
                if (trainerData) {
                    isTrainerCheck.checked = true;
                    trainerSection.classList.remove("hidden");

                    document.getElementById("specialization").value = trainerData.specialization || trainerData.Specialization || '';
                    document.getElementById("workingHours").value = trainerData.workingHours || trainerData.WorkingHours || '';
                    document.getElementById("bio").value = trainerData.bio || trainerData.Bio || '';
                }
            } else {
                console.error("Failed to load profile data.");
            }
        } catch (error) {
            console.error("Error loading profile:", error);
        }
    }

    loadCurrentProfileData();

    goBackBtn.addEventListener("click", () => {
        window.history.back();
    });

    isTrainerCheck.addEventListener("change", (e) => {
        if (e.target.checked) {
            trainerSection.classList.remove("hidden");
        } else {
            trainerSection.classList.add("hidden");
        }
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        errorContainer.classList.add("hidden");
        errorContainer.innerHTML = '';

        const payload = {
            fullName: document.getElementById("fullName").value.trim(),
            email: document.getElementById("email").value.trim(),
            phoneNumber: document.getElementById("phoneNumber").value.trim(),
            trainerDto: null 
        };

        if (isTrainerCheck.checked) {
            payload.trainerDto = {
                specialization: document.getElementById("specialization").value.trim(),
                workingHours: document.getElementById("workingHours").value.trim(),
                bio: document.getElementById("bio").value.trim()
            };
        }

        const originalBtnText = saveBtn.innerHTML;
        saveBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Saving...';
        saveBtn.disabled = true;

        try {
            const response = await fetch("/api/Profile", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                alert("Profile updated successfully!");
                window.location.href = "/HTML/Profile/profile.html";
            } else {
                const errorData = await response.json();

                if (errorData.errors && Array.isArray(errorData.errors)) {
                    showErrors(errorData.errors);
                }
                else if (errorData.errors && typeof errorData.errors === 'object') {
                    const errorMessages = [];
                    for (const key in errorData.errors) {
                        errorMessages.push(...errorData.errors[key]);
                    }
                    showErrors(errorMessages);
                }
                else {
                    showErrors([errorData.message || "Failed to update profile."]);
                }
            }
        } catch (error) {
            console.error("Error saving profile:", error);
            showErrors(["A network error occurred. Please check your connection."]);
        } finally {
            saveBtn.innerHTML = originalBtnText;
            saveBtn.disabled = false;
        }
    });

    function showErrors(errorsArray) {
        errorContainer.innerHTML = `<strong>Please fix the following errors:</strong>`;
        const ul = document.createElement('ul');
        errorsArray.forEach(err => {
            const li = document.createElement('li');
            li.textContent = err;
            ul.appendChild(li);
        });
        errorContainer.appendChild(ul);
        errorContainer.classList.remove("hidden");
        errorContainer.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
});