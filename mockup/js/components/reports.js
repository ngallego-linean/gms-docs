// Reports Component - IHE Completion & LEA Payment Reports

const Reports = {
    currentReport: null,
    editingReportId: null,

    render(portal) {
        if (this.editingReportId) {
            return this.renderReportForm(this.editingReportId, portal);
        }

        const reports = this.getReportsForPortal(portal);

        if (portal === 'staff') {
            return this.renderStaffView(reports);
        } else if (portal === 'ihe') {
            return this.renderIHEView(reports);
        } else if (portal === 'lea') {
            return this.renderLEAView(reports);
        }
    },

    getReportsForPortal(portal) {
        const currentUser = mockData.portalUsers[portal];
        const allReports = [];

        if (portal === 'staff') {
            // Staff sees all reports with student info
            mockData.reports.forEach(report => {
                const student = dataHelpers.getStudent(report.studentId);
                const application = dataHelpers.getApplication(student.applicationId);
                allReports.push({
                    ...report,
                    student,
                    application
                });
            });
        } else if (portal === 'ihe') {
            // IHE sees their completion reports
            const iheApps = mockData.applications.filter(a => a.iheId === currentUser.organizationId);
            const iheStudentIds = mockData.students
                .filter(s => iheApps.some(a => a.id === s.applicationId))
                .map(s => s.id);

            mockData.reports
                .filter(r => iheStudentIds.includes(r.studentId) && r.reportType === 'IHE_COMPLETION')
                .forEach(report => {
                    const student = dataHelpers.getStudent(report.studentId);
                    const application = dataHelpers.getApplication(student.applicationId);
                    allReports.push({
                        ...report,
                        student,
                        application
                    });
                });
        } else if (portal === 'lea') {
            // LEA sees their payment reports
            const leaApps = mockData.applications.filter(a => a.leaId === currentUser.organizationId);
            const leaStudentIds = mockData.students
                .filter(s => leaApps.some(a => a.id === s.applicationId))
                .map(s => s.id);

            mockData.reports
                .filter(r => leaStudentIds.includes(r.studentId) && r.reportType === 'LEA_PAYMENT')
                .forEach(report => {
                    const student = dataHelpers.getStudent(report.studentId);
                    const application = dataHelpers.getApplication(student.applicationId);
                    allReports.push({
                        ...report,
                        student,
                        application
                    });
                });
        }

        return allReports;
    },

    renderStaffView(reports) {
        const iheReports = reports.filter(r => r.reportType === 'IHE_COMPLETION');
        const leaReports = reports.filter(r => r.reportType === 'LEA_PAYMENT');

        const iheSubmitted = iheReports.filter(r => r.status === 'SUBMITTED').length;
        const leaSubmitted = leaReports.filter(r => r.status === 'SUBMITTED').length;
        const totalStudents = new Set(reports.map(r => r.studentId)).size;

        return `
            <div class="page-header">
                <h2>Reports Overview</h2>
                <p class="subtitle">IHE completion reports and LEA payment reports</p>
            </div>

            <div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));">
                <div class="metric-card">
                    <div class="metric-label">IHE Reports</div>
                    <div class="metric-value">${iheSubmitted}/${iheReports.length}</div>
                    <div class="metric-subtext">Submitted (${Math.round((iheSubmitted / iheReports.length) * 100)}%)</div>
                </div>
                <div class="metric-card">
                    <div class="metric-label">LEA Reports</div>
                    <div class="metric-value">${leaSubmitted}/${leaReports.length}</div>
                    <div class="metric-subtext">Submitted (${Math.round((leaSubmitted / leaReports.length) * 100)}%)</div>
                </div>
                <div class="metric-card">
                    <div class="metric-label">Total Students</div>
                    <div class="metric-value">${totalStudents}</div>
                    <div class="metric-subtext">With reports in system</div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">Report Status by Student</div>
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Student Name</th>
                            <th>IHE</th>
                            <th>LEA</th>
                            <th>IHE Report</th>
                            <th>LEA Report</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${this.renderStaffReportRows(reports)}
                    </tbody>
                </table>
            </div>
        `;
    },

    renderStaffReportRows(reports) {
        // Group reports by student
        const studentReports = {};
        reports.forEach(report => {
            if (!studentReports[report.studentId]) {
                studentReports[report.studentId] = {
                    student: report.student,
                    application: report.application,
                    iheReport: null,
                    leaReport: null
                };
            }
            if (report.reportType === 'IHE_COMPLETION') {
                studentReports[report.studentId].iheReport = report;
            } else {
                studentReports[report.studentId].leaReport = report;
            }
        });

        return Object.values(studentReports).map(({ student, application, iheReport, leaReport }) => {
            const iheStatus = iheReport?.status || 'NOT_STARTED';
            const leaStatus = leaReport?.status || 'NOT_STARTED';

            let overallStatus = 'complete';
            if (iheStatus === 'NOT_STARTED' || leaStatus === 'NOT_STARTED' ||
                iheStatus === 'DRAFT' || leaStatus === 'DRAFT') {
                overallStatus = 'incomplete';
            }

            return `
                <tr>
                    <td><strong>${student.firstName} ${student.lastName}</strong></td>
                    <td>${application.ihe.name}</td>
                    <td>${application.lea.name}</td>
                    <td>
                        ${this.renderReportStatusBadge(iheStatus)}
                        ${iheReport && iheStatus === 'SUBMITTED' ?
                            `<button class="btn btn-sm btn-secondary" onclick="Reports.viewReport(${iheReport.id})">View</button>` : ''
                        }
                    </td>
                    <td>
                        ${this.renderReportStatusBadge(leaStatus)}
                        ${leaReport && leaStatus === 'SUBMITTED' ?
                            `<button class="btn btn-sm btn-secondary" onclick="Reports.viewReport(${leaReport.id})">View</button>` : ''
                        }
                    </td>
                    <td><span class="badge badge-${overallStatus === 'complete' ? 'approved' : 'pending'}">${overallStatus === 'complete' ? 'Complete' : 'Incomplete'}</span></td>
                </tr>
            `;
        }).join('');
    },

    renderReportStatusBadge(status) {
        const badges = {
            'SUBMITTED': '<span class="badge badge-approved">Submitted</span>',
            'DRAFT': '<span class="badge badge-draft">Draft</span>',
            'NOT_STARTED': '<span class="badge badge-pending">Not Started</span>'
        };
        return badges[status] || '';
    },

    renderIHEView(reports) {
        const submitted = reports.filter(r => r.status === 'SUBMITTED').length;
        const draft = reports.filter(r => r.status === 'DRAFT').length;
        const pending = reports.length - submitted - draft;

        return `
            <div class="page-header">
                <h2>IHE Completion Reports</h2>
                <p class="subtitle">Report on student teacher completion and employment</p>
            </div>

            <div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));">
                <div class="metric-card">
                    <div class="metric-label">Submitted</div>
                    <div class="metric-value">${submitted}</div>
                </div>
                <div class="metric-card">
                    <div class="metric-label">Draft</div>
                    <div class="metric-value">${draft}</div>
                </div>
                <div class="metric-card">
                    <div class="metric-label">Not Started</div>
                    <div class="metric-value">${pending}</div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">Your Reports</div>
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Student Name</th>
                            <th>LEA</th>
                            <th>Due Date</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${reports.map(r => this.renderIHEReportRow(r)).join('')}
                    </tbody>
                </table>
            </div>
        `;
    },

    renderIHEReportRow(report) {
        const dueDate = new Date(report.dueDate).toLocaleDateString();
        const isOverdue = new Date(report.dueDate) < new Date() && report.status !== 'SUBMITTED';

        return `
            <tr>
                <td><strong>${report.student.firstName} ${report.student.lastName}</strong></td>
                <td>${report.application.lea.name}</td>
                <td ${isOverdue ? 'class="text-error"' : ''}>${dueDate} ${isOverdue ? '(Overdue)' : ''}</td>
                <td>${this.renderReportStatusBadge(report.status)}</td>
                <td>
                    ${report.status === 'SUBMITTED' ?
                        `<button class="btn btn-sm btn-secondary" onclick="Reports.viewReport(${report.id})">View</button>` :
                        `<button class="btn btn-sm btn-primary" onclick="Reports.editReport(${report.id})">
                            ${report.status === 'DRAFT' ? 'Continue' : 'Start Report'}
                        </button>`
                    }
                </td>
            </tr>
        `;
    },

    renderLEAView(reports) {
        const submitted = reports.filter(r => r.status === 'SUBMITTED').length;
        const draft = reports.filter(r => r.status === 'DRAFT').length;
        const pending = reports.length - submitted - draft;

        return `
            <div class="page-header">
                <h2>LEA Payment Reports</h2>
                <p class="subtitle">Report on student teacher payments and employment outcomes</p>
            </div>

            <div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));">
                <div class="metric-card">
                    <div class="metric-label">Submitted</div>
                    <div class="metric-value">${submitted}</div>
                </div>
                <div class="metric-card">
                    <div class="metric-label">Draft</div>
                    <div class="metric-value">${draft}</div>
                </div>
                <div class="metric-card">
                    <div class="metric-label">Not Started</div>
                    <div class="metric-value">${pending}</div>
                </div>
            </div>

            <div class="card">
                <div class="card-header">Your Reports</div>
                <table class="data-table">
                    <thead>
                        <tr>
                            <th>Student Name</th>
                            <th>IHE</th>
                            <th>Award Amount</th>
                            <th>Due Date</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${reports.map(r => this.renderLEAReportRow(r)).join('')}
                    </tbody>
                </table>
            </div>
        `;
    },

    renderLEAReportRow(report) {
        const dueDate = new Date(report.dueDate).toLocaleDateString();
        const isOverdue = new Date(report.dueDate) < new Date() && report.status !== 'SUBMITTED';
        const award = dataHelpers.getAwardByStudent(report.studentId);

        return `
            <tr>
                <td><strong>${report.student.firstName} ${report.student.lastName}</strong></td>
                <td>${report.application.ihe.name}</td>
                <td>${award ? '$' + award.amount.toLocaleString() : 'N/A'}</td>
                <td ${isOverdue ? 'class="text-error"' : ''}>${dueDate} ${isOverdue ? '(Overdue)' : ''}</td>
                <td>${this.renderReportStatusBadge(report.status)}</td>
                <td>
                    ${report.status === 'SUBMITTED' ?
                        `<button class="btn btn-sm btn-secondary" onclick="Reports.viewReport(${report.id})">View</button>` :
                        `<button class="btn btn-sm btn-primary" onclick="Reports.editReport(${report.id})">
                            ${report.status === 'DRAFT' ? 'Continue' : 'Start Report'}
                        </button>`
                    }
                </td>
            </tr>
        `;
    },

    renderReportForm(reportId, portal) {
        const report = mockData.reports.find(r => r.id === reportId);
        if (!report) return '<div class="card"><p>Report not found.</p></div>';

        const student = dataHelpers.getStudent(report.studentId);
        const application = dataHelpers.getApplication(student.applicationId);

        if (report.reportType === 'IHE_COMPLETION') {
            return this.renderIHECompletionForm(report, student, application);
        } else {
            return this.renderLEAPaymentForm(report, student, application);
        }
    },

    renderIHECompletionForm(report, student, application) {
        const data = report.data || {};
        const isSubmitted = report.status === 'SUBMITTED';

        return `
            <div class="page-header flex-between">
                <div>
                    <button class="btn btn-secondary" onclick="Reports.cancelEdit()">← Back to Reports</button>
                    <h2 style="margin-top: 1rem;">IHE Completion Report</h2>
                    <p class="subtitle">${student.firstName} ${student.lastName} - ${application.lea.name}</p>
                </div>
            </div>

            <div class="card">
                <div class="form-section">
                    <h3>Program Completion</h3>

                    <div class="form-group">
                        <label>Did the student complete the program?</label>
                        <select id="completedProgram" ${isSubmitted ? 'disabled' : ''}>
                            <option value="true" ${data.completedProgram === true ? 'selected' : ''}>Yes</option>
                            <option value="false" ${data.completedProgram === false ? 'selected' : ''}>No</option>
                        </select>
                    </div>

                    <div class="form-group" id="completionDateGroup" style="display: ${data.completedProgram === true ? 'block' : 'none'};">
                        <label>Completion Date</label>
                        <input type="date" id="completionDate" value="${data.completionDate || ''}" ${isSubmitted ? 'disabled' : ''}>
                    </div>
                </div>

                <div class="form-section">
                    <h3>Credential Information</h3>

                    <div class="form-group">
                        <label>Did the student earn a credential?</label>
                        <select id="credentialEarned" ${isSubmitted ? 'disabled' : ''}>
                            <option value="true" ${data.credentialEarned === true ? 'selected' : ''}>Yes</option>
                            <option value="false" ${data.credentialEarned === false ? 'selected' : ''}>No</option>
                        </select>
                    </div>

                    <div id="credentialFields" style="display: ${data.credentialEarned === true ? 'block' : 'none'};">
                        <div class="form-group">
                            <label>Credential Area</label>
                            <select id="credentialArea" ${isSubmitted ? 'disabled' : ''}>
                                <option value="">Select...</option>
                                <option value="Multiple Subject" ${data.credentialArea === 'Multiple Subject' ? 'selected' : ''}>Multiple Subject</option>
                                <option value="Single Subject - Mathematics" ${data.credentialArea === 'Single Subject - Mathematics' ? 'selected' : ''}>Single Subject - Mathematics</option>
                                <option value="Single Subject - English" ${data.credentialArea === 'Single Subject - English' ? 'selected' : ''}>Single Subject - English</option>
                                <option value="Single Subject - Science" ${data.credentialArea === 'Single Subject - Science' ? 'selected' : ''}>Single Subject - Science</option>
                                <option value="Education Specialist" ${data.credentialArea === 'Education Specialist' ? 'selected' : ''}>Education Specialist</option>
                            </select>
                        </div>

                        <div class="form-group">
                            <label>Credential Number</label>
                            <input type="text" id="credentialNumber" value="${data.credentialNumber || ''}" ${isSubmitted ? 'disabled' : ''}>
                        </div>

                        <div class="form-group">
                            <label>Did student remain in original credential area?</label>
                            <select id="remainedInCredentialArea" ${isSubmitted ? 'disabled' : ''}>
                                <option value="true" ${data.remainedInCredentialArea === true ? 'selected' : ''}>Yes</option>
                                <option value="false" ${data.remainedInCredentialArea === false ? 'selected' : ''}>No</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="form-section">
                    <h3>Employment Information</h3>

                    <div class="form-group">
                        <label>Employment Status</label>
                        <select id="employmentStatus" ${isSubmitted ? 'disabled' : ''}>
                            <option value="">Select...</option>
                            <option value="Employed in education" ${data.employmentStatus === 'Employed in education' ? 'selected' : ''}>Employed in education</option>
                            <option value="Not employed in education" ${data.employmentStatus === 'Not employed in education' ? 'selected' : ''}>Not employed in education</option>
                            <option value="Unknown" ${data.employmentStatus === 'Unknown' ? 'selected' : ''}>Unknown</option>
                        </select>
                    </div>

                    <div id="employmentFields" style="display: ${data.employmentStatus === 'Employed in education' ? 'block' : 'none'};">
                        <div class="form-group">
                            <label>Employment LEA</label>
                            <input type="text" id="employmentLEA" value="${data.employmentLEA || ''}" ${isSubmitted ? 'disabled' : ''}>
                        </div>

                        <div class="form-group">
                            <label>Employment Position</label>
                            <input type="text" id="employmentPosition" value="${data.employmentPosition || ''}" ${isSubmitted ? 'disabled' : ''}>
                        </div>
                    </div>
                </div>

                <div class="form-section">
                    <div class="form-group">
                        <label>Additional Notes (optional)</label>
                        <textarea id="notes" rows="4" ${isSubmitted ? 'disabled' : ''}>${data.notes || ''}</textarea>
                    </div>
                </div>

                ${!isSubmitted ? `
                    <div class="form-actions">
                        <button class="btn btn-secondary" onclick="Reports.cancelEdit()">Cancel</button>
                        <button class="btn btn-secondary" onclick="Reports.saveDraft(${report.id})">Save Draft</button>
                        <button class="btn btn-primary" onclick="Reports.submitReport(${report.id})">Submit Report</button>
                    </div>
                ` : `
                    <div class="form-actions">
                        <button class="btn btn-secondary" onclick="Reports.cancelEdit()">Back to Reports</button>
                    </div>
                `}
            </div>

            <script>
                // Show/hide conditional fields
                document.getElementById('completedProgram').addEventListener('change', (e) => {
                    document.getElementById('completionDateGroup').style.display =
                        e.target.value === 'true' ? 'block' : 'none';
                });

                document.getElementById('credentialEarned').addEventListener('change', (e) => {
                    document.getElementById('credentialFields').style.display =
                        e.target.value === 'true' ? 'block' : 'none';
                });

                document.getElementById('employmentStatus').addEventListener('change', (e) => {
                    document.getElementById('employmentFields').style.display =
                        e.target.value === 'Employed in education' ? 'block' : 'none';
                });
            </script>
        `;
    },

    renderLEAPaymentForm(report, student, application) {
        const data = report.data || {};
        const isSubmitted = report.status === 'SUBMITTED';
        const award = dataHelpers.getAwardByStudent(student.id);

        return `
            <div class="page-header flex-between">
                <div>
                    <button class="btn btn-secondary" onclick="Reports.cancelEdit()">← Back to Reports</button>
                    <h2 style="margin-top: 1rem;">LEA Payment Report</h2>
                    <p class="subtitle">${student.firstName} ${student.lastName} - ${application.ihe.name}</p>
                </div>
            </div>

            <div class="card">
                <div class="form-section">
                    <h3>Payment Information</h3>

                    <div class="form-group">
                        <label>Award Amount</label>
                        <input type="text" value="$${award ? award.amount.toLocaleString() : '10,000'}" disabled>
                    </div>

                    <div class="form-group">
                        <label>How was student teacher categorized for payment?</label>
                        <select id="paymentCategory" ${isSubmitted ? 'disabled' : ''}>
                            <option value="">Select...</option>
                            <option value="Classified" ${data.paymentCategory === 'Classified' ? 'selected' : ''}>Classified</option>
                            <option value="Certificated" ${data.paymentCategory === 'Certificated' ? 'selected' : ''}>Certificated</option>
                            <option value="Other" ${data.paymentCategory === 'Other' ? 'selected' : ''}>Other</option>
                        </select>
                    </div>

                    <div class="form-group">
                        <label>Payment Schedule</label>
                        <select id="paymentSchedule" ${isSubmitted ? 'disabled' : ''}>
                            <option value="">Select...</option>
                            <option value="One payment" ${data.paymentSchedule === 'One payment' ? 'selected' : ''}>One payment</option>
                            <option value="Two payments" ${data.paymentSchedule === 'Two payments' ? 'selected' : ''}>Two payments</option>
                            <option value="Monthly" ${data.paymentSchedule === 'Monthly' ? 'selected' : ''}>Monthly</option>
                        </select>
                    </div>

                    <div class="form-group">
                        <label>Total Amount Paid to Student Teacher</label>
                        <input type="number" id="paymentAmount" value="${data.paymentAmount || ''}" ${isSubmitted ? 'disabled' : ''}>
                    </div>

                    <div class="form-group">
                        <label>Payment Date (or final payment if multiple)</label>
                        <input type="date" id="paymentDate" value="${data.paymentDate || ''}" ${isSubmitted ? 'disabled' : ''}>
                    </div>
                </div>

                <div class="form-section">
                    <h3>Eligibility Confirmation</h3>

                    <div class="form-group">
                        <label>Hours Completed</label>
                        <input type="number" id="hoursCompleted" value="${data.hoursCompleted || ''}" ${isSubmitted ? 'disabled' : ''}>
                        <small>Minimum 540 hours required</small>
                    </div>

                    <div class="form-group">
                        <label>
                            <input type="checkbox" id="eligibilityConfirmed" ${data.eligibilityConfirmed ? 'checked' : ''} ${isSubmitted ? 'disabled' : ''}>
                            I confirm the student teacher completed 540+ hours and met all eligibility requirements
                        </label>
                    </div>
                </div>

                <div class="form-section">
                    <h3>Employment Outcome</h3>

                    <div class="form-group">
                        <label>Was the student teacher hired by your LEA?</label>
                        <select id="studentTeacherHired" ${isSubmitted ? 'disabled' : ''}>
                            <option value="true" ${data.studentTeacherHired === true ? 'selected' : ''}>Yes</option>
                            <option value="false" ${data.studentTeacherHired === false ? 'selected' : ''}>No</option>
                        </select>
                    </div>

                    <div class="form-group" id="hireDateGroup" style="display: ${data.studentTeacherHired === true ? 'block' : 'none'};">
                        <label>Hire Date</label>
                        <input type="date" id="hireDate" value="${data.hireDate || ''}" ${isSubmitted ? 'disabled' : ''}>
                    </div>
                </div>

                <div class="form-section">
                    <div class="form-group">
                        <label>Additional Notes (optional)</label>
                        <textarea id="notes" rows="4" ${isSubmitted ? 'disabled' : ''}>${data.notes || ''}</textarea>
                    </div>
                </div>

                ${!isSubmitted ? `
                    <div class="form-actions">
                        <button class="btn btn-secondary" onclick="Reports.cancelEdit()">Cancel</button>
                        <button class="btn btn-secondary" onclick="Reports.saveDraft(${report.id})">Save Draft</button>
                        <button class="btn btn-primary" onclick="Reports.submitReport(${report.id})">Submit Report</button>
                    </div>
                ` : `
                    <div class="form-actions">
                        <button class="btn btn-secondary" onclick="Reports.cancelEdit()">Back to Reports</button>
                    </div>
                `}
            </div>

            <script>
                // Show/hide conditional fields
                document.getElementById('studentTeacherHired').addEventListener('change', (e) => {
                    document.getElementById('hireDateGroup').style.display =
                        e.target.value === 'true' ? 'block' : 'none';
                });
            </script>
        `;
    },

    editReport(reportId) {
        this.editingReportId = reportId;
        App.renderCurrentView();
    },

    viewReport(reportId) {
        this.editingReportId = reportId;
        App.renderCurrentView();
    },

    cancelEdit() {
        this.editingReportId = null;
        App.renderCurrentView();
    },

    saveDraft(reportId) {
        const report = mockData.reports.find(r => r.id === reportId);
        if (!report) return;

        // Collect form data
        const formData = this.collectFormData(report.reportType);

        // Update report
        report.data = formData;
        report.status = 'DRAFT';

        Utils.showToast('Report saved as draft', 'success');
        this.cancelEdit();
    },

    submitReport(reportId) {
        const report = mockData.reports.find(r => r.id === reportId);
        if (!report) return;

        // Collect form data
        const formData = this.collectFormData(report.reportType);

        // Validate required fields
        if (!this.validateReportData(formData, report.reportType)) {
            Utils.showToast('Please fill in all required fields', 'error');
            return;
        }

        // Update report
        report.data = formData;
        report.status = 'SUBMITTED';
        report.submittedAt = new Date().toISOString();
        report.submittedBy = mockData.currentUser.email;

        Utils.showToast('Report submitted successfully!', 'success');
        this.cancelEdit();
    },

    collectFormData(reportType) {
        if (reportType === 'IHE_COMPLETION') {
            return {
                completedProgram: document.getElementById('completedProgram').value === 'true',
                completionDate: document.getElementById('completionDate').value,
                credentialEarned: document.getElementById('credentialEarned').value === 'true',
                credentialArea: document.getElementById('credentialArea').value,
                credentialNumber: document.getElementById('credentialNumber').value,
                remainedInCredentialArea: document.getElementById('remainedInCredentialArea').value === 'true',
                employmentStatus: document.getElementById('employmentStatus').value,
                employmentLEA: document.getElementById('employmentLEA').value,
                employmentPosition: document.getElementById('employmentPosition').value,
                notes: document.getElementById('notes').value
            };
        } else {
            return {
                paymentCategory: document.getElementById('paymentCategory').value,
                paymentSchedule: document.getElementById('paymentSchedule').value,
                paymentAmount: parseFloat(document.getElementById('paymentAmount').value),
                paymentDate: document.getElementById('paymentDate').value,
                hoursCompleted: parseInt(document.getElementById('hoursCompleted').value),
                eligibilityConfirmed: document.getElementById('eligibilityConfirmed').checked,
                studentTeacherHired: document.getElementById('studentTeacherHired').value === 'true',
                hireDate: document.getElementById('hireDate').value,
                notes: document.getElementById('notes').value
            };
        }
    },

    validateReportData(data, reportType) {
        if (reportType === 'IHE_COMPLETION') {
            if (data.completedProgram && !data.completionDate) return false;
            if (data.credentialEarned && (!data.credentialArea || !data.credentialNumber)) return false;
            return true;
        } else {
            if (!data.paymentCategory || !data.paymentSchedule || !data.paymentAmount || !data.paymentDate) return false;
            if (!data.hoursCompleted || data.hoursCompleted < 540) return false;
            if (!data.eligibilityConfirmed) return false;
            return true;
        }
    }
};
