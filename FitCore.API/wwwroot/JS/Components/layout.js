async function loadComponent(elementId, componentPath) {
    try {
        const response = await fetch(componentPath);
        const html = await response.text();
        document.getElementById(elementId).innerHTML = html;
    } catch (error) {
        console.error(`Error loading ${componentPath}:`, error);
    }
}

function setActiveSidebarLink() {
    const currentPage = document.body.dataset.page;
    if (!currentPage) return;

    // بنحول اسم الصفحة لحروف صغيرة
    const currentPageLower = currentPage.toLowerCase();

    document.querySelectorAll('.sidebar-nav ul li').forEach(item => {
        // بنحول اسم الزرار كمان لحروف صغيرة ونقارنهم ببعض
        if (item.dataset.page) {
            const itemPageLower = item.dataset.page.toLowerCase();
            item.classList.toggle('active', itemPageLower === currentPageLower);
        }
    });
}

document.addEventListener("DOMContentLoaded", () => {
    fetch('/HTML/Components/sidebar.html')
        .then(response => response.text())
        .then(html => {
            document.getElementById('sidebar-container').innerHTML = html;
            setActiveSidebarLink();
        })
        .catch(error => console.error('Error loading sidebar:', error));

    // بنستدعي الهيدر، ولما يخلص (then) بنشغل كود الإشعارات
    loadComponent('header-container', '/HTML/Components/header.html').then(() => {
        // السطر ده هيشتغل فقط بعد ما الهيدر يترسم في الشاشة
        if (typeof initNotificationSystem === 'function') {
            initNotificationSystem();
        }
    });
});


// المتغيرات الأساسية للـ Pagination
let notifPage = 1;
const notifPageSize = 10;

// استدعاء الدالة دي بعد ما الـ Header HTML يترسم في الشاشة
function initNotificationSystem() {
    const bellBtn = document.getElementById('notificationBellBtn');
    const panel = document.getElementById('notificationPanel');
    const closeBtn = document.getElementById('closeNotificationBtn');
    const loadMoreBtn = document.getElementById('loadMoreNotifsBtn');
    const markAllReadBtn = document.getElementById('markAllReadBtn');

    // 1. فتح وقفل البانل
    bellBtn.addEventListener('click', () => {
        panel.classList.add('open');
        // لو أول مرة يفتح، نحمل الداتا
        if (notifPage === 1 && document.getElementById('notificationList').innerHTML.trim() === '') {
            fetchNotifications(notifPage);
        }
    });

    closeBtn.addEventListener('click', () => {
        panel.classList.remove('open');
    });

    // 2. زرار Load More
    loadMoreBtn.addEventListener('click', () => {
        notifPage++;
        fetchNotifications(notifPage, true);
    });

    // 3. Mark All as Read
    markAllReadBtn.addEventListener('click', async () => {
        try {
            await fetch('/api/Notification/mark-all-read', { method: 'PUT' });

            // نخلي كل الإشعارات اللي في الشاشة مقروءة
            document.querySelectorAll('.notification-item.unread').forEach(item => {
                item.classList.remove('unread');
            });
            updateBadge(0); // نخفي النقطة الحمرا
        } catch (error) {
            console.error("Error marking all as read", error);
        }
    });

    // نحمل أول صفحة في الخلفية عشان نعرف في إشعارات جديدة ولا لأ
    fetchNotifications(1);
}

// دالة جلب الإشعارات من الـ API
async function fetchNotifications(page, append = false) {
    try {
        const response = await fetch(`/api/Notification?Page=${page}&Page_Size=${notifPageSize}`);
        const data = await response.json();

        // افتراض إن الـ API بيرجع { data: [...], totalCount: 50, unreadCount: 5 }
        const notifications = data.data || data.Data || [];

        renderNotifications(notifications, append);

        // تحديث زرار Load More لو الداتا خلصت
        const totalCount = data.totalCount ?? data.TotalCount ?? 0;
        const footer = document.getElementById('notificationFooter');
        if (page * notifPageSize >= totalCount) {
            footer.style.display = 'none';
        } else {
            footer.style.display = 'block';
        }

        // لو الـ API بيبعت عدد الـ unread في الـ Response، نحدث البادج (النقطة)
        // أو نقدر نحسبهم من اللستة كحل مؤقت لو مش مبعوت
        const unreadCount = data.unreadCount ?? notifications.filter(n => !n.isRead && !n.IsRead).length;
        if (page === 1) updateBadge(unreadCount);

    } catch (error) {
        console.error("Error fetching notifications", error);
    }
}

// دالة رسم الإشعارات
function renderNotifications(notifications, append) {
    const list = document.getElementById('notificationList');
    if (!append) list.innerHTML = ''; // لو مش بنعمل Load More، امسح القديم

    if (notifications.length === 0 && !append) {
        list.innerHTML = '<p style="text-align:center; color: var(--text-muted); margin-top: 20px;">No notifications yet.</p>';
        return;
    }

    notifications.forEach(notif => {
        // تظبيط حالة الحروف حسب الـ JSON
        const id = notif.id || notif.Id;
        const title = notif.title || notif.Title;
        const message = notif.message || notif.Message;
        const isRead = notif.isRead || notif.IsRead;
        const createdAt = notif.createdAt || notif.CreatedAt;

        // تنسيق الوقت
        const type = notif.type ?? notif.Type;
        const notifStyle = getNotificationStyle(type);

        const dateObj = new Date(createdAt);
        const timeString = dateObj.toLocaleDateString() + ' ' + dateObj.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

        const item = document.createElement('div');
        item.className = `notification-item ${isRead ? '' : 'unread'}`;
        item.innerHTML = `
            <div class="notif-icon">
                <i class="fa-solid fa-info"></i>
            </div>
            <div class="notif-content">
                <div class="notif-title">${title}</div>
                <div class="notif-desc">${message}</div>
                <div class="notif-time">${timeString}</div>
            </div>
        `;

        // إيفنت عشان يقرأ الإشعار لما تدوسي عليه
        item.addEventListener('click', async () => {
            if (!isRead && item.classList.contains('unread')) {
                try {
                    await fetch(`/api/Notification/mark-as-read/${id}`, { method: 'PUT' });
                    item.classList.remove('unread');

                    // نقلل عدد الإشعارات الغير مقروءة 
                    const currentBadge = document.getElementById('unreadBadge');
                    if (currentBadge.style.display !== 'none') {
                        // Logic بسيط لإخفاء النقطة لو قرأنا كل حاجة
                        // (يفضل تعملي دالة منفصلة بتعمل Check لعدد الـ unread المتبقي)
                    }
                } catch (err) { console.error(err); }
            }
        });

        list.appendChild(item);
    });
}

// دالة لإظهار/إخفاء النقطة الحمرا
function updateBadge(unreadCount) {
    const badge = document.getElementById('unreadBadge');
    if (unreadCount > 0) {
        badge.style.display = 'block';
    } else {
        badge.style.display = 'none';
    }
}

// دالة بتحدد الأيقونة واللون بناءً على الـ Enum
function getNotificationStyle(type) {
    // بنحول النوع لـ String عشان لو الباك إند بعته كرقم (0,1,2,3) أو كنص
    const typeStr = String(type).toLowerCase();

    if (typeStr === '0' || typeStr === 'membershipexpiration') {
        // اشتراك هينتهي -> أيقونة كارت أحمر
        return { icon: 'fa-solid fa-id-card-clip', colorClass: 'icon-danger' };
    }
    else if (typeStr === '1' || typeStr === 'productexpiry') {
        // منتج هتنتهي صلاحيته -> أيقونة نتيجة حمراء
        return { icon: 'fa-solid fa-calendar-xmark', colorClass: 'icon-danger' };
    }
    else if (typeStr === '2' || typeStr === 'announcement') {
        // إعلان عام -> أيقونة ميكروفون أزرق
        return { icon: 'fa-solid fa-bullhorn', colorClass: 'icon-info' };
    }
    else if (typeStr === '3' || typeStr === 'lowstock') {
        // مخزون قليل -> أيقونة صندوق مفتوح أصفر/برتقالي
        return { icon: 'fa-solid fa-box-open', colorClass: 'icon-warning' };
    }

    // الأيقونة الافتراضية لو جيه نوع غريب
    return { icon: 'fa-solid fa-bell', colorClass: 'icon-info' };
}