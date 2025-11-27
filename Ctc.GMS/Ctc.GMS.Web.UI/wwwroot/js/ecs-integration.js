/**
 * ECS Integration JavaScript
 * Handles cascading dropdowns, SEID lookup, and contact autocomplete for GMS
 */

const ECS = {
    baseUrl: '/api/ecs',

    /**
     * Fetch helper with error handling
     */
    async fetch(endpoint, options = {}) {
        try {
            const response = await fetch(`${this.baseUrl}${endpoint}`, {
                headers: {
                    'Content-Type': 'application/json',
                    ...options.headers
                },
                ...options
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.error || `HTTP ${response.status}`);
            }

            return await response.json();
        } catch (error) {
            console.error(`ECS API Error: ${endpoint}`, error);
            throw error;
        }
    },

    // Organization Hierarchy

    /**
     * Load counties into a select element
     */
    async loadCounties(selectElement, placeholder = '-- Select County --') {
        try {
            selectElement.disabled = true;
            selectElement.innerHTML = `<option value="">${placeholder}</option>`;

            const counties = await this.fetch('/counties');

            counties.forEach(county => {
                const option = document.createElement('option');
                option.value = county.code;
                option.textContent = county.name;
                selectElement.appendChild(option);
            });

            selectElement.disabled = false;
        } catch (error) {
            selectElement.innerHTML = '<option value="">Error loading counties</option>';
        }
    },

    /**
     * Load districts into a select element, optionally filtered by county
     */
    async loadDistricts(selectElement, countyCode = null, placeholder = '-- Select District --') {
        try {
            selectElement.disabled = true;
            selectElement.innerHTML = `<option value="">${placeholder}</option>`;

            const url = countyCode ? `/districts?countyCode=${countyCode}` : '/districts';
            const districts = await this.fetch(url);

            districts.forEach(district => {
                const option = document.createElement('option');
                option.value = district.cdsCode;
                option.textContent = district.name;
                option.dataset.countyCode = district.countyCode;
                selectElement.appendChild(option);
            });

            selectElement.disabled = false;
        } catch (error) {
            selectElement.innerHTML = '<option value="">Error loading districts</option>';
        }
    },

    /**
     * Load schools into a select element, optionally filtered by district
     */
    async loadSchools(selectElement, districtCdsCode = null, placeholder = '-- Select School --') {
        try {
            selectElement.disabled = true;
            selectElement.innerHTML = `<option value="">${placeholder}</option>`;

            const url = districtCdsCode ? `/schools?districtCdsCode=${districtCdsCode}` : '/schools';
            const schools = await this.fetch(url);

            schools.forEach(school => {
                const option = document.createElement('option');
                option.value = school.cdsCode;
                option.textContent = school.name;
                option.dataset.address = school.address || '';
                option.dataset.city = school.city || '';
                selectElement.appendChild(option);
            });

            selectElement.disabled = false;
        } catch (error) {
            selectElement.innerHTML = '<option value="">Error loading schools</option>';
        }
    },

    /**
     * Initialize cascading dropdowns for county > district > school hierarchy
     */
    initCascadingDropdowns(countySelect, districtSelect, schoolSelect, options = {}) {
        const { onChange } = options;

        // Load counties on init
        this.loadCounties(countySelect);

        // County change -> load districts
        countySelect.addEventListener('change', async () => {
            const countyCode = countySelect.value;

            // Reset dependent dropdowns
            districtSelect.innerHTML = '<option value="">-- Select District --</option>';
            districtSelect.disabled = true;
            schoolSelect.innerHTML = '<option value="">-- Select School --</option>';
            schoolSelect.disabled = true;

            if (countyCode) {
                await this.loadDistricts(districtSelect, countyCode);
            }

            if (onChange) onChange({ county: countyCode, district: null, school: null });
        });

        // District change -> load schools
        districtSelect.addEventListener('change', async () => {
            const districtCdsCode = districtSelect.value;

            // Reset school dropdown
            schoolSelect.innerHTML = '<option value="">-- Select School --</option>';
            schoolSelect.disabled = true;

            if (districtCdsCode) {
                await this.loadSchools(schoolSelect, districtCdsCode);
            }

            if (onChange) onChange({
                county: countySelect.value,
                district: districtCdsCode,
                school: null
            });
        });

        // School change
        schoolSelect.addEventListener('change', () => {
            if (onChange) onChange({
                county: countySelect.value,
                district: districtSelect.value,
                school: schoolSelect.value
            });
        });
    },

    // Contact Lookup

    /**
     * Load contacts for autocomplete selection
     */
    async loadContacts(districtCdsCode, role = null) {
        const url = role
            ? `/contacts?districtCdsCode=${districtCdsCode}&role=${role}`
            : `/contacts?districtCdsCode=${districtCdsCode}`;

        return await this.fetch(url);
    },

    /**
     * Initialize contact autocomplete on input elements
     */
    initContactAutocomplete(inputElement, districtCdsCode, options = {}) {
        const { onSelect, role } = options;

        let dropdown = null;

        const showDropdown = async () => {
            try {
                const contacts = await this.loadContacts(districtCdsCode, role);

                // Remove existing dropdown
                if (dropdown) dropdown.remove();

                if (contacts.length === 0) return;

                dropdown = document.createElement('div');
                dropdown.className = 'ecs-contact-dropdown';
                dropdown.style.cssText = `
                    position: absolute;
                    background: white;
                    border: 1px solid #ccc;
                    border-radius: 4px;
                    max-height: 200px;
                    overflow-y: auto;
                    width: ${inputElement.offsetWidth}px;
                    z-index: 1000;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.15);
                `;

                contacts.forEach(contact => {
                    const item = document.createElement('div');
                    item.className = 'ecs-contact-item';
                    item.style.cssText = 'padding: 8px 12px; cursor: pointer; border-bottom: 1px solid #eee;';
                    item.innerHTML = `
                        <strong>${contact.name}</strong><br>
                        <small>${contact.email} | ${contact.title || 'No title'}</small>
                    `;
                    item.addEventListener('click', () => {
                        inputElement.value = contact.name;
                        dropdown.remove();
                        dropdown = null;
                        if (onSelect) onSelect(contact);
                    });
                    item.addEventListener('mouseenter', () => item.style.backgroundColor = '#f5f5f5');
                    item.addEventListener('mouseleave', () => item.style.backgroundColor = '');
                    dropdown.appendChild(item);
                });

                // Add "Enter new contact" option
                const newOption = document.createElement('div');
                newOption.className = 'ecs-contact-new';
                newOption.style.cssText = 'padding: 8px 12px; cursor: pointer; color: #0066cc; font-style: italic;';
                newOption.textContent = '+ Enter new contact manually';
                newOption.addEventListener('click', () => {
                    dropdown.remove();
                    dropdown = null;
                    inputElement.focus();
                });
                dropdown.appendChild(newOption);

                // Position dropdown
                const rect = inputElement.getBoundingClientRect();
                dropdown.style.left = rect.left + 'px';
                dropdown.style.top = (rect.bottom + window.scrollY) + 'px';

                document.body.appendChild(dropdown);
            } catch (error) {
                console.error('Error loading contacts:', error);
            }
        };

        inputElement.addEventListener('focus', showDropdown);

        // Close dropdown on outside click
        document.addEventListener('click', (e) => {
            if (dropdown && !dropdown.contains(e.target) && e.target !== inputElement) {
                dropdown.remove();
                dropdown = null;
            }
        });
    },

    /**
     * Create a new contact (write-back to ECS)
     */
    async createContact(contactData) {
        return await this.fetch('/contacts', {
            method: 'POST',
            body: JSON.stringify(contactData)
        });
    },

    // SEID Lookup

    /**
     * Lookup SEID by student information
     */
    async lookupSEID(firstName, lastName, dateOfBirth, last4SSN) {
        return await this.fetch('/seid/lookup', {
            method: 'POST',
            body: JSON.stringify({
                firstName,
                lastName,
                dateOfBirth,
                last4SSN
            })
        });
    },

    /**
     * Validate if a SEID exists
     */
    async validateSEID(seid) {
        return await this.fetch(`/seid/validate/${seid}`);
    },

    /**
     * Initialize SEID lookup form
     */
    initSEIDLookup(formElement, options = {}) {
        const { onResult, onError, resultContainer } = options;

        formElement.addEventListener('submit', async (e) => {
            e.preventDefault();

            const firstName = formElement.querySelector('[name="firstName"]')?.value?.trim();
            const lastName = formElement.querySelector('[name="lastName"]')?.value?.trim();
            const dob = formElement.querySelector('[name="dateOfBirth"]')?.value;
            const last4 = formElement.querySelector('[name="last4SSN"]')?.value?.trim();

            if (!firstName || !lastName || !dob || !last4) {
                if (onError) onError('All fields are required');
                return;
            }

            if (!/^\d{4}$/.test(last4)) {
                if (onError) onError('Last 4 SSN must be exactly 4 digits');
                return;
            }

            try {
                const result = await this.lookupSEID(firstName, lastName, dob, last4);

                if (resultContainer) {
                    this.renderSEIDResult(resultContainer, result);
                }

                if (onResult) onResult(result);
            } catch (error) {
                if (onError) onError(error.message || 'SEID lookup failed');
            }
        });
    },

    /**
     * Render SEID lookup result into a container
     */
    renderSEIDResult(container, result) {
        let html = '';

        if (result.status === 'SingleMatch') {
            const record = result.records[0];
            html = `
                <div class="ecs-seid-result ecs-seid-success">
                    <strong>SEID Found:</strong> ${record.seid}<br>
                    <small>${record.firstName} ${record.lastName} | DOB: ${record.dateOfBirth}</small>
                </div>
            `;
        } else if (result.status === 'MultipleMatches') {
            html = `
                <div class="ecs-seid-result ecs-seid-warning">
                    <strong>Multiple Matches (${result.matchCount})</strong><br>
                    <small>${result.message}</small>
                    <ul style="margin: 8px 0 0 20px;">
                        ${result.records.map(r => `
                            <li>${r.seid} - ${r.firstName} ${r.lastName}</li>
                        `).join('')}
                    </ul>
                </div>
            `;
        } else {
            html = `
                <div class="ecs-seid-result ecs-seid-error">
                    <strong>No Match Found</strong><br>
                    <small>${result.message}</small>
                </div>
            `;
        }

        container.innerHTML = html;
    },

    // User Organization Association

    /**
     * Check if an email is associated with a known organization
     */
    async getOrganizationByEmail(email) {
        return await this.fetch(`/organization/by-email?email=${encodeURIComponent(email)}`);
    }
};

// Add CSS styles for ECS components
const ecsStyles = document.createElement('style');
ecsStyles.textContent = `
    .ecs-seid-result {
        padding: 12px;
        border-radius: 4px;
        margin: 10px 0;
    }
    .ecs-seid-success {
        background-color: #d4edda;
        border: 1px solid #c3e6cb;
        color: #155724;
    }
    .ecs-seid-warning {
        background-color: #fff3cd;
        border: 1px solid #ffeeba;
        color: #856404;
    }
    .ecs-seid-error {
        background-color: #f8d7da;
        border: 1px solid #f5c6cb;
        color: #721c24;
    }
    .ecs-contact-item:hover {
        background-color: #f5f5f5;
    }
`;
document.head.appendChild(ecsStyles);

// Export for use in pages
window.ECS = ECS;
