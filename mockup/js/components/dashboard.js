// Dashboard Component

const Dashboard = {
    render(grantCycleId = 1) {
        const metrics = dataHelpers.calculateGrantCycleMetrics(grantCycleId);

        if (!metrics) {
            return '<div class="card"><p>Grant cycle not found.</p></div>';
        }

        const fundStatus = this.getFundStatus(metrics.remainingPercent);

        return `
            <div class="page-header">
                <h2>Dashboard</h2>
                <p class="subtitle">${metrics.name}</p>
            </div>

            ${this.renderActionItems(metrics)}

            ${this.renderFundWarning(metrics)}

            <div class="dashboard-grid">
                ${this.renderMetricCard(
                    'Appropriated',
                    this.formatCurrency(metrics.appropriatedAmount),
                    'Total grant cycle funding'
                )}
                ${this.renderMetricCard(
                    'Reserved',
                    this.formatCurrency(metrics.reservedAmount),
                    'Awards approved, GAA not signed',
                    ''
                )}
                ${this.renderMetricCard(
                    'Encumbered',
                    this.formatCurrency(metrics.encumberedAmount),
                    'GAA signed, payment not completed',
                    ''
                )}
                ${this.renderMetricCard(
                    'Disbursed',
                    this.formatCurrency(metrics.disbursedAmount),
                    'Payments completed',
                    ''
                )}
                ${this.renderMetricCard(
                    'Remaining',
                    this.formatCurrency(metrics.remainingAmount),
                    `${metrics.remainingPercent.toFixed(1)}% available`,
                    fundStatus,
                    this.renderProgressBar(metrics.remainingPercent, fundStatus)
                )}
                ${this.renderMetricCard(
                    'Outstanding Balance',
                    this.formatCurrency(metrics.outstandingBalance),
                    'Committed funds in pipeline'
                )}
            </div>

            <div class="card">
                <div class="card-header">Student Applications by Status</div>
                <div class="status-summary">
                    ${this.renderStatusItem('Draft', metrics.statusCounts.draft, 'badge-draft')}
                    ${this.renderStatusItem('Pending LEA', metrics.statusCounts.pending_lea, 'badge-pending')}
                    ${this.renderStatusItem('Submitted', metrics.statusCounts.submitted, 'badge-submitted')}
                    ${this.renderStatusItem('Under Review', metrics.statusCounts.under_review, 'badge-review')}
                    ${this.renderStatusItem('Approved', metrics.statusCounts.approved, 'badge-approved')}
                    ${this.renderStatusItem('Rejected', metrics.statusCounts.rejected, 'badge-rejected')}
                </div>
            </div>

            <div class="card">
                <div class="card-header">Participant Metrics</div>
                <div class="dashboard-grid">
                    ${this.renderMetricCard('Total Students', metrics.totalStudents, 'Unique SEIDs across all applications')}
                    ${this.renderMetricCard('IHEs Participating', metrics.uniqueIHEs, 'Institutions with active applications')}
                    ${this.renderMetricCard('LEAs Participating', metrics.uniqueLEAs, 'Districts with active applications')}
                </div>
            </div>

            <div class="chart-container">
                <div class="card-header">Fund Depletion Over Time</div>
                <div class="chart-placeholder">
                    Chart: Line graph showing fund depletion from July 2025 to present
                    <br>(Would use Chart.js or similar in production)
                </div>
            </div>
        `;
    },

    renderMetricCard(label, value, subtext, cssClass = '', extraContent = '') {
        return `
            <div class="metric-card ${cssClass}">
                <div class="metric-label">${label}</div>
                <div class="metric-value">${value}</div>
                <div class="metric-subtext">${subtext}</div>
                ${extraContent}
            </div>
        `;
    },

    renderStatusItem(label, count, badgeClass) {
        return `
            <div class="status-item" data-status="${label.toLowerCase().replace(' ', '_')}">
                <span class="count">${count}</span>
                <span class="label">${label}</span>
            </div>
        `;
    },

    renderProgressBar(percent, status) {
        return `
            <div class="progress-bar">
                <div class="progress-fill ${status}" style="width: ${percent}%"></div>
            </div>
        `;
    },

    renderFundWarning(metrics) {
        if (metrics.remainingPercent > 10) {
            return '';
        }

        const alertClass = metrics.remainingPercent < 5 ? 'danger' : '';
        const icon = metrics.remainingPercent < 5 ? '🚨' : '⚠️';

        return `
            <div class="fund-warning-alert ${alertClass}">
                <h4>${icon} Fund Depletion ${alertClass ? 'Critical' : 'Warning'}</h4>
                <p>
                    <strong>Remaining funds:</strong> ${this.formatCurrency(metrics.remainingAmount)}
                    (${metrics.remainingPercent.toFixed(1)}%)
                </p>
                <p>
                    At $10,000 per award, approximately ${Math.floor(metrics.remainingAmount / 10000)} more
                    students can be approved before funds are exhausted.
                </p>
                ${metrics.statusCounts.submitted > 0 ? `
                    <p>
                        <strong>${metrics.statusCounts.submitted} students currently awaiting review.</strong>
                    </p>
                ` : ''}
            </div>
        `;
    },

    getFundStatus(percent) {
        if (percent < 10) return 'low';
        if (percent < 25) return 'warning';
        return 'good';
    },

    formatCurrency(amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).format(amount);
    },

    renderActionItems(metrics) {
        const actionItems = [];

        // Students awaiting review
        if (metrics.statusCounts.submitted > 0) {
            actionItems.push({
                icon: '📋',
                title: `${metrics.statusCounts.submitted} students awaiting your review`,
                action: 'Review Queue',
                onClick: () => { App.currentView = 'applications'; App.renderCurrentView(); }
            });
        }

        // Pending LEA completions
        if (metrics.statusCounts.pending_lea > 0) {
            actionItems.push({
                icon: '⏰',
                title: `${metrics.statusCounts.pending_lea} applications pending LEA information`,
                action: 'Send Reminders',
                onClick: () => Utils.showToast('Reminder emails sent!', 'success')
            });
        }

        // GAAs needing attention
        const gaasReady = mockData.awards.filter(a => a.status === 'GAA_SIGNED').length;
        if (gaasReady > 0) {
            actionItems.push({
                icon: '💰',
                title: `${gaasReady} GAAs fully signed and ready for payment`,
                action: 'Initiate Payments',
                onClick: () => { App.currentView = 'awards'; App.renderCurrentView(); }
            });
        }

        // Fund status insight
        const remainingStudents = Math.floor(metrics.remainingAmount / 10000);
        if (metrics.remainingPercent > 25) {
            actionItems.push({
                icon: '📊',
                title: `Fund Status: ${this.formatCurrency(metrics.remainingAmount)} remaining (${metrics.remainingPercent.toFixed(1)}%) - HEALTHY`,
                action: 'View Fund Analysis',
                subtitle: `Approximately ${remainingStudents} more students can be funded at current rate`,
                onClick: () => Utils.showToast('Detailed fund analysis coming soon', 'info')
            });
        }

        if (actionItems.length === 0) {
            return '';
        }

        return `
            <div class="card" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; margin-bottom: 2rem;">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem;">
                    <h3 style="margin: 0; color: white;">⚡ Action Required</h3>
                    <span style="font-size: 0.9rem; opacity: 0.9;">${actionItems.length} item${actionItems.length > 1 ? 's' : ''}</span>
                </div>
                ${actionItems.map(item => `
                    <div style="background: rgba(255,255,255,0.1); padding: 1rem; margin-bottom: 0.75rem; border-radius: 8px; backdrop-filter: blur(10px);">
                        <div style="display: flex; justify-content: space-between; align-items: start;">
                            <div style="flex: 1;">
                                <div style="font-size: 1.2rem; margin-bottom: 0.25rem;">
                                    ${item.icon} <strong>${item.title}</strong>
                                </div>
                                ${item.subtitle ? `<div style="opacity: 0.8; font-size: 0.9rem; margin-top: 0.25rem;">${item.subtitle}</div>` : ''}
                            </div>
                            <button class="btn btn-secondary btn-sm" onclick='(${item.onClick.toString()})()' style="background: white; color: #667eea; border: none; white-space: nowrap;">
                                ${item.action} →
                            </button>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
    },

    attachEventListeners() {
        // Click status items to filter applications (would navigate in real app)
        document.querySelectorAll('.status-item').forEach(item => {
            item.addEventListener('click', (e) => {
                const status = e.currentTarget.dataset.status;
                Utils.showToast(`Filtering applications by status: ${status}`, 'info');
                // In real app: would switch to Applications view with filter
            });
        });
    }
};
