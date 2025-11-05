// Magic Link Portal - Student Data Collection (No login required)

const MagicLinkPortal = {
    currentToken: null,

    openCollectionForm(token) {
        this.currentToken = token;
        const magicLink = dataHelpers.getMagicLinkByToken(token);

        if (!magicLink) {
            this.renderInvalidLink();
            return;
        }

        if (magicLink.status === 'COMPLETED') {
            this.renderAlreadyCompleted(magicLink);
            return;
        }

        const student = dataHelpers.getStudent(magicLink.studentId);
        const application = dataHelpers.getApplication(student.applicationId);

        this.renderCollectionForm(magicLink, student, application);
    },

    renderInvalidLink() {
        document.getElementById('app-content').innerHTML = `
            <div class="public-portal">
                <div class="public-header">
                    <h1>California Commission on Teacher Credentialing</h1>
                    <h2>Student Teacher Data Collection</h2>
                </div>

                <div class="card" style="text-align: center; padding: 3rem;">
                    <div style="font-size: 3rem; margin-bottom: 1rem;">⚠️</div>
                    <h2>Invalid or Expired Link</h2>
                    <p style="margin-top: 1rem; color: var(--gray-medium);">
                        This data collection link is not valid or has expired.
                        Please contact your LEA coordinator for assistance.
                    </p>
                </div>
            </div>
        `;
    },

    renderAlreadyCompleted(magicLink) {
        const completedDate = new Date(magicLink.completedAt).toLocaleString();

        document.getElementById('app-content').innerHTML = `
            <div class="public-portal">
                <div class="public-header">
                    <h1>California Commission on Teacher Credentialing</h1>
                    <h2>Student Teacher Data Collection</h2>
                </div>

                <div class="card" style="text-align: center; padding: 3rem;">
                    <div style="font-size: 3rem; margin-bottom: 1rem; color: var(--success-green);">✅</div>
                    <h2>Already Completed</h2>
                    <p style="margin-top: 1rem;">
                        You have already completed this data collection form.
                    </p>
                    <p style="margin-top: 0.5rem; color: var(--gray-medium); font-size: 0.9rem;">
                        Submitted on ${completedDate}
                    </p>
                    <p style="margin-top: 2rem;">
                        Thank you for your participation in the STIPEND grant program!
                    </p>
                </div>
            </div>
        `;
    },

    renderCollectionForm(magicLink, student, application) {
        const existingData = magicLink.data || {};

        document.getElementById('app-content').innerHTML = `
            <div class="public-portal">
                <div class="public-header">
                    <h1>California Commission on Teacher Credentialing</h1>
                    <h2>Student Teacher Data Collection</h2>
                </div>

                <div class="card">
                    <div style="background: #F0F7FF; padding: 1.5rem; border-radius: 8px; margin-bottom: 2rem;">
                        <h3 style="margin-top: 0;">Hello ${student.firstName} ${student.lastName},</h3>
                        <p style="margin-bottom: 0;">
                            Your LEA (${application.lea.name}) has requested additional information
                            to complete your STIPEND grant application. This information will be used for program
                            evaluation and reporting purposes. Some fields are optional.
                        </p>
                    </div>

                    <form id="magic-link-form">
                        <div class="form-section">
                            <h3>Demographic Information</h3>
                            <p style="color: var(--gray-medium); font-size: 0.9rem; margin-bottom: 1.5rem;">
                                This information is collected to help the CTC understand program reach and effectiveness.
                                All responses are voluntary and will be kept confidential.
                            </p>

                            <div class="form-group">
                                <label>Ethnicity (Voluntary) <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Select the category that best describes you</span></label>
                                <select id="ethnicity">
                                    <option value="">Prefer not to say</option>
                                    <option value="American Indian or Alaska Native" ${existingData.ethnicity === 'American Indian or Alaska Native' ? 'selected' : ''}>American Indian or Alaska Native</option>
                                    <option value="Asian" ${existingData.ethnicity === 'Asian' ? 'selected' : ''}>Asian</option>
                                    <option value="Black or African American" ${existingData.ethnicity === 'Black or African American' ? 'selected' : ''}>Black or African American</option>
                                    <option value="Hispanic or Latino" ${existingData.ethnicity === 'Hispanic or Latino' ? 'selected' : ''}>Hispanic or Latino</option>
                                    <option value="Native Hawaiian or Other Pacific Islander" ${existingData.ethnicity === 'Native Hawaiian or Other Pacific Islander' ? 'selected' : ''}>Native Hawaiian or Other Pacific Islander</option>
                                    <option value="White" ${existingData.ethnicity === 'White' ? 'selected' : ''}>White</option>
                                    <option value="Two or More Races" ${existingData.ethnicity === 'Two or More Races' ? 'selected' : ''}>Two or More Races</option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Gender Identity (Voluntary)</label>
                                <select id="genderIdentity">
                                    <option value="">Prefer not to say</option>
                                    <option value="Male" ${existingData.genderIdentity === 'Male' ? 'selected' : ''}>Male</option>
                                    <option value="Female" ${existingData.genderIdentity === 'Female' ? 'selected' : ''}>Female</option>
                                    <option value="Non-binary" ${existingData.genderIdentity === 'Non-binary' ? 'selected' : ''}>Non-binary</option>
                                    <option value="Transgender Male" ${existingData.genderIdentity === 'Transgender Male' ? 'selected' : ''}>Transgender Male</option>
                                    <option value="Transgender Female" ${existingData.genderIdentity === 'Transgender Female' ? 'selected' : ''}>Transgender Female</option>
                                    <option value="Other" ${existingData.genderIdentity === 'Other' ? 'selected' : ''}>Other</option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Sexual Orientation (Voluntary)</label>
                                <select id="sexualOrientation">
                                    <option value="">Prefer not to say</option>
                                    <option value="Heterosexual" ${existingData.sexualOrientation === 'Heterosexual' ? 'selected' : ''}>Heterosexual</option>
                                    <option value="Gay or Lesbian" ${existingData.sexualOrientation === 'Gay or Lesbian' ? 'selected' : ''}>Gay or Lesbian</option>
                                    <option value="Bisexual" ${existingData.sexualOrientation === 'Bisexual' ? 'selected' : ''}>Bisexual</option>
                                    <option value="Questioning" ${existingData.sexualOrientation === 'Questioning' ? 'selected' : ''}>Questioning</option>
                                    <option value="Other" ${existingData.sexualOrientation === 'Other' ? 'selected' : ''}>Other</option>
                                </select>
                            </div>
                        </div>

                        <div class="form-section">
                            <h3>Educational Background</h3>

                            <div class="form-group">
                                <label>
                                    <input type="checkbox" id="firstGenerationCollege" ${existingData.firstGenerationCollege ? 'checked' : ''}>
                                    I am a first-generation college student (neither parent completed a 4-year college degree)
                                </label>
                            </div>

                            <div class="form-group">
                                <label>Languages Spoken (Voluntary) <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Select all that apply</span></label>
                                <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 0.5rem; margin-top: 0.5rem;">
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_english" ${existingData.languagesSpoken?.includes('English') ? 'checked' : ''}>
                                        English
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_spanish" ${existingData.languagesSpoken?.includes('Spanish') ? 'checked' : ''}>
                                        Spanish
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_chinese" ${existingData.languagesSpoken?.includes('Chinese (Mandarin)') ? 'checked' : ''}>
                                        Chinese (Mandarin)
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_cantonese" ${existingData.languagesSpoken?.includes('Cantonese') ? 'checked' : ''}>
                                        Cantonese
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_vietnamese" ${existingData.languagesSpoken?.includes('Vietnamese') ? 'checked' : ''}>
                                        Vietnamese
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_tagalog" ${existingData.languagesSpoken?.includes('Tagalog') ? 'checked' : ''}>
                                        Tagalog
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_korean" ${existingData.languagesSpoken?.includes('Korean') ? 'checked' : ''}>
                                        Korean
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_armenian" ${existingData.languagesSpoken?.includes('Armenian') ? 'checked' : ''}>
                                        Armenian
                                    </label>
                                    <label style="font-weight: normal;">
                                        <input type="checkbox" id="lang_other" ${existingData.languagesSpoken?.includes('Other') ? 'checked' : ''}>
                                        Other
                                    </label>
                                </div>
                            </div>
                        </div>

                        <div class="form-section">
                            <h3>Credential Information</h3>

                            <div class="form-group">
                                <label>Credential Program <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Pre-filled from your application</span></label>
                                <input type="text" value="${student.credentialArea}" disabled>
                            </div>

                            <div class="form-group">
                                <label>Expected Completion Date <span style="color: var(--gray-medium); font-weight: normal; font-size: 0.85rem;">ⓘ Pre-filled from your application</span></label>
                                <input type="text" value="${new Date(student.expectedCompletion).toLocaleDateString()}" disabled>
                            </div>
                        </div>

                        <div style="background: #E7F3FF; padding: 1rem; border-radius: 4px; margin: 2rem 0;">
                            <h4 style="margin-top: 0;">Privacy Notice</h4>
                            <p style="font-size: 0.9rem; margin-bottom: 0;">
                                The information collected on this form is used for program evaluation, reporting to the Legislature,
                                and continuous improvement of CTC grant programs. Your responses will be kept confidential and used only
                                in aggregate form for reporting purposes. Individual responses will not be shared publicly.
                            </p>
                        </div>

                        <div style="background: #FFF3CD; padding: 1rem; border-radius: 4px; margin-bottom: 2rem;">
                            <label style="font-weight: normal; margin: 0;">
                                <input type="checkbox" id="confirmAccuracy" required>
                                <strong style="margin-left: 0.5rem;">I certify that the information provided is accurate to the best of my knowledge.</strong>
                            </label>
                        </div>

                        <div class="form-actions">
                            <button type="button" class="btn btn-secondary" onclick="MagicLinkPortal.saveDraft()">Save Draft</button>
                            <button type="button" class="btn btn-primary" onclick="MagicLinkPortal.submitForm()">Submit Information</button>
                        </div>
                    </form>
                </div>

                <div class="card" style="background: #F8F9FA;">
                    <h4>Questions?</h4>
                    <p>
                        If you have questions about this form or the STIPEND grant program, please contact your LEA coordinator:
                    </p>
                    <p style="margin-top: 0.5rem;">
                        <strong>${application.lea.name}</strong>
                    </p>
                </div>
            </div>
        `;
    },

    collectFormData() {
        const languages = [];
        ['english', 'spanish', 'chinese', 'cantonese', 'vietnamese', 'tagalog', 'korean', 'armenian', 'other'].forEach(lang => {
            if (document.getElementById(`lang_${lang}`)?.checked) {
                const labels = {
                    'english': 'English',
                    'spanish': 'Spanish',
                    'chinese': 'Chinese (Mandarin)',
                    'cantonese': 'Cantonese',
                    'vietnamese': 'Vietnamese',
                    'tagalog': 'Tagalog',
                    'korean': 'Korean',
                    'armenian': 'Armenian',
                    'other': 'Other'
                };
                languages.push(labels[lang]);
            }
        });

        return {
            ethnicity: document.getElementById('ethnicity').value,
            genderIdentity: document.getElementById('genderIdentity').value,
            sexualOrientation: document.getElementById('sexualOrientation').value,
            firstGenerationCollege: document.getElementById('firstGenerationCollege').checked,
            languagesSpoken: languages
        };
    },

    saveDraft() {
        const magicLink = dataHelpers.getMagicLinkByToken(this.currentToken);
        if (!magicLink) return;

        magicLink.data = this.collectFormData();

        Utils.showToast('Draft saved! You can return to complete this form later using the same link.', 'success');
    },

    submitForm() {
        const confirmAccuracy = document.getElementById('confirmAccuracy');
        if (!confirmAccuracy.checked) {
            Utils.showToast('Please certify that the information is accurate before submitting', 'error');
            return;
        }

        const magicLink = dataHelpers.getMagicLinkByToken(this.currentToken);
        if (!magicLink) return;

        magicLink.data = this.collectFormData();
        magicLink.status = 'COMPLETED';
        magicLink.completedAt = new Date().toISOString();

        document.getElementById('app-content').innerHTML = `
            <div class="public-portal">
                <div class="public-header">
                    <h1>California Commission on Teacher Credentialing</h1>
                    <h2>Student Teacher Data Collection</h2>
                </div>

                <div class="card" style="text-align: center; padding: 3rem;">
                    <div style="font-size: 3rem; margin-bottom: 1rem; color: var(--success-green);">✅</div>
                    <h2>Thank You!</h2>
                    <p style="margin-top: 1rem; font-size: 1.1rem;">
                        Your information has been submitted successfully.
                    </p>
                    <p style="margin-top: 1rem; color: var(--gray-medium);">
                        Your LEA will be notified that you have completed this form.
                        You will receive updates about your STIPEND grant application via email.
                    </p>

                    <div style="background: #F0F7FF; padding: 1.5rem; border-radius: 8px; margin: 2rem auto; max-width: 500px; text-align: left;">
                        <h4 style="margin-top: 0;">Next Steps:</h4>
                        <ol style="margin: 0; padding-left: 1.5rem;">
                            <li>Your LEA will complete the fiscal information</li>
                            <li>The application will be submitted to CTC for review</li>
                            <li>You will be notified of the approval decision</li>
                            <li>If approved, your LEA will process the $10,000 stipend payment</li>
                        </ol>
                    </div>

                    <p style="margin-top: 2rem; font-size: 0.9rem; color: var(--gray-medium);">
                        You may now close this window.
                    </p>
                </div>
            </div>
        `;
    }
};
