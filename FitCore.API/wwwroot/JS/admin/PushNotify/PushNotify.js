let token = getToken();

document.addEventListener("DOMContentLoaded", () => {
    requireRole(["Admin"]);
    const form = document.getElementById('pushForm');
    const titleInput = document.getElementById('notifTitle');
    const messageInput = document.getElementById('notifMessage');
    const sendBtn = document.getElementById('sendBtn');
    token = getToken();

    const previewTitle = document.getElementById('previewTitle');
    const previewMessage = document.getElementById('previewMessage');
    const previewIcon = document.getElementById('previewIcon');


    function updatePreview() {
        previewTitle.textContent = titleInput.value || 'Notification Title';
        previewMessage.textContent = messageInput.value || 'Your message will appear here...';


        previewIcon.className = 'notif-icon icon-info';
        previewIcon.innerHTML = '<i class="fa-solid fa-bullhorn"></i>';
    }


    titleInput.addEventListener('input', updatePreview);
    messageInput.addEventListener('input', updatePreview);


    form.addEventListener('submit', async (e) => {
        e.preventDefault();


        const selectedRoles = Array.from(document.querySelectorAll('input[name="targetRoles"]:checked'))
            .map(checkbox => parseInt(checkbox.value));

        if (selectedRoles.length === 0) {
            alert('Please select at least one role to receive this notification.');
            return;
        }

        const originalBtnText = sendBtn.innerHTML;
        sendBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Sending...';
        sendBtn.disabled = true;

        const payload = {
            title: titleInput.value.trim(),
            message: messageInput.value.trim(),
            type: 2, 
            recieveUserRoles: selectedRoles
        };

        try {
            const response = await fetch('/api/Notification', {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                alert('Announcement sent successfully!');
                form.reset();
                updatePreview();
            } else {
                const errorText = await response.text();
                alert(`Failed to send: ${errorText}`);
            }
        } catch (error) {
            console.error("Error pushing notification:", error);
            alert('A network error occurred. Check the console.');
        } finally {
            sendBtn.innerHTML = originalBtnText;
            sendBtn.disabled = false;
        }
    });

    updatePreview();
});