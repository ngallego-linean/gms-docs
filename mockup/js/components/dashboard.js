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
                ${this.renderFundDepletionChart()}
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

    formatCurrency(amount, short = false) {
        if (short && amount >= 1000000) {
            return `$${(amount / 1000000).toFixed(1)}M`;
        }
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
    },

    renderFundDepletionChart() {
        const timeline = mockData.fundDepletionTimeline;
        if (!timeline || timeline.length === 0) {
            return '<div class="chart-placeholder">No data available</div>';
        }

        // Chart dimensions
        const width = 1000;
        const height = 300;
        const padding = { top: 20, right: 120, bottom: 60, left: 80 };
        const chartWidth = width - padding.left - padding.right;
        const chartHeight = height - padding.top - padding.bottom;

        // Find min/max values for scaling
        const maxFunds = Math.max(...timeline.map(d => d.remainingFunds));
        const minFunds = 0;

        // Create SVG path for the line
        const points = timeline.map((d, i) => {
            const x = padding.left + (i / (timeline.length - 1)) * chartWidth;
            const y = padding.top + chartHeight - ((d.remainingFunds - minFunds) / (maxFunds - minFunds)) * chartHeight;
            return { x, y, data: d };
        });

        const linePath = points.map((p, i) =>
            `${i === 0 ? 'M' : 'L'} ${p.x},${p.y}`
        ).join(' ');

        // Create area fill path
        const areaPath = `M ${padding.left},${padding.top + chartHeight} ` +
            points.map(p => `L ${p.x},${p.y}`).join(' ') +
            ` L ${padding.left + chartWidth},${padding.top + chartHeight} Z`;

        // Generate Y-axis labels
        const yAxisSteps = 5;
        const yAxisLabels = Array.from({ length: yAxisSteps + 1 }, (_, i) => {
            const value = maxFunds - (i * maxFunds / yAxisSteps);
            const y = padding.top + (i * chartHeight / yAxisSteps);
            return { value, y };
        });

        // Generate X-axis labels (show every other point to avoid crowding)
        const xAxisLabels = points.filter((_, i) => i % 2 === 0 || i === points.length - 1).map(p => ({
            x: p.x,
            label: new Date(p.data.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric' }),
            isCurrent: p.data.label.includes('Current')
        }));

        return `
            <div class="fund-chart-wrapper">
                <svg viewBox="0 0 ${width} ${height}" class="fund-chart">
                    <!-- Grid lines -->
                    ${yAxisLabels.map(label => `
                        <line x1="${padding.left}" y1="${label.y}"
                              x2="${padding.left + chartWidth}" y2="${label.y}"
                              stroke="#E9ECEF" stroke-width="1" />
                    `).join('')}

                    <!-- Area fill -->
                    <path d="${areaPath}"
                          fill="url(#gradient)"
                          opacity="0.3" />

                    <!-- Line -->
                    <path d="${linePath}"
                          stroke="#0066CC"
                          stroke-width="3"
                          fill="none"
                          stroke-linecap="round"
                          stroke-linejoin="round" />

                    <!-- Data points -->
                    ${points.map((p, i) => `
                        <circle cx="${p.x}" cy="${p.y}" r="${i === points.length - 1 ? '6' : '4'}"
                                fill="${i === points.length - 1 ? '#0066CC' : '#FFFFFF'}"
                                stroke="#0066CC"
                                stroke-width="2"
                                class="chart-point"
                                data-index="${i}">
                            <title>${p.data.label}: ${this.formatCurrency(p.data.remainingFunds)}</title>
                        </circle>
                    `).join('')}

                    <!-- Y-axis -->
                    <line x1="${padding.left}" y1="${padding.top}"
                          x2="${padding.left}" y2="${padding.top + chartHeight}"
                          stroke="#6C757D" stroke-width="2" />

                    <!-- Y-axis labels -->
                    ${yAxisLabels.map(label => `
                        <text x="${padding.left - 10}" y="${label.y + 5}"
                              text-anchor="end"
                              font-size="12"
                              fill="#6C757D">
                            ${this.formatCurrency(label.value, true)}
                        </text>
                    `).join('')}

                    <!-- X-axis -->
                    <line x1="${padding.left}" y1="${padding.top + chartHeight}"
                          x2="${padding.left + chartWidth}" y2="${padding.top + chartHeight}"
                          stroke="#6C757D" stroke-width="2" />

                    <!-- X-axis labels -->
                    ${xAxisLabels.map(label => `
                        <text x="${label.x}" y="${padding.top + chartHeight + 25}"
                              text-anchor="middle"
                              font-size="12"
                              fill="${label.isCurrent ? '#0066CC' : '#6C757D'}"
                              font-weight="${label.isCurrent ? 'bold' : 'normal'}">
                            ${label.label}
                        </text>
                    `).join('')}

                    <!-- Current marker -->
                    ${(() => {
                        const current = points[points.length - 1];
                        return `
                            <line x1="${current.x}" y1="${padding.top}"
                                  x2="${current.x}" y2="${padding.top + chartHeight}"
                                  stroke="#0066CC"
                                  stroke-width="2"
                                  stroke-dasharray="5,5"
                                  opacity="0.5" />
                            <text x="${current.x + 10}" y="${current.y - 10}"
                                  font-size="14"
                                  font-weight="bold"
                                  fill="#0066CC">
                                TODAY
                            </text>
                        `;
                    })()}

                    <!-- Gradient definition -->
                    <defs>
                        <linearGradient id="gradient" x1="0%" y1="0%" x2="0%" y2="100%">
                            <stop offset="0%" style="stop-color:#0066CC;stop-opacity:0.5" />
                            <stop offset="100%" style="stop-color:#0066CC;stop-opacity:0.1" />
                        </linearGradient>
                    </defs>

                    <!-- Y-axis label -->
                    <text x="20" y="${padding.top + chartHeight / 2}"
                          text-anchor="middle"
                          font-size="13"
                          font-weight="600"
                          fill="#495057"
                          transform="rotate(-90, 20, ${padding.top + chartHeight / 2})">
                        Remaining Funds
                    </text>

                    <!-- X-axis label -->
                    <text x="${padding.left + chartWidth / 2}" y="${height - 10}"
                          text-anchor="middle"
                          font-size="13"
                          font-weight="600"
                          fill="#495057">
                        Timeline (FY 2025-26)
                    </text>
                </svg>

                <!-- Legend -->
                <div class="chart-legend">
                    <div class="legend-item">
                        <div class="legend-color" style="background: linear-gradient(135deg, rgba(0,102,204,0.5) 0%, rgba(0,102,204,0.1) 100%);"></div>
                        <span>Funds Depleted</span>
                    </div>
                    <div class="legend-item">
                        <div class="legend-color" style="background: #0066CC;"></div>
                        <span>Remaining Funds</span>
                    </div>
                    <div class="legend-item">
                        <div class="legend-marker" style="border: 2px dashed #0066CC;"></div>
                        <span>Current Date</span>
                    </div>
                </div>

                <!-- Summary stats below chart -->
                <div class="chart-summary">
                    <div class="summary-stat">
                        <div class="summary-label">Depletion Rate</div>
                        <div class="summary-value">${this.calculateDepletionRate(timeline)}</div>
                    </div>
                    <div class="summary-stat">
                        <div class="summary-label">Projected Depletion</div>
                        <div class="summary-value">${this.calculateProjectedDepletion(timeline)}</div>
                    </div>
                    <div class="summary-stat">
                        <div class="summary-label">Total Committed</div>
                        <div class="summary-value">${this.formatCurrency(timeline[0].remainingFunds - timeline[timeline.length - 1].remainingFunds)}</div>
                    </div>
                </div>
            </div>
        `;
    },

    calculateDepletionRate(timeline) {
        if (timeline.length < 2) return 'N/A';

        const start = new Date(timeline[0].date);
        const current = new Date(timeline[timeline.length - 1].date);
        const daysElapsed = Math.floor((current - start) / (1000 * 60 * 60 * 24));
        const fundsUsed = timeline[0].remainingFunds - timeline[timeline.length - 1].remainingFunds;
        const dailyRate = fundsUsed / daysElapsed;

        return this.formatCurrency(dailyRate) + '/day';
    },

    calculateProjectedDepletion(timeline) {
        if (timeline.length < 2) return 'N/A';

        const start = new Date(timeline[0].date);
        const current = new Date(timeline[timeline.length - 1].date);
        const daysElapsed = Math.floor((current - start) / (1000 * 60 * 60 * 24));
        const fundsUsed = timeline[0].remainingFunds - timeline[timeline.length - 1].remainingFunds;
        const dailyRate = fundsUsed / daysElapsed;
        const remainingFunds = timeline[timeline.length - 1].remainingFunds;
        const daysRemaining = Math.floor(remainingFunds / dailyRate);

        const projectedDate = new Date(current);
        projectedDate.setDate(projectedDate.getDate() + daysRemaining);

        return projectedDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
    }
};
