// Awards Component - GAA Generation and Tracking

const Awards = {
    render(portal) {
        if (portal === 'staff') {
            return this.renderStaffView();
        }
        return '<div class="card"><p>Awards view for grantee portals coming soon...</p></div>';
    },

    renderStaffView() {
        const awards = mockData.awards;

        return `
            <div class="page-header">
                <h2>Awards & GAAs</h2>
                <p class="subtitle">Grant Agreement Authorizations and Payment Tracking</p>
            </div>

            <div class="card">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Student</th>
                            <th>IHE / LEA</th>
                            <th>Amount</th>
                            <th>Status</th>
                            <th>GAA Progress</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${awards.map(award => this.renderAwardRow(award)).join('')}
                    </tbody>
                </table>
            </div>
        `;
    },

    renderAwardRow(award) {
        const student = dataHelpers.getStudent(award.studentId);
        const application = dataHelpers.getApplication(student.applicationId);
        const statusBadge = this.getAwardStatusBadge(award.status);

        return `
            <tr>
                <td><strong>${student.firstName} ${student.lastName}</strong><br>
                    <small class="text-muted">SEID: ${student.seid}</small>
                </td>
                <td>${application.ihe.name}<br>→ ${application.lea.name}</td>
                <td>${Dashboard.formatCurrency(award.amount)}</td>
                <td><span class="badge ${statusBadge.class}">${statusBadge.text}</span></td>
                <td>${this.renderGAAProgress(award)}</td>
                <td>
                    ${this.getAwardActions(award)}
                </td>
            </tr>
        `;
    },

    getAwardStatusBadge(status) {
        const badges = {
            'PENDING_GAA': { text: 'Pending GAA', class: 'badge-draft' },
            'GAA_SENT': { text: 'GAA Sent', class: 'badge-pending' },
            'GAA_SIGNED': { text: 'GAA Signed', class: 'badge-approved' },
            'PAYMENT_INITIATED': { text: 'Payment Initiated', class: 'badge-submitted' },
            'PAYMENT_COMPLETED': { text: 'Paid', class: 'badge-completed' },
            'AWAITING_REPORTS': { text: 'Awaiting Reports', class: 'badge-review' },
            'REPORTS_SUBMITTED': { text: 'Reports Submitted', class: 'badge-approved' },
            'CLOSED': { text: 'Closed', class: 'badge-draft' }
        };
        return badges[status] || { text: status, class: '' };
    },

    renderGAAProgress(award) {
        if (!award.gaaSignatures) {
            return '<span class="text-muted">Not started</span>';
        }

        const signed = award.gaaSignatures.filter(s => s.status === 'SIGNED').length;
        const total = award.gaaSignatures.length;

        return `
            <div>
                <small>${signed} of ${total} signed</small>
                <div class="progress-bar mt-1">
                    <div class="progress-fill good" style="width: ${(signed/total)*100}%"></div>
                </div>
            </div>
        `;
    },

    getAwardActions(award) {
        if (award.status === 'GAA_SIGNED') {
            return `<button class="btn btn-primary btn-sm" onclick="Awards.initiatePayment(${award.id})">Initiate Payment</button>`;
        } else if (award.status === 'PAYMENT_INITIATED') {
            return `<button class="btn btn-success btn-sm" onclick="Awards.markPaymentComplete(${award.id})">Mark as Paid</button>`;
        }
        return `<button class="btn btn-secondary btn-sm" onclick="Awards.viewAwardDetails(${award.id})">View Details</button>`;
    },

    generateGAA(studentId) {
        const student = dataHelpers.getStudent(studentId);
        const application = dataHelpers.getApplication(student.applicationId);

        const modal = `
            <div class="modal-header">
                <h3>Generate Grant Agreement Authorization</h3>
                <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <div class="card mb-2">
                    <h4>Award Details</h4>
                    <p><strong>Student:</strong> ${student.firstName} ${student.lastName} (SEID: ${student.seid})</p>
                    <p><strong>IHE:</strong> ${application.ihe.name}</p>
                    <p><strong>LEA:</strong> ${application.lea.name}</p>
                    <p><strong>Amount:</strong> ${Dashboard.formatCurrency(student.awardAmount)}</p>
                </div>

                <div class="card">
                    <h4>DocuSign Routing</h4>
                    <p>GAA will be sent for signature in this order:</p>
                    <ol>
                        <li><strong>LEA Authorized Signatory:</strong> ${student.leaAuthorizedSignatory}</li>
                        <li><strong>CTC Grants Manager:</strong> Cara Mendoza</li>
                        <li><strong>CTC Fiscal Officer:</strong> Sara Saelee</li>
                    </ol>
                </div>

                <p class="mt-2"><strong>Confirm generation of GAA?</strong></p>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" onclick="Utils.closeModal()">Cancel</button>
                <button class="btn btn-success" onclick="Awards.confirmGenerateGAA(${studentId})">Generate & Send GAA</button>
            </div>
        `;
        Utils.showModal(modal);
    },

    confirmGenerateGAA(studentId) {
        const student = dataHelpers.getStudent(studentId);
        const now = new Date().toISOString();

        const newAward = {
            id: mockData.awards.length + 1,
            studentId: studentId,
            grantCycleId: 1,
            amount: student.awardAmount,
            status: 'GAA_SENT',

            gaaGeneratedAt: now,
            gaaSentAt: now,

            gaaSignatures: [
                {
                    party: "LEA",
                    name: student.leaAuthorizedSignatory,
                    status: "PENDING",
                    sentAt: now,
                    signedAt: null
                },
                {
                    party: "GRANTS_MANAGER",
                    name: "Cara Mendoza",
                    status: "PENDING",
                    sentAt: null,
                    signedAt: null
                },
                {
                    party: "FISCAL_OFFICER",
                    name: "Sara Saelee",
                    status: "PENDING",
                    sentAt: null,
                    signedAt: null
                }
            ],

            fiscalReferenceNumber: `FY26-STIPEND-${String(mockData.awards.length + 1).padStart(4, '0')}`,

            createdAt: now,
            lastModified: now
        };

        mockData.awards.push(newAward);

        // Update grant cycle funds: Reserved → Encumbered (when GAA sent)
        // Actually no - stays Reserved until fully signed
        // Reserved = approved but not signed
        // Encumbered = signed but not paid

        Utils.closeModal();
        Utils.showToast(`GAA generated and sent to ${student.leaAuthorizedSignatory}`, 'success');

        // Show GAA tracking modal
        this.showGAATracking(newAward.id);
    },

    showGAATracking(awardId) {
        const award = mockData.awards.find(a => a.id === awardId);
        const student = dataHelpers.getStudent(award.studentId);

        const modal = `
            <div class="modal-header">
                <h3>GAA Signature Tracking</h3>
                <button class="modal-close" onclick="Utils.closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <h4>${student.firstName} ${student.lastName} - ${Dashboard.formatCurrency(award.amount)}</h4>
                <p class="text-muted mb-2">Award #${award.fiscalReferenceNumber}</p>

                <div class="state-machine">
                    <h4>Signature Progress</h4>
                    ${award.gaaSignatures.map((sig, idx) => this.renderSignatureItem(sig, idx)).join('')}
                </div>

                <p class="mt-2 text-muted">
                    <small>Click "Simulate" buttons to test the signature workflow</small>
                </p>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" onclick="Utils.closeModal()">Close</button>
            </div>
        `;
        Utils.showModal(modal);
    },

    renderSignatureItem(sig, index) {
        const icon = sig.status === 'SIGNED' ? '✓' : sig.status === 'PENDING' ? '⏳' : '○';
        const statusClass = sig.status === 'SIGNED' ? 'completed' : sig.status === 'PENDING' ? 'current' : '';

        return `
            <div class="history-item ${statusClass}">
                <div class="flex-between">
                    <div>
                        <strong>${icon} ${index + 1}. ${sig.name}</strong> (${sig.party})
                        ${sig.status === 'SIGNED' ? `<br><small class="text-muted">Signed: ${new Date(sig.signedAt).toLocaleString()}</small>` : ''}
                        ${sig.status === 'PENDING' && sig.sentAt ? `<br><small class="text-muted">Sent: ${new Date(sig.sentAt).toLocaleString()}</small>` : ''}
                    </div>
                    ${sig.status === 'PENDING' && sig.sentAt ? `
                        <button class="btn btn-sm btn-success" onclick="Awards.simulateSignature(${index})">
                            Simulate Sign
                        </button>
                    ` : ''}
                </div>
            </div>
        `;
    },

    simulateSignature(sigIndex) {
        // Find the currently open award in modal (we'll use the last award for simplicity)
        const award = mockData.awards[mockData.awards.length - 1];
        const sig = award.gaaSignatures[sigIndex];

        sig.status = 'SIGNED';
        sig.signedAt = new Date().toISOString();

        // Send to next party if available
        const nextSig = award.gaaSignatures[sigIndex + 1];
        if (nextSig) {
            nextSig.sentAt = new Date().toISOString();
        }

        // Check if all signed
        const allSigned = award.gaaSignatures.every(s => s.status === 'SIGNED');
        if (allSigned) {
            award.status = 'GAA_SIGNED';
            award.gaaFullySignedAt = new Date().toISOString();

            // Move funds: Reserved → Encumbered
            const grantCycle = dataHelpers.getGrantCycle(1);
            grantCycle.reservedAmount -= award.amount;
            grantCycle.encumberedAmount += award.amount;

            Utils.showToast(`All signatures complete! Award moved to GAA_SIGNED status. Funds encumbered.`, 'success');
        } else {
            Utils.showToast(`${sig.name} signed. Sent to ${nextSig.name}.`, 'success');
        }

        // Refresh the modal
        this.showGAATracking(award.id);
    },

    initiatePayment(awardId) {
        const award = mockData.awards.find(a => a.id === awardId);
        award.status = 'PAYMENT_INITIATED';
        award.paymentInitiatedAt = new Date().toISOString();

        Utils.showToast('Payment initiated in FI$Cal', 'success');
        App.renderCurrentView();
    },

    markPaymentComplete(awardId) {
        const award = mockData.awards.find(a => a.id === awardId);
        award.status = 'PAYMENT_COMPLETED';
        award.paymentCompletedAt = new Date().toISOString();

        // Move funds: Encumbered → Disbursed
        const grantCycle = dataHelpers.getGrantCycle(1);
        grantCycle.encumberedAmount -= award.amount;
        grantCycle.disbursedAmount += award.amount;

        Utils.showToast('Payment completed and funds disbursed', 'success');
        App.renderCurrentView();
    },

    viewAwardDetails(awardId) {
        Utils.showToast('View award details (would show full GAA history)', 'info');
    }
};
