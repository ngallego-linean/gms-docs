// Public Portal Component - Grant Listings and Information

const PublicPortal = {
    currentGrantId: null,

    render() {
        if (this.currentGrantId) {
            return this.renderGrantDetail(this.currentGrantId);
        }
        return this.renderGrantListings();
    },

    renderGrantListings() {
        return `
            <div class="public-portal">
                <div class="public-header">
                    <h1>California Commission on Teacher Credentialing</h1>
                    <h2>Grant Management System</h2>
                    <p class="subtitle">Supporting California's educators through grant programs</p>
                </div>

                <div class="card">
                    <h2>Available Grant Programs</h2>
                    <p style="margin-bottom: 2rem;">
                        The CTC administers various grant programs to support teacher preparation and retention in California.
                        Browse available programs below.
                    </p>

                    ${this.renderGrantCard(mockData.grantCycles[0], 'open')}

                    ${this.renderGrantCard({
                        id: 'ctrg',
                        name: "California Teacher Residency Grant",
                        programType: "CTRG",
                        appropriatedAmount: 50000000,
                        awardAmount: 20000,
                        description: "Supports high-quality teacher residency programs that prepare teacher candidates to serve in high-need subject areas and schools.",
                        applicationOpen: false,
                        nextCycle: "March 2026"
                    }, 'closed')}

                    ${this.renderGrantCard({
                        id: 'tsgp',
                        name: "Teacher Shortage Grant Program",
                        programType: "TSGP",
                        appropriatedAmount: 30000000,
                        awardAmount: 15000,
                        description: "Provides financial incentives for teachers to serve in shortage areas and high-need schools.",
                        applicationOpen: false,
                        nextCycle: "January 2026"
                    }, 'closed')}
                </div>

                <div class="card">
                    <h3>About the Grant Management System</h3>
                    <p>
                        The GMS streamlines the application, review, and reporting process for CTC grant programs.
                        Institutions of Higher Education (IHEs) and Local Educational Agencies (LEAs) can collaborate
                        to submit applications, track award status, and submit required reports.
                    </p>
                    <ul style="margin-top: 1rem;">
                        <li>Simplified online application process</li>
                        <li>Real-time application status tracking</li>
                        <li>Electronic Grant Agreement Authorizations (GAAs)</li>
                        <li>Streamlined reporting workflows</li>
                        <li>Transparent fund availability tracking</li>
                    </ul>
                </div>

                <div class="card">
                    <h3>Contact Information</h3>
                    <p>
                        For questions about grant programs or the GMS system, please contact:
                    </p>
                    <p style="margin-top: 1rem;">
                        <strong>Grants Team</strong><br>
                        California Commission on Teacher Credentialing<br>
                        Email: grants@ctc.ca.gov<br>
                        Phone: (916) 555-0100
                    </p>
                </div>
            </div>
        `;
    },

    renderGrantCard(grant, status) {
        const isOpen = status === 'open';
        const metrics = isOpen ? dataHelpers.calculateGrantCycleMetrics(grant.id) : null;

        return `
            <div class="grant-card ${isOpen ? 'grant-open' : 'grant-closed'}">
                <div class="grant-card-header">
                    <div>
                        <h3>${grant.name}</h3>
                        <span class="badge ${isOpen ? 'badge-approved' : 'badge-pending'}">
                            ${isOpen ? '🟢 Applications Open' : '🔴 Applications Closed'}
                        </span>
                    </div>
                </div>

                <div class="grant-card-body">
                    <div class="grant-metrics">
                        <div class="grant-metric">
                            <div class="grant-metric-label">Award Amount</div>
                            <div class="grant-metric-value">$${(grant.awardAmount || 10000).toLocaleString()}</div>
                            <div class="grant-metric-subtext">per participant</div>
                        </div>
                        ${isOpen && metrics ? `
                            <div class="grant-metric">
                                <div class="grant-metric-label">Funding Available</div>
                                <div class="grant-metric-value">$${(metrics.remainingAmount / 1000000).toFixed(1)}M</div>
                                <div class="grant-metric-subtext">${metrics.remainingPercent.toFixed(1)}% of $${(grant.appropriatedAmount / 1000000).toFixed(1)}M</div>
                            </div>
                            <div class="grant-metric">
                                <div class="grant-metric-label">Participants</div>
                                <div class="grant-metric-value">${metrics.totalStudents}</div>
                                <div class="grant-metric-subtext">student teachers</div>
                            </div>
                        ` : `
                            <div class="grant-metric">
                                <div class="grant-metric-label">Total Funding</div>
                                <div class="grant-metric-value">$${(grant.appropriatedAmount / 1000000).toFixed(1)}M</div>
                                <div class="grant-metric-subtext">appropriated</div>
                            </div>
                            <div class="grant-metric">
                                <div class="grant-metric-label">Next Cycle</div>
                                <div class="grant-metric-value">${grant.nextCycle || 'TBD'}</div>
                                <div class="grant-metric-subtext">applications open</div>
                            </div>
                        `}
                    </div>

                    ${grant.description ? `
                        <p class="grant-description">${grant.description}</p>
                    ` : ''}

                    <div class="grant-actions">
                        <button class="btn btn-secondary" onclick="PublicPortal.viewGrantDetail('${grant.id}')">
                            View Details
                        </button>
                        ${isOpen ? `
                            <button class="btn btn-primary" onclick="PublicPortal.applyNow('${grant.id}')">
                                Apply Now (IHE Login)
                            </button>
                        ` : ''}
                    </div>
                </div>
            </div>
        `;
    },

    renderGrantDetail(grantId) {
        let grant;
        if (typeof grantId === 'number') {
            grant = dataHelpers.getGrantCycle(grantId);
        } else {
            // Mock grant for closed programs
            return this.renderClosedGrantDetail(grantId);
        }

        if (!grant) return '<div class="card"><p>Grant not found.</p></div>';

        const metrics = dataHelpers.calculateGrantCycleMetrics(grant.id);

        return `
            <div class="public-portal">
                <div class="page-header flex-between">
                    <div>
                        <button class="btn btn-secondary" onclick="PublicPortal.backToListings()">← Back to All Grants</button>
                        <h1 style="margin-top: 1rem;">${grant.name}</h1>
                        <span class="badge badge-approved">🟢 Applications Open</span>
                    </div>
                </div>

                <div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));">
                    <div class="metric-card">
                        <div class="metric-label">Award Amount</div>
                        <div class="metric-value">$10,000</div>
                        <div class="metric-subtext">per student teacher</div>
                    </div>
                    <div class="metric-card ${metrics.remainingPercent < 25 ? 'status-warning' : ''}">
                        <div class="metric-label">Funding Available</div>
                        <div class="metric-value">$${(metrics.remainingAmount / 1000000).toFixed(2)}M</div>
                        <div class="metric-subtext">${metrics.remainingPercent.toFixed(1)}% remaining</div>
                    </div>
                    <div class="metric-card">
                        <div class="metric-label">Current Participants</div>
                        <div class="metric-value">${metrics.totalStudents}</div>
                        <div class="metric-subtext">student teachers</div>
                    </div>
                    <div class="metric-card">
                        <div class="metric-label">Application Deadline</div>
                        <div class="metric-value">Rolling</div>
                        <div class="metric-subtext">until funds exhausted</div>
                    </div>
                </div>

                <div class="card">
                    <h2>Program Overview</h2>
                    <p>
                        The Teacher Recruitment Incentive Grant (STIPEND) provides $10,000 stipends to student teachers
                        who complete 540 or more hours of student teaching at a qualifying Local Educational Agency (LEA).
                        The program aims to support teacher candidates during their student teaching experience and encourage
                        them to continue in the teaching profession.
                    </p>
                </div>

                <div class="card">
                    <h2>Eligibility Requirements</h2>
                    <h3>Student Teacher Eligibility</h3>
                    <ul>
                        <li>Enrolled in a credential program at a qualifying Institution of Higher Education (IHE)</li>
                        <li>Completing student teaching placement at a qualifying LEA</li>
                        <li>Must complete minimum of 540 hours of student teaching</li>
                        <li>Must not have received the stipend for a previous student teaching assignment</li>
                    </ul>

                    <h3>IHE Eligibility</h3>
                    <ul>
                        <li>Commission-approved teacher preparation program</li>
                        <li>In good standing with the CTC</li>
                    </ul>

                    <h3>LEA Eligibility</h3>
                    <ul>
                        <li>California public school district or charter school</li>
                        <li>Able to provide required fiscal documentation</li>
                        <li>Agrees to pay stipend directly to student teacher</li>
                    </ul>
                </div>

                <div class="card">
                    <h2>Application Process</h2>
                    <div class="process-steps">
                        <div class="process-step">
                            <div class="process-step-number">1</div>
                            <div class="process-step-content">
                                <h3>IHE Initiates Application</h3>
                                <p>The Institution of Higher Education creates an application and provides student teacher information including demographics and placement details.</p>
                            </div>
                        </div>

                        <div class="process-step">
                            <div class="process-step-number">2</div>
                            <div class="process-step-content">
                                <h3>LEA Completes Fiscal Information</h3>
                                <p>The LEA receives notification and adds fiscal details, confirms eligibility, and identifies the authorized signatory for the Grant Agreement Authorization (GAA).</p>
                            </div>
                        </div>

                        <div class="process-step">
                            <div class="process-step-number">3</div>
                            <div class="process-step-content">
                                <h3>Student Provides Demographics</h3>
                                <p>The student teacher receives a secure link to provide demographic information and grant-specific data directly to the system.</p>
                            </div>
                        </div>

                        <div class="process-step">
                            <div class="process-step-number">4</div>
                            <div class="process-step-content">
                                <h3>CTC Reviews Application</h3>
                                <p>CTC staff review the application for completeness and eligibility. Funding is checked and reserved for approved applications.</p>
                            </div>
                        </div>

                        <div class="process-step">
                            <div class="process-step-number">5</div>
                            <div class="process-step-content">
                                <h3>GAA Execution</h3>
                                <p>A Grant Agreement Authorization is generated and routed for three-party signature (LEA, Grants Manager, Fiscal Officer).</p>
                            </div>
                        </div>

                        <div class="process-step">
                            <div class="process-step-number">6</div>
                            <div class="process-step-content">
                                <h3>Payment Processing</h3>
                                <p>Once the GAA is fully executed, payment is processed through FI$Cal. The LEA receives funds and pays the student teacher directly.</p>
                            </div>
                        </div>

                        <div class="process-step">
                            <div class="process-step-number">7</div>
                            <div class="process-step-content">
                                <h3>Reporting</h3>
                                <p>After program completion, both IHE and LEA submit reports on completion status, employment outcomes, and payment confirmation.</p>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card">
                    <h2>Required Documents & Information</h2>
                    <div class="info-grid">
                        <div>
                            <h3>From IHE</h3>
                            <ul>
                                <li>Student teacher name, DOB, last 4 of SSN/ITIN</li>
                                <li>Credential area and program information</li>
                                <li>County assignment (CDS code)</li>
                                <li>LEA placement information</li>
                                <li>IHE contact person information</li>
                            </ul>
                        </div>
                        <div>
                            <h3>From LEA</h3>
                            <ul>
                                <li>Fiscal contact information</li>
                                <li>Authorized signatory for GAA</li>
                                <li>Superintendent information</li>
                                <li>Student teaching start and end dates</li>
                                <li>Payment schedule preference</li>
                            </ul>
                        </div>
                        <div>
                            <h3>From Student Teacher</h3>
                            <ul>
                                <li>Demographic information (voluntary)</li>
                                <li>Educational background</li>
                                <li>Language proficiencies</li>
                                <li>Grant-specific data elements</li>
                            </ul>
                        </div>
                    </div>
                </div>

                <div class="card">
                    <h2>Timeline & Deadlines</h2>
                    <ul>
                        <li><strong>Applications:</strong> Rolling basis throughout the fiscal year (July 1 - June 30)</li>
                        <li><strong>Review:</strong> Applications reviewed within 2 weeks of submission</li>
                        <li><strong>GAA Execution:</strong> Approximately 2-3 weeks after approval</li>
                        <li><strong>Payment Processing:</strong> 4-6 weeks after GAA fully executed</li>
                        <li><strong>IHE Reports Due:</strong> August 1 following program completion</li>
                        <li><strong>LEA Reports Due:</strong> August 15 following program completion</li>
                    </ul>
                </div>

                <div class="card">
                    <h2>Resources</h2>
                    <ul>
                        <li><a href="#" onclick="Utils.showToast('RFA download coming soon', 'info'); return false;">Download Request for Applications (RFA)</a></li>
                        <li><a href="#" onclick="Utils.showToast('FAQ download coming soon', 'info'); return false;">Frequently Asked Questions</a></li>
                        <li><a href="#" onclick="Utils.showToast('User guide download coming soon', 'info'); return false;">GMS User Guide for IHEs</a></li>
                        <li><a href="#" onclick="Utils.showToast('User guide download coming soon', 'info'); return false;">GMS User Guide for LEAs</a></li>
                        <li><a href="#" onclick="Utils.showToast('Video tutorial coming soon', 'info'); return false;">Application Process Video Tutorial</a></li>
                    </ul>
                </div>

                <div class="card">
                    <h2>Contact Information</h2>
                    <div class="info-grid">
                        <div>
                            <h3>Grants Team</h3>
                            <p>For questions about eligibility, applications, and awards:</p>
                            <p>
                                <strong>Cara Mendoza</strong>, Grants Manager<br>
                                Email: cara.mendoza@ctc.ca.gov<br>
                                Phone: (916) 555-0101
                            </p>
                        </div>
                        <div>
                            <h3>Fiscal Team</h3>
                            <p>For questions about payments and GAAs:</p>
                            <p>
                                <strong>Sara Saelee</strong>, Fiscal Officer<br>
                                Email: sara.saelee@ctc.ca.gov<br>
                                Phone: (916) 555-0102
                            </p>
                        </div>
                        <div>
                            <h3>Technical Support</h3>
                            <p>For questions about the GMS system:</p>
                            <p>
                                Email: gms.support@ctc.ca.gov<br>
                                Phone: (916) 555-0100
                            </p>
                        </div>
                    </div>
                </div>

                <div class="card" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-align: center;">
                    <h2 style="color: white; margin-bottom: 1rem;">Ready to Apply?</h2>
                    <p style="margin-bottom: 2rem;">
                        ${metrics.remainingPercent < 10 ?
                            '⚠️ Funding is limited! Apply soon to secure your stipend.' :
                            'Applications are now being accepted on a rolling basis.'
                        }
                    </p>
                    <button class="btn" style="background: white; color: #667eea; font-weight: bold;" onclick="PublicPortal.applyNow(${grant.id})">
                        Apply Now - IHE Login Required
                    </button>
                </div>
            </div>
        `;
    },

    renderClosedGrantDetail(grantId) {
        const grants = {
            'ctrg': {
                name: "California Teacher Residency Grant",
                description: "Supports high-quality teacher residency programs that prepare teacher candidates to serve in high-need subject areas and schools.",
                nextCycle: "March 2026"
            },
            'tsgp': {
                name: "Teacher Shortage Grant Program",
                description: "Provides financial incentives for teachers to serve in shortage areas and high-need schools.",
                nextCycle: "January 2026"
            }
        };

        const grant = grants[grantId];
        if (!grant) return '<div class="card"><p>Grant not found.</p></div>';

        return `
            <div class="public-portal">
                <div class="page-header">
                    <button class="btn btn-secondary" onclick="PublicPortal.backToListings()">← Back to All Grants</button>
                    <h1 style="margin-top: 1rem;">${grant.name}</h1>
                    <span class="badge badge-pending">🔴 Applications Closed</span>
                </div>

                <div class="card">
                    <h2>Program Overview</h2>
                    <p>${grant.description}</p>
                    <p style="margin-top: 1rem;">
                        <strong>Next Application Cycle:</strong> ${grant.nextCycle}
                    </p>
                    <p>
                        Applications for this grant program are currently closed. Please check back in ${grant.nextCycle}
                        when the next application cycle opens.
                    </p>
                </div>

                <div class="card">
                    <h2>Stay Informed</h2>
                    <p>
                        Sign up for email notifications to be alerted when applications open:
                    </p>
                    <form style="margin-top: 1rem;" onsubmit="event.preventDefault(); Utils.showToast('Email subscription coming soon', 'info');">
                        <input type="email" placeholder="Your email address" style="width: 300px; margin-right: 1rem;">
                        <button type="submit" class="btn btn-primary">Subscribe</button>
                    </form>
                </div>
            </div>
        `;
    },

    viewGrantDetail(grantId) {
        this.currentGrantId = grantId;
        App.renderCurrentView();
    },

    backToListings() {
        this.currentGrantId = null;
        App.renderCurrentView();
    },

    applyNow(grantId) {
        Utils.showModal(
            'IHE Login Required',
            `
                <p>To apply for this grant, you must log in with your IHE (Institution of Higher Education) credentials.</p>
                <p style="margin-top: 1rem;">In this mockup, click "IHE Portal" at the top to simulate logging in as an IHE coordinator.</p>
            `,
            [
                { text: 'Switch to IHE Portal', class: 'btn-primary', action: () => App.switchPortal('ihe') },
                { text: 'Cancel', class: 'btn-secondary', action: () => {} }
            ]
        );
    }
};
