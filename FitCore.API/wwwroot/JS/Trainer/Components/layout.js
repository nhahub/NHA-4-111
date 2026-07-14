
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

function initTrainerLayout() {
    console.log("🎬 Trainer layout started initializing...");

    fetch('/HTML/Trainer/Components/sidebar-trainer.html')
        .then(response => response.text())
        .then(html => {
            const sidebarContainer = document.getElementById('sidebar-container');
            if (sidebarContainer) {
                sidebarContainer.innerHTML = html;
            }

            if (typeof setActiveSidebarLink === 'function') {
                setActiveSidebarLink();
            }
        })
        .catch(error => console.error('Error loading sidebar:', error));

    
    loadComponent('header-container', '/HTML/Trainer/Components/header.html').then(() => {

      
        if (typeof initNotificationSystem === 'function') {
            initNotificationSystem();
        }

       
        const toggleBtn = document.querySelector('.sidebar-toggle-btn');
        const sidebar = document.getElementById('sidebar-container');

       
        if (toggleBtn && sidebar) {
            toggleBtn.addEventListener('click', () => {
                sidebar.classList.toggle('open');
            });
        }

   
        document.addEventListener('click', (event) => {
            if (sidebar && sidebar.classList.contains('open') && toggleBtn) {
                if (!sidebar.contains(event.target) && !toggleBtn.contains(event.target)) {
                    sidebar.classList.remove('open');
                }
            }
        });

        
        
    });
}

if (document.readyState === "complete" || document.readyState === "interactive") {
    initTrainerLayout();
} else {
    document.addEventListener("DOMContentLoaded", initTrainerLayout);
}
const loadName = () => {
    const TranierName = document.getElementById("TranierName");

    const userAvatar = document.querySelector('[data-user-avatar]');


    if (user && user.fullName) {


        if (TranierName) {
            TranierName.innerHTML = user.fullName;
        }


        if (userAvatar) {

            const nameParts = user.fullName.trim().split(/\s+/);
            let initials = "";

            if (nameParts.length >= 2) {
                initials = nameParts[0][0] + nameParts[1][0];
            } else if (nameParts.length === 1 && nameParts[0].length > 0) {
                initials = nameParts[0].substring(0, 2);
            }

            userAvatar.innerHTML = initials.toUpperCase();
        }
    }
};



let notifPage = 1;
const notifPageSize = 10;


function initNotificationSystem() {
    const bellBtn = document.getElementById('notificationBellBtn');
    const panel = document.getElementById('notificationPanel');
    const closeBtn = document.getElementById('closeNotificationBtn');
    const loadMoreBtn = document.getElementById('loadMoreNotifsBtn');
    const markAllReadBtn = document.getElementById('markAllReadBtn');


    bellBtn.addEventListener('click', () => {
        panel.classList.add('open');

        if (notifPage === 1 && document.getElementById('notificationList').innerHTML.trim() === '') {
            fetchNotifications(notifPage);
        }
    });

    closeBtn.addEventListener('click', () => {
        panel.classList.remove('open');
    });


    loadMoreBtn.addEventListener('click', () => {
        notifPage++;
        fetchNotifications(notifPage, true);
    });

 
    markAllReadBtn.addEventListener('click', async () => {
        try {
            await authFetch('/api/Notification/mark-all-read', { method: 'PATCH' });

            document.querySelectorAll('.notification-item.unread').forEach(item => {
                item.classList.remove('unread');
            });
            updateBadge(0); 
        } catch (error) {
            console.error("Error marking all as read", error);
        }
    });


    fetchNotifications(1);
    setInterval(pollUnreadCount, 30000);
}


async function fetchNotifications(page, append = false) {
    try {
        const data = await authFetch(`/api/Notification?Page=${page}&Page_Size=${notifPageSize}`);

        const notifications = data.data || data.Data || [];

        renderNotifications(notifications, append);


        const totalCount = data.totalCount ?? data.TotalCount ?? 0;
        const footer = document.getElementById('notificationFooter');
        if (page * notifPageSize >= totalCount) {
            footer.style.display = 'none';
        } else {
            footer.style.display = 'block';
        }

        const unreadCount = data.unreadCount ?? notifications.filter(n => !n.isRead && !n.IsRead).length;
        if (page === 1) updateBadge(unreadCount);

    } catch (error) {
        console.error("Error fetching notifications", error);
    }
}


function renderNotifications(notifications, append) {
    const list = document.getElementById('notificationList');
    if (!append) list.innerHTML = ''; 

    if (notifications.length === 0 && !append) {
        list.innerHTML = '<p style="text-align:center; color: var(--text-muted); margin-top: 20px;">No notifications yet.</p>';
        return;
    }

    notifications.forEach(notif => {
 
        const id = notif.id || notif.Id;
        const title = notif.title || notif.Title;
        const message = notif.message || notif.Message;
        const isRead = notif.isRead || notif.IsRead;
        const createdAt = notif.createdAt || notif.CreatedAt;

      
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

        item.addEventListener('click', async () => {
            if (item.classList.contains('unread')) {
                try {
                    const response = await authFetch(`/api/Notification/mark-as-read/${id}`, { method: 'PATCH' });

                    if (response.ok) {
                        item.classList.remove('unread'); 

                        const remainingUnread = document.querySelectorAll('.notification-item.unread').length;
                        updateBadge(remainingUnread);

                    } else {
                        console.error("Error from backend:", await response.text());
                        alert("Failed to mark as read! Check console.");
                    }
                } catch (err) { console.error(err); }
            }
        });

        list.appendChild(item);
    });
}


function updateBadge(unreadCount) {
    const badge = document.getElementById('unreadBadge');
    if (unreadCount > 0) {
        badge.style.display = 'block';
    } else {
        badge.style.display = 'none';
    }
}


function getNotificationStyle(type) {

    const typeStr = String(type).toLowerCase();

    if (typeStr === '0' || typeStr === 'membershipexpiration') {

        return { icon: 'fa-solid fa-id-card-clip', colorClass: 'icon-danger' };
    }
    else if (typeStr === '1' || typeStr === 'productexpiry') {
        
        return { icon: 'fa-solid fa-calendar-xmark', colorClass: 'icon-danger' };
    }
    else if (typeStr === '2' || typeStr === 'announcement') {

        return { icon: 'fa-solid fa-bullhorn', colorClass: 'icon-info' };
    }
    else if (typeStr === '3' || typeStr === 'lowstock') {

        return { icon: 'fa-solid fa-box-open', colorClass: 'icon-warning' };
    }


    return { icon: 'fa-solid fa-bell', colorClass: 'icon-info' };
}


async function pollUnreadCount() {
    try {

        const response = await authFetch(`/api/Notification/UnRead-Count`);

        if (response.ok) {
            const data = await response.json();

            const count = data.unreadCount ?? data.UnreadCount ?? 0;
            updateBadge(count);
        }
    } catch (error) {
        console.error("Error polling unread count:", error);
    }
}