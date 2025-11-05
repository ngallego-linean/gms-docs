// Main Application Controller

const App = {
    currentPortal: 'staff',
    currentView: 'dashboard',

    init() {
        this.attachGlobalEventListeners();
        Notifications.updateBadge();
        this.renderCurrentView();
    },

    attachGlobalEventListeners() {
        // Portal switcher
        document.querySelectorAll('.portal-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                this.switchPortal(e.target.dataset.portal);
            });
        });

        // Grant cycle selector (will be rendered in sidebar)
        document.addEventListener('change', (e) => {
            if (e.target.id === 'grant-cycle-select') {
                this.renderCurrentView();
            }
        });
    },

    switchPortal(portal) {
        // Handle public portal separately
        if (portal === 'public') {
            this.showPublicPortal();
            return;
        }

        // Restore sidebar if coming from public portal
        if (this.currentPortal === 'public') {
            document.getElementById('sidebar').style.display = 'block';
        }

        this.currentPortal = portal;

        // Update current user
        mockData.currentUser = mockData.portalUsers[portal];
        document.getElementById('current-user-name').textContent = mockData.currentUser.name;

        // Update active button
        document.querySelectorAll('.portal-btn').forEach(btn => {
            btn.classList.remove('active');
            if (btn.dataset.portal === portal) {
                btn.classList.add('active');
            }
        });

        // Clear application detail view when switching portals
        Applications.currentApplication = null;

        // Reset to dashboard view for new portal
        this.currentView = 'dashboard';
        this.renderCurrentView();

        Utils.showToast(`Switched to ${portal.toUpperCase()} Portal`, 'info');
    },

    renderCurrentView() {
        this.renderSidebar();
        this.renderContent();
    },

    renderSidebar() {
        const sidebar = document.getElementById('sidebar');

        const staffMenu = `
            <div class="sidebar-section">
                <div class="grant-cycle-selector">
                    <label>Grant Cycle</label>
                    <select id="grant-cycle-select">
                        ${mockData.grantCycles.map(gc => `
                            <option value="${gc.id}">${gc.name}</option>
                        `).join('')}
                    </select>
                </div>
            </div>
            <div class="sidebar-section">
                <div class="sidebar-title">Navigation</div>
                <ul class="sidebar-nav">
                    <li><a href="#" class="${this.currentView === 'dashboard' ? 'active' : ''}" data-view="dashboard">Dashboard</a></li>
                    <li><a href="#" class="${this.currentView === 'applications' ? 'active' : ''}" data-view="applications">Applications</a></li>
                    <li><a href="#" class="${this.currentView === 'awards' ? 'active' : ''}" data-view="awards">Awards & GAAs</a></li>
                    <li><a href="#" class="${this.currentView === 'reports' ? 'active' : ''}" data-view="reports">Reports</a></li>
                </ul>
            </div>
        `;

        const iheMenu = `
            <div class="sidebar-section">
                <div class="sidebar-title">Your Applications</div>
                <ul class="sidebar-nav">
                    <li><a href="#" class="${this.currentView === 'dashboard' ? 'active' : ''}" data-view="dashboard">Dashboard</a></li>
                    <li><a href="#" class="${this.currentView === 'applications' ? 'active' : ''}" data-view="applications">Manage Students</a></li>
                    <li><a href="#" class="${this.currentView === 'reports' ? 'active' : ''}" data-view="reports">Reports</a></li>
                </ul>
            </div>
            <div class="sidebar-section">
                <div class="sidebar-title">Quick Stats</div>
                <div style="padding: 0 1.5rem;">
                    ${this.renderIHEQuickStats()}
                </div>
            </div>
        `;

        const leaMenu = `
            <div class="sidebar-section">
                <div class="sidebar-title">Your Applications</div>
                <ul class="sidebar-nav">
                    <li><a href="#" class="${this.currentView === 'dashboard' ? 'active' : ''}" data-view="dashboard">Dashboard</a></li>
                    <li><a href="#" class="${this.currentView === 'applications' ? 'active' : ''}" data-view="applications">Pending Students</a></li>
                    <li><a href="#" class="${this.currentView === 'reports' ? 'active' : ''}" data-view="reports">Reports</a></li>
                </ul>
            </div>
            <div class="sidebar-section">
                <div class="sidebar-title">Quick Stats</div>
                <div style="padding: 0 1.5rem;">
                    ${this.renderLEAQuickStats()}
                </div>
            </div>
        `;

        const menus = {
            staff: staffMenu,
            ihe: iheMenu,
            lea: leaMenu
        };

        sidebar.innerHTML = menus[this.currentPortal];

        // Attach sidebar nav listeners
        sidebar.querySelectorAll('.sidebar-nav a').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                this.currentView = e.target.dataset.view;
                this.renderCurrentView();
            });
        });
    },

    renderIHEQuickStats() {
        const applications = dataHelpers.getApplicationsByPortal('ihe');
        if (!applications.length) return '<p class="text-muted">No active applications</p>';

        const app = applications[0];
        const totalStudents = app.students.length;
        const approved = app.students.filter(s => s.status === 'APPROVED').length;
        const pending = app.students.filter(s => ['DRAFT', 'PENDING_LEA'].includes(s.status)).length;

        return `
            <p style="margin-bottom: 0.5rem;"><strong>${totalStudents}</strong> total students</p>
            <p style="margin-bottom: 0.5rem;"><strong>${approved}</strong> approved</p>
            <p style="margin-bottom: 0.5rem;"><strong>${pending}</strong> pending action</p>
        `;
    },

    renderLEAQuickStats() {
        const applications = dataHelpers.getApplicationsByPortal('lea');
        if (!applications.length) return '<p class="text-muted">No active applications</p>';

        const app = applications[0];
        const totalStudents = app.students.length;
        const pendingLEA = app.students.filter(s => s.status === 'PENDING_LEA').length;
        const completed = app.students.filter(s => ['SUBMITTED', 'APPROVED'].includes(s.status)).length;

        return `
            <p style="margin-bottom: 0.5rem;"><strong>${totalStudents}</strong> total students</p>
            <p style="margin-bottom: 0.5rem;"><strong>${pendingLEA}</strong> need your info</p>
            <p style="margin-bottom: 0.5rem;"><strong>${completed}</strong> completed</p>
        `;
    },

    renderContent() {
        const contentArea = document.getElementById('content-area');

        let html = '';

        switch (this.currentView) {
            case 'dashboard':
                if (this.currentPortal === 'public') {
                    html = PublicPortal.render();
                } else {
                    html = Dashboard.render(1);
                }
                break;
            case 'applications':
                html = Applications.render(this.currentPortal);
                break;
            case 'awards':
                html = Awards.render(this.currentPortal);
                break;
            case 'reports':
                html = Reports.render(this.currentPortal);
                break;
            default:
                html = '<div class="card"><p>View not found</p></div>';
        }

        contentArea.innerHTML = html;

        // Attach component-specific event listeners
        if (this.currentView === 'dashboard' && this.currentPortal !== 'public') {
            Dashboard.attachEventListeners();
        }
    },

    showPublicPortal() {
        // Hide sidebar for public portal
        document.getElementById('sidebar').style.display = 'none';
        document.getElementById('current-user-name').textContent = 'Guest';

        this.currentPortal = 'public';
        this.currentView = 'dashboard';

        // Update active button
        document.querySelectorAll('.portal-btn').forEach(btn => {
            btn.classList.remove('active');
            if (btn.dataset.portal === 'public') {
                btn.classList.add('active');
            }
        });

        // Render public portal
        document.getElementById('content-area').innerHTML = PublicPortal.render();

        Utils.showToast('Viewing Public Portal', 'info');
    },

    hidePublicPortal() {
        // Restore sidebar
        document.getElementById('sidebar').style.display = 'block';
        this.switchPortal('staff');
    }
};

// Utility Functions
const Utils = {
    showModal(titleOrContent, body = null, buttons = null) {
        const overlay = document.getElementById('modal-overlay');
        const modal = document.getElementById('modal-content');

        // If only one argument and it's a string with HTML, use it directly
        if (body === null && buttons === null && typeof titleOrContent === 'string' && titleOrContent.includes('<')) {
            modal.innerHTML = titleOrContent;
        } else {
            // Build modal with title, body, and buttons
            const buttonsHTML = buttons ? buttons.map(btn =>
                `<button class="btn ${btn.class || 'btn-secondary'}" onclick="${btn.action ? `(${btn.action.toString()})(); Utils.closeModal();` : 'Utils.closeModal();'}">${btn.text}</button>`
            ).join('') : '<button class="btn btn-secondary" onclick="Utils.closeModal()">Close</button>';

            modal.innerHTML = `
                <div class="modal-header">
                    <h3>${titleOrContent}</h3>
                    <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
                </div>
                <div class="modal-body">
                    ${body}
                </div>
                <div class="modal-footer">
                    ${buttonsHTML}
                </div>
            `;
        }

        overlay.classList.remove('hidden');
    },

    closeModal() {
        const overlay = document.getElementById('modal-overlay');
        overlay.classList.add('hidden');
    },

    showToast(message, type = 'info') {
        const container = document.getElementById('toast-container');
        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.textContent = message;

        container.appendChild(toast);

        // Auto-remove after 3 seconds
        setTimeout(() => {
            toast.style.animation = 'slideOut 0.3s ease-out';
            setTimeout(() => {
                container.removeChild(toast);
            }, 300);
        }, 3000);
    }
};

// Close modal when clicking overlay
document.getElementById('modal-overlay').addEventListener('click', (e) => {
    if (e.target.id === 'modal-overlay') {
        Utils.closeModal();
    }
});

// Add slideOut animation to CSS dynamically
const style = document.createElement('style');
style.textContent = `
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Initialize app when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    App.init();
});
