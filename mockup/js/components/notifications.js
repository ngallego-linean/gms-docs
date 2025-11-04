// Notifications Component

const Notifications = {
    isOpen: false,

    toggle() {
        this.isOpen = !this.isOpen;
        const dropdown = document.getElementById('notification-dropdown');

        if (this.isOpen) {
            this.render();
            dropdown.classList.remove('hidden');
        } else {
            dropdown.classList.add('hidden');
        }
    },

    render() {
        const portalNotifications = this.getPortalNotifications();
        const list = document.getElementById('notification-list');

        if (portalNotifications.length === 0) {
            list.innerHTML = `
                <div style="padding: 2rem; text-align: center; color: #6C757D;">
                    <p>No notifications</p>
                </div>
            `;
            return;
        }

        list.innerHTML = portalNotifications.map(notif => this.renderNotification(notif)).join('');

        // Update badge
        this.updateBadge();
    },

    renderNotification(notif) {
        return `
            <div class="notification-item ${notif.read ? '' : 'unread'}" onclick="Notifications.handleClick(${notif.id})">
                <div style="display: flex; align-items: start;">
                    <span class="notification-icon">${notif.icon}</span>
                    <div class="notification-content">
                        <div class="notification-title">${notif.title}</div>
                        <div class="notification-body">${notif.body}</div>
                        <div class="notification-time">${notif.time}</div>
                    </div>
                </div>
            </div>
        `;
    },

    getPortalNotifications() {
        const currentPortal = App.currentPortal;
        return mockData.notifications.filter(n =>
            n.portal === currentPortal || n.portal === 'all'
        );
    },

    getUnreadCount() {
        const portalNotifications = this.getPortalNotifications();
        return portalNotifications.filter(n => !n.read).length;
    },

    updateBadge() {
        const badge = document.getElementById('notification-badge');
        const count = this.getUnreadCount();

        if (count > 0) {
            badge.textContent = count;
            badge.style.display = 'block';
        } else {
            badge.style.display = 'none';
        }
    },

    handleClick(notifId) {
        const notif = mockData.notifications.find(n => n.id === notifId);
        if (!notif) return;

        // Mark as read
        notif.read = true;

        // Close dropdown
        this.toggle();

        // Execute action
        if (notif.action) {
            notif.action();
        }

        this.updateBadge();
    },

    markAllRead() {
        const portalNotifications = this.getPortalNotifications();
        portalNotifications.forEach(n => n.read = true);
        this.render();
        Utils.showToast('All notifications marked as read', 'success');
    },

    viewAll() {
        this.toggle();
        Utils.showToast('Full notification history coming soon', 'info');
    },

    // Add notification programmatically
    add(notification) {
        mockData.notifications.unshift({
            id: mockData.notifications.length + 1,
            read: false,
            time: 'Just now',
            ...notification
        });

        this.updateBadge();

        // Show toast for new notification
        Utils.showToast(`📬 ${notification.title}`, 'info');
    }
};

// Close notification dropdown when clicking outside
document.addEventListener('click', (e) => {
    const dropdown = document.getElementById('notification-dropdown');
    const bell = document.getElementById('notification-bell');

    if (Notifications.isOpen &&
        !dropdown.contains(e.target) &&
        !bell.contains(e.target)) {
        Notifications.toggle();
    }
});
