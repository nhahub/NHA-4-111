// access-key.js
// Wires the QR Key page to AttendanceController's self-service endpoints:
//   GET  /api/Attendance/me/qrcode?userId=
//   POST /api/Attendance/me/qrcode/regenerate?userId=
//   GET  /api/Attendance/me/status-today?userId=
// These endpoints take userId as a required query param since there is no
// auth yet; window.CURRENT_MEMBER_USER_ID stands in for the logged-in member.

const ATTENDANCE_BASE = '/api/Attendance';
const user = getCurrentUser();

document.addEventListener('DOMContentLoaded', () => {
    loadQrCode();
    loadStatusToday();

    document.getElementById('btn-regenerate')?.addEventListener('click', regenerateKey);
});

function currentUserId() {
    return user?.userId ;
}

async function loadQrCode() {
    try {
        const data = await FitCoreApi.get(`${ATTENDANCE_BASE}/me/qrcode?userId=${currentUserId()}`);
        const qrCodeValue = pick(data, 'qrCode', 'QrCode');
        updateQrImage(qrCodeValue);
    } catch (error) {
        console.error("Error loading QR code:", error);
        showBanner('Could not load your access key.');
    }
}

async function loadStatusToday() {
    const badge = document.getElementById('statusBadge');
    const badgeText = document.getElementById('statusBadgeText');
    try {
        const checkedInToday = await FitCoreApi.get(`${ATTENDANCE_BASE}/me/status-today?userId=${currentUserId()}`);
        if (checkedInToday) {
            badge.classList.remove('status-inactive');
            badgeText.textContent = 'Checked in today';
        } else {
            badge.classList.add('status-inactive');
            badgeText.textContent = 'Not checked in yet';
        }
    } catch (error) {
        console.error('Error loading today status:', error);
        badgeText.textContent = 'Status unavailable';
    }
}

async function regenerateKey() {
    const btn = document.getElementById('btn-regenerate');
    btn.disabled = true;
    const originalHtml = btn.innerHTML;
    btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Generating…';

    try {
        const newQrCode = await FitCoreApi.post(`${ATTENDANCE_BASE}/me/qrcode/regenerate?userId=${currentUserId()}`);
        updateQrImage(newQrCode);
        showToast('Your access key has been regenerated.');
    } catch (error) {
        console.error('Error regenerating QR code:', error);
        showToast('Could not regenerate your key. Please try again.');
    } finally {
        btn.disabled = false;
        btn.innerHTML = originalHtml;
    }
}

function updateQrImage(qrCode) {
    const qrImg = document.getElementById('qr-code');
    const qrText = document.getElementById('qrCodeText');
    if (!qrCode) return;

    qrImg.src = `https://api.qrserver.com/v1/create-qr-code/?size=256x256&data=${encodeURIComponent(qrCode)}`;
    qrText.textContent = qrCode;
}

function showBanner(message) {
    const banner = document.getElementById('msgBanner');
    banner.textContent = message;
    banner.style.display = 'block';
}

function showToast(message) {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.classList.add('show');
    setTimeout(() => toast.classList.remove('show'), 3000);
}
