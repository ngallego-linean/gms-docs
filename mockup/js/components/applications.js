// Applications Component - Batch Student Management

const Applications = {
    currentApplication: null,

    render(portal) {
        const applications = dataHelpers.getApplicationsByPortal(portal);

        if (portal === 'staff') {
            return this.renderStaffView(applications);
        } else if (portal === 'ihe') {
            return this.renderIHEView(applications);
        } else if (portal === 'lea') {
            return this.renderLEAView(applications);
        }
    },

    renderStaffView(applications) {
        return `
            <div class="page-header">
                <h2>Applications</h2>
                <p class="subtitle">All IHE-LEA applications for current grant cycle</p>
            </div>

            <div class="card">
                ${applications.map(app => this.renderApplicationCard(app, 'staff')).join('')}
            </div>
        `;
    },

    renderIHEView(applications) {
        // If viewing a specific application, show student management
        if (this.currentApplication) {
            return this.renderApplicationDetail(this.currentApplication, 'ihe');
        }

        // Otherwise show list of applications
        if (!applications.length) {
            return `
                <div class="page-header">
                    <h2>Your Grant Applications</h2>
                    <p class="subtitle">No active applications found</p>
                </div>
            `;
        }

        return `
            <div class="page-header">
                <h2>Your Grant Applications</h2>
                <p class="subtitle">Select an application to manage students</p>
            </div>

            <div class="card">
                ${applications.map(app => this.renderApplicationCard(app, 'ihe')).join('')}
            </div>
        `;
    },

    renderLEAView(applications) {
        // If viewing a specific application, show student management
        if (this.currentApplication) {
            return this.renderApplicationDetail(this.currentApplication, 'lea');
        }

        // Otherwise show list of applications
        if (!applications.length) {
            return `
                <div class="page-header">
                    <h2>Your Grant Applications</h2>
                    <p class="subtitle">No applications pending your review</p>
                </div>
            `;
        }

        return `
            <div class="page-header">
                <h2>Your Grant Applications</h2>
                <p class="subtitle">Select an application to complete student information</p>
            </div>

            <div class="card">
                ${applications.map(app => this.renderApplicationCard(app, 'lea')).join('')}
            </div>
        `;
    },

    renderApplicationDetail(app, portal) {
        const pendingStudents = app.students.filter(s => s.status === 'PENDING_LEA');
        const draftCount = app.students.filter(s => s.status === 'DRAFT').length;
        const approvedCount = app.students.filter(s => s.status === 'APPROVED').length;
        const totalStudents = app.students.length;

        return `
            <div class="page-header flex-between">
                <div>
                    <button class="btn btn-secondary" onclick="Applications.backToList()">← Back to Applications</button>
                    <h2 style="margin-top: 1rem;">${app.ihe.name} → ${app.lea.name}</h2>
                    <p class="subtitle">Grant Cycle: ${dataHelpers.getGrantCycle(app.grantCycleId).name}</p>
                </div>
                ${portal === 'ihe' ? `
                    <button class="btn btn-primary" onclick="Applications.showAddStudentModal(${app.id})">
                        + Add Student
                    </button>
                ` : ''}
            </div>

            <div class="card">
                <h3 style="margin-bottom: 1rem;">Application Progress</h3>
                <div class="progress-timeline">
                    <div class="progress-step completed">
                        <div class="progress-dot">✓</div>
                        <div class="progress-label">Application Created</div>
                    </div>
                    <div class="progress-step ${draftCount === 0 ? 'completed' : 'current'}">
                        <div class="progress-dot">${draftCount === 0 ? '✓' : draftCount}</div>
                        <div class="progress-label">Adding Students</div>
                    </div>
                    <div class="progress-step ${pendingStudents.length === 0 && draftCount === 0 ? 'completed' : pendingStudents.length > 0 ? 'current' : ''}">
                        <div class="progress-dot">${pendingStudents.length === 0 && draftCount === 0 ? '✓' : pendingStudents.length || '○'}</div>
                        <div class="progress-label">LEA Review</div>
                    </div>
                    <div class="progress-step ${approvedCount > 0 ? 'completed' : ''}">
                        <div class="progress-dot">${approvedCount > 0 ? approvedCount : '○'}</div>
                        <div class="progress-label">CTC Approved</div>
                    </div>
                    <div class="progress-step">
                        <div class="progress-dot">○</div>
                        <div class="progress-label">Awards Issued</div>
                    </div>
                </div>

                <div style="margin-top: 1.5rem; padding-top: 1.5rem; border-top: 1px solid var(--border-color);">
                    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem;">
                        <div>
                            <div style="font-size: 2rem; font-weight: 700; color: var(--primary-blue);">${totalStudents}</div>
                            <div style="color: var(--gray-medium);">Total Students</div>
                        </div>
                        <div>
                            <div style="font-size: 2rem; font-weight: 700; color: var(--gray-medium);">${draftCount}</div>
                            <div style="color: var(--gray-medium);">Draft</div>
                        </div>
                        <div>
                            <div style="font-size: 2rem; font-weight: 700; color: var(--warning-orange);">${pendingStudents.length}</div>
                            <div style="color: var(--gray-medium);">Pending LEA</div>
                        </div>
                        <div>
                            <div style="font-size: 2rem; font-weight: 700; color: var(--success-green);">${approvedCount}</div>
                            <div style="color: var(--gray-medium);">Approved</div>
                        </div>
                    </div>
                </div>
            </div>

            ${portal === 'lea' && pendingStudents.length > 0 ? `
                <div class="fund-warning-alert">
                    <h4>⏳ ${pendingStudents.length} student${pendingStudents.length > 1 ? 's' : ''} awaiting your fiscal information</h4>
                    <p>Please complete the LEA information for each student to proceed with CTC review.</p>
                </div>
            ` : ''}

            ${this.renderStudentTable(app, portal)}
        `;
    },

    renderApplicationCard(app, portal) {
        const totalStudents = app.students.length;
        const approvedCount = app.students.filter(s => s.status === 'APPROVED').length;
        const pendingLEACount = app.students.filter(s => s.status === 'PENDING_LEA').length;
        const submittedCount = app.students.filter(s => s.status === 'SUBMITTED').length;
        const draftCount = app.students.filter(s => s.status === 'DRAFT').length;

        let statusItems = '';
        if (portal === 'ihe') {
            statusItems = `
                <div class="status-item">
                    <span class="count">${totalStudents}</span>
                    <span class="label">Total</span>
                </div>
                <div class="status-item">
                    <span class="count">${draftCount}</span>
                    <span class="label">Draft</span>
                </div>
                <div class="status-item">
                    <span class="count">${approvedCount}</span>
                    <span class="label">Approved</span>
                </div>
            `;
        } else if (portal === 'lea') {
            statusItems = `
                <div class="status-item">
                    <span class="count">${totalStudents}</span>
                    <span class="label">Total</span>
                </div>
                <div class="status-item">
                    <span class="count">${pendingLEACount}</span>
                    <span class="label">Need LEA Info</span>
                </div>
                <div class="status-item">
                    <span class="count">${approvedCount}</span>
                    <span class="label">Approved</span>
                </div>
            `;
        } else {
            statusItems = `
                <div class="status-item">
                    <span class="count">${totalStudents}</span>
                    <span class="label">Total</span>
                </div>
                <div class="status-item">
                    <span class="count">${submittedCount}</span>
                    <span class="label">Awaiting Review</span>
                </div>
                <div class="status-item">
                    <span class="count">${approvedCount}</span>
                    <span class="label">Approved</span>
                </div>
            `;
        }

        return `
            <div class="card mb-2">
                <div class="flex-between mb-2">
                    <div>
                        <h3>${app.ihe.name} → ${app.lea.name}</h3>
                        <p class="text-muted">Created ${this.formatDate(app.createdAt)}</p>
                    </div>
                    <button class="btn btn-primary btn-sm" onclick="Applications.viewApplication(${app.id})">
                        ${portal === 'staff' ? 'View Details' : 'Manage Students'} (${totalStudents})
                    </button>
                </div>
                <div class="status-summary">
                    ${statusItems}
                </div>
            </div>
        `;
    },

    renderStudentTable(app, portal) {
        if (app.students.length === 0) {
            return `
                <div class="card">
                    <p class="text-center text-muted">No students added yet. Click "Add Student" to begin.</p>
                </div>
            `;
        }

        return `
            <div class="card">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Student Name</th>
                            <th>SEID</th>
                            <th>Credential Area</th>
                            <th>Status</th>
                            <th>Award</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${app.students.map(student => this.renderStudentRow(student, portal)).join('')}
                    </tbody>
                </table>
            </div>
        `;
    },

    renderStudentRow(student, portal) {
        const statusBadge = this.getStatusBadge(student.status);
        const actions = this.getStudentActions(student, portal);

        return `
            <tr>
                <td><strong>${student.firstName} ${student.lastName}</strong></td>
                <td>${student.seid}</td>
                <td>${student.credentialArea}</td>
                <td><span class="badge ${statusBadge.class}">${statusBadge.text}</span></td>
                <td>${student.awardAmount ? Dashboard.formatCurrency(student.awardAmount) : '-'}</td>
                <td>
                    ${actions}
                </td>
            </tr>
        `;
    },

    getStatusBadge(status) {
        const badges = {
            'DRAFT': { text: 'Draft', class: 'badge-draft' },
            'PENDING_LEA': { text: 'Pending LEA', class: 'badge-pending' },
            'PENDING_SUBMISSION': { text: 'Pending Submit', class: 'badge-pending' },
            'SUBMITTED': { text: 'Submitted', class: 'badge-submitted' },
            'UNDER_REVIEW': { text: 'Under Review', class: 'badge-review' },
            'APPROVED': { text: 'Approved', class: 'badge-approved' },
            'REJECTED': { text: 'Rejected', class: 'badge-rejected' }
        };
        return badges[status] || { text: status, class: '' };
    },

    getStudentActions(student, portal) {
        if (portal === 'ihe') {
            if (student.status === 'DRAFT') {
                return `
                    <button class="btn btn-primary btn-sm" onclick="Applications.editStudent(${student.id})">Edit</button>
                    <button class="btn btn-success btn-sm" onclick="Applications.submitToLEA(${student.id})">Submit to LEA</button>
                `;
            } else {
                return `<button class="btn btn-secondary btn-sm" onclick="Applications.viewStudent(${student.id})">View</button>`;
            }
        } else if (portal === 'lea') {
            if (student.status === 'PENDING_LEA') {
                return `<button class="btn btn-primary btn-sm" onclick="Applications.completeLEAInfo(${student.id})">Complete LEA Info</button>`;
            } else {
                return `<button class="btn btn-secondary btn-sm" onclick="Applications.viewStudent(${student.id})">View</button>`;
            }
        } else if (portal === 'staff') {
            if (student.status === 'SUBMITTED') {
                return `
                    <button class="btn btn-success btn-sm" onclick="Applications.approveStudent(${student.id})">Approve</button>
                    <button class="btn btn-danger btn-sm" onclick="Applications.rejectStudent(${student.id})">Reject</button>
                `;
            } else if (student.status === 'APPROVED' && !dataHelpers.getAwardByStudent(student.id)) {
                return `<button class="btn btn-primary btn-sm" onclick="Awards.generateGAA(${student.id})">Generate GAA</button>`;
            } else {
                return `<button class="btn btn-secondary btn-sm" onclick="Applications.viewStudent(${student.id})">View Details</button>`;
            }
        }
        return '';
    },

    // Modal: Add new student (IHE)
    showAddStudentModal(applicationId) {
        this.tempApplicationId = applicationId; // Store for save functions

        const modal = `
            <div class="modal-header">
                <h3>Add New Student</h3>
                <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <form id="add-student-form">
                    <div class="form-row">
                        <div class="form-group">
                            <label>First Name * <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Legal first name</span></label>
                            <input type="text" class="form-control" id="firstName" required onblur="Applications.attemptSEIDMatch()">
                        </div>
                        <div class="form-group">
                            <label>Last Name * <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Legal last name</span></label>
                            <input type="text" class="form-control" id="lastName" required onblur="Applications.attemptSEIDMatch()">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>Date of Birth * <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Format: MM/DD/YYYY</span></label>
                            <input type="date" class="form-control" id="dob" required onchange="Applications.attemptSEIDMatch()">
                        </div>
                    </div>

                    <!-- SEID Match Result Area -->
                    <div id="seid-match-result" style="display: none; margin: 1rem 0;"></div>

                    <div class="form-group">
                        <label>Credential Area * <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Primary credential sought</span></label>
                        <select class="form-control" id="credentialArea" required>
                            <option value="">Select credential area...</option>
                            <option value="Multiple Subject">Multiple Subject</option>
                            <option value="Single Subject - Mathematics">Single Subject - Mathematics</option>
                            <option value="Single Subject - English">Single Subject - English</option>
                            <option value="Single Subject - Science">Single Subject - Science</option>
                            <option value="Education Specialist">Education Specialist</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Expected Completion Date * <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ When will student teaching end?</span></label>
                        <input type="date" class="form-control" id="expectedCompletion" required>
                    </div>

                    <div style="background: #F0F7FF; padding: 1rem; border-radius: 4px; border-left: 4px solid var(--info-blue); font-size: 0.9rem;">
                        💡 <strong>Tip:</strong> After entering name and DOB, we'll automatically search for the student's SEID in the ECS database. This helps ensure accurate records.
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" onclick="Utils.closeModal()">Cancel</button>
                <button class="btn btn-primary" onclick="Applications.previewSubmission('draft')">Review & Save as Draft</button>
                <button class="btn btn-success" onclick="Applications.previewSubmission('submit')">Review & Submit to LEA</button>
            </div>
        `;
        Utils.showModal(modal);
    },

    attemptSEIDMatch() {
        const firstName = document.getElementById('firstName')?.value;
        const lastName = document.getElementById('lastName')?.value;
        const dob = document.getElementById('dob')?.value;
        const resultDiv = document.getElementById('seid-match-result');

        if (!firstName || !lastName || !dob || !resultDiv) return;

        // Show loading
        resultDiv.style.display = 'block';
        resultDiv.innerHTML = `
            <div style="padding: 1rem; background: #F8F9FA; border-radius: 4px; text-align: center;">
                🔄 Searching ECS database for matching student...
            </div>
        `;

        // Simulate API call with delay
        setTimeout(() => {
            const matched = Math.random() > 0.3; // 70% match rate for demo

            if (matched) {
                const mockSEID = this.mockSEIDMatch(firstName, lastName, dob);
                const matchedCredential = ['Multiple Subject', 'Single Subject - Mathematics', 'Single Subject - English'][Math.floor(Math.random() * 3)];

                resultDiv.innerHTML = `
                    <div style="padding: 1rem; background: #D4EDDA; border: 2px solid #28A745; border-radius: 4px;">
                        <div style="display: flex; align-items: start; gap: 0.75rem;">
                            <span style="font-size: 2rem;">🎯</span>
                            <div style="flex: 1;">
                                <div style="font-weight: 700; color: #155724; margin-bottom: 0.5rem;">✓ SEID Match Found!</div>
                                <div style="font-size: 0.9rem; color: #155724;">
                                    <strong>SEID:</strong> ${mockSEID}<br>
                                    <strong>Name:</strong> ${lastName.toUpperCase()}, ${firstName.toUpperCase()}<br>
                                    <strong>Status:</strong> Active Student Teacher<br>
                                    <strong>Credential Area:</strong> ${matchedCredential}
                                </div>
                                <div style="margin-top: 0.75rem; padding-top: 0.75rem; border-top: 1px solid #C3E6CB;">
                                    <small style="color: #155724;">✓ All details verified from ECS database. Credential area will be auto-filled.</small>
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                // Auto-fill credential area
                const credentialSelect = document.getElementById('credentialArea');
                if (credentialSelect) {
                    credentialSelect.value = matchedCredential;
                }
            } else {
                resultDiv.innerHTML = `
                    <div style="padding: 1rem; background: #FFF3CD; border: 2px solid #FFC107; border-radius: 4px;">
                        <div style="display: flex; align-items: start; gap: 0.75rem;">
                            <span style="font-size: 2rem;">⚠️</span>
                            <div style="flex: 1;">
                                <div style="font-weight: 700; color: #856404; margin-bottom: 0.5rem;">No SEID Match Found</div>
                                <div style="font-size: 0.9rem; color: #856404;">
                                    We couldn't find a matching student in the ECS database. This could mean:
                                    <ul style="margin: 0.5rem 0 0 1.5rem;">
                                        <li>Student hasn't been registered yet</li>
                                        <li>Name or DOB doesn't match ECS records exactly</li>
                                        <li>Student is not yet in the system</li>
                                    </ul>
                                </div>
                                <div style="margin-top: 0.75rem; padding-top: 0.75rem; border-top: 1px solid #FFEAA7;">
                                    <small style="color: #856404;">ℹ️ You can still continue. The LEA will verify student information later.</small>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
            }
        }, 800);
    },

    previewSubmission(action) {
        const formData = this.getFormData('add-student-form');

        if (!formData.firstName || !formData.lastName || !formData.dob || !formData.credentialArea || !formData.expectedCompletion) {
            Utils.showToast('Please fill in all required fields', 'error');
            return;
        }

        const modal = `
            <div class="modal-header">
                <h3>Review Before ${action === 'draft' ? 'Saving' : 'Submitting'}</h3>
                <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <div style="background: #F0F7FF; padding: 1.5rem; border-radius: 8px; margin-bottom: 1.5rem;">
                    <h4 style="margin-top: 0;">📋 Student Information Summary</h4>
                    <table style="width: 100%; font-size: 0.95rem;">
                        <tr><td style="padding: 0.5rem 0; font-weight: 600;">Name:</td><td>${formData.firstName} ${formData.lastName}</td></tr>
                        <tr><td style="padding: 0.5rem 0; font-weight: 600;">Date of Birth:</td><td>${formData.dob}</td></tr>
                        <tr><td style="padding: 0.5rem 0; font-weight: 600;">Credential Area:</td><td>${formData.credentialArea}</td></tr>
                        <tr><td style="padding: 0.5rem 0; font-weight: 600;">Expected Completion:</td><td>${formData.expectedCompletion}</td></tr>
                    </table>
                </div>

                ${action === 'submit' ? `
                    <div style="background: #FFF3CD; padding: 1rem; border-radius: 4px; border-left: 4px solid #FFC107; margin-bottom: 1rem;">
                        <h4 style="margin-top: 0;">⏭️ What happens next?</h4>
                        <ol style="margin: 0.5rem 0 0 1.5rem; line-height: 1.8;">
                            <li>LEA will receive email notification</li>
                            <li>You won't be able to edit this student's information</li>
                            <li>LEA will complete fiscal information (dates, contact info, signatory)</li>
                            <li>After LEA submits, CTC will review for approval</li>
                        </ol>
                        <div style="margin-top: 1rem; padding-top: 1rem; border-top: 1px solid #FFEAA7;">
                            <strong>⏰ Typical timeline:</strong> 1-2 weeks from LEA completion to CTC decision
                        </div>
                    </div>
                ` : `
                    <div style="background: #E7F3FF; padding: 1rem; border-radius: 4px; border-left: 4px solid var(--info-blue); margin-bottom: 1rem;">
                        <h4 style="margin-top: 0;">💾 Saving as Draft</h4>
                        <p style="margin: 0;">This student will be saved but not sent to the LEA yet. You can:</p>
                        <ul style="margin: 0.5rem 0 0 1.5rem; line-height: 1.8;">
                            <li>Edit this student's information anytime</li>
                            <li>Add more students to this application</li>
                            <li>Submit to LEA when ready</li>
                        </ul>
                    </div>
                `}

                <div style="background: #F8F9FA; padding: 1rem; border-radius: 4px; font-size: 0.9rem; color: var(--gray-medium);">
                    ℹ️ You can review this information before final submission. Make sure all details are correct.
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" onclick="Applications.showAddStudentModal(${this.tempApplicationId})">← Back to Edit</button>
                <button class="btn ${action === 'draft' ? 'btn-primary' : 'btn-success'}" onclick="Applications.${action === 'draft' ? 'saveNewStudent' : 'saveAndSubmitStudent'}()">
                    Confirm ${action === 'draft' ? 'Save' : 'Submit'} →
                </button>
            </div>
        `;
        Utils.showModal(modal);
    },

    saveNewStudent() {
        const formData = this.getFormData('add-student-form');
        const newStudent = {
            id: mockData.students.length + 1,
            applicationId: this.tempApplicationId || this.currentApplication.id,
            ...formData,
            seid: this.mockSEIDMatch(formData.firstName, formData.lastName, formData.dob),
            status: 'DRAFT',
            awardAmount: null,
            createdAt: new Date().toISOString(),
            lastModified: new Date().toISOString()
        };

        mockData.students.push(newStudent);

        // Update the current application's students array
        if (this.currentApplication) {
            this.currentApplication.students.push(newStudent);
        }

        Utils.closeModal();
        Utils.showToast(`${newStudent.firstName} ${newStudent.lastName} added as draft`, 'success');
        App.renderCurrentView();
    },

    saveAndSubmitStudent() {
        const formData = this.getFormData('add-student-form');
        const newStudent = {
            id: mockData.students.length + 1,
            applicationId: this.tempApplicationId || this.currentApplication.id,
            ...formData,
            seid: this.mockSEIDMatch(formData.firstName, formData.lastName, formData.dob),
            status: 'PENDING_LEA',
            awardAmount: null,
            iheSubmittedAt: new Date().toISOString(),
            iheSubmittedBy: mockData.currentUser.email,
            createdAt: new Date().toISOString(),
            lastModified: new Date().toISOString()
        };

        mockData.students.push(newStudent);

        // Update the current application's students array
        if (this.currentApplication) {
            this.currentApplication.students.push(newStudent);
        }

        Utils.closeModal();
        Utils.showToast(`${newStudent.firstName} ${newStudent.lastName} submitted to LEA for fiscal info`, 'success');
        App.renderCurrentView();
    },

    submitToLEA(studentId) {
        const student = dataHelpers.getStudent(studentId);
        student.status = 'PENDING_LEA';
        student.iheSubmittedAt = new Date().toISOString();
        student.iheSubmittedBy = mockData.currentUser.email;
        student.lastModified = new Date().toISOString();

        Utils.showToast(`${student.firstName} ${student.lastName} submitted to LEA`, 'success');
        App.renderCurrentView();
    },

    completeLEAInfo(studentId) {
        const student = dataHelpers.getStudent(studentId);

        const modal = `
            <div class="modal-header">
                <h3>Complete LEA Fiscal Information</h3>
                <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <div class="card mb-2">
                    <h4>Student Information (from IHE)</h4>
                    <p><strong>Name:</strong> ${student.firstName} ${student.lastName}</p>
                    <p><strong>SEID:</strong> ${student.seid}</p>
                    <p><strong>Credential Area:</strong> ${student.credentialArea}</p>
                    <p class="text-muted">This information is read-only.</p>
                </div>

                <form id="lea-info-form">
                    <h4 class="mb-2">LEA Fiscal Information</h4>
                    <div class="form-row">
                        <div class="form-group">
                            <label>Student Teaching Start Date *</label>
                            <input type="date" class="form-control" id="leaStartDate" required>
                        </div>
                        <div class="form-group">
                            <label>Student Teaching End Date *</label>
                            <input type="date" class="form-control" id="leaEndDate" required>
                        </div>
                    </div>
                    <div class="form-group">
                        <label>LEA Contact Email *</label>
                        <input type="email" class="form-control" id="leaContactEmail" value="${mockData.currentUser.email}" required>
                    </div>
                    <div class="form-group">
                        <label>LEA Contact Phone *</label>
                        <input type="tel" class="form-control" id="leaContactPhone" required>
                    </div>
                    <div class="form-group">
                        <label>Authorized Signatory *</label>
                        <input type="text" class="form-control" id="leaAuthorizedSignatory" placeholder="Name, Title" required>
                    </div>
                    <div class="form-group">
                        <label>
                            <input type="checkbox" id="leaConfirmedEligibility" required>
                            I confirm this student teacher worked 540+ hours and is eligible for the stipend
                        </label>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" onclick="Utils.closeModal()">Cancel</button>
                <button class="btn btn-success" onclick="Applications.submitLEAInfo(${studentId})">Submit to CTC</button>
            </div>
        `;
        Utils.showModal(modal);
    },

    submitLEAInfo(studentId) {
        const formData = this.getFormData('lea-info-form');
        const student = dataHelpers.getStudent(studentId);

        Object.assign(student, formData);
        student.status = 'SUBMITTED';
        student.leaSubmittedAt = new Date().toISOString();
        student.leaSubmittedBy = mockData.currentUser.email;
        student.lastModified = new Date().toISOString();

        // Calculate total days
        const start = new Date(formData.leaStartDate);
        const end = new Date(formData.leaEndDate);
        student.leaTotalDays = Math.floor((end - start) / (1000 * 60 * 60 * 24));

        Utils.closeModal();
        Utils.showToast(`LEA information submitted for ${student.firstName} ${student.lastName}`, 'success');
        App.renderCurrentView();
    },

    approveStudent(studentId) {
        const student = dataHelpers.getStudent(studentId);
        const grantCycle = dataHelpers.getGrantCycle(1);

        // Check fund availability
        const remainingAmount = grantCycle.appropriatedAmount -
            (grantCycle.reservedAmount + grantCycle.encumberedAmount + grantCycle.disbursedAmount);

        if (remainingAmount < 10000) {
            Utils.showModal(`
                <div class="modal-header">
                    <h3>🚨 Insufficient Funds</h3>
                    <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
                </div>
                <div class="modal-body">
                    <p>Cannot approve this student. Remaining funds: ${Dashboard.formatCurrency(remainingAmount)}</p>
                    <p>Award amount needed: $10,000</p>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" onclick="Utils.closeModal()">OK</button>
                </div>
            `);
            return;
        }

        student.status = 'APPROVED';
        student.awardAmount = 10000;
        student.reviewedAt = new Date().toISOString();
        student.reviewedBy = mockData.currentUser.name;
        student.reviewNotes = 'Approved - all eligibility criteria met';
        student.lastModified = new Date().toISOString();

        // Update grant cycle funds
        grantCycle.reservedAmount += 10000;

        Utils.showToast(`${student.firstName} ${student.lastName} approved for $10,000 award`, 'success');
        App.renderCurrentView();
    },

    rejectStudent(studentId) {
        const student = dataHelpers.getStudent(studentId);
        student.status = 'REJECTED';
        student.reviewedAt = new Date().toISOString();
        student.reviewedBy = mockData.currentUser.name;
        student.reviewNotes = 'Rejected - eligibility requirements not met';
        student.lastModified = new Date().toISOString();

        Utils.showToast(`${student.firstName} ${student.lastName} rejected`, 'warning');
        App.renderCurrentView();
    },

    editStudent(studentId) {
        Utils.showToast('Edit student functionality (would open form with current values)', 'info');
    },

    viewStudent(studentId) {
        Utils.showToast('View student details (would show full record)', 'info');
    },

    viewApplication(appId) {
        this.currentApplication = dataHelpers.getApplication(appId);
        App.renderCurrentView();
    },

    backToList() {
        this.currentApplication = null;
        App.renderCurrentView();
    },

    mockSEIDMatch(firstName, lastName, dob) {
        // Mock SEID matching - in real app would call ECS API
        return String(Math.floor(Math.random() * 90000000) + 10000000);
    },

    getFormData(formId) {
        const form = document.getElementById(formId);
        const formData = new FormData(form);
        const data = {};

        form.querySelectorAll('input, select, textarea').forEach(field => {
            if (field.type === 'checkbox') {
                data[field.id] = field.checked;
            } else {
                data[field.id] = field.value;
            }
        });

        return data;
    },

    formatDate(isoString) {
        return new Date(isoString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }
};
