// Main initialization
document.addEventListener('DOMContentLoaded', () => {
    initializeSettings();
    setupFormValidations();
});

function initializeSettings() {
   
    // Initialize company ID
    const companyId = getUrlParameter('companyId') || window.settingsConfig.companyId;

    // Initialize permissions
    const permissions = window.settingsConfig.permissions;
    console.log('Permissions:', permissions);

    // Initialize combo boxes
    refreshComboboxes(companyId);
}

function refreshComboboxes(companyId) {
    // GL Account Comboboxes
    const glUrl = `${window.settingsConfig.urls.glLookup}?companyId=${companyId}`;
    [
        'cmb_exhgainloss', 'cmb_bankcharges', 'cmb_profitndloss',
        'cmb_returneraing', 'cmb_salegst', 'cmb_purchasegst',
        'cmb_saledefine', 'cmb_purchasedefine'
    ].forEach(id => bindComboBox(glUrl, id, 'glName', 'glId'));

    // Currency Comboboxes
    const currencyUrl = `${window.settingsConfig.urls.currencyLookup}?companyId=${companyId}`;
    ['cmb_basecurrency', 'cmb_localcurrency'].forEach(id =>
        bindComboBox(currencyUrl, id, 'currencyName', 'currencyId'));
}

function setupFormValidations() {
    // Decimal Form Validation
    const decimalForm = document.getElementById('decimalForm');
    if (decimalForm) {
        setupNumberValidation(decimalForm);
    }

    // Finance Form Validation
    const financeForm = document.getElementById('financeForm');
    if (financeForm) {
        setupSelectValidation(financeForm);
    }
}

function setupNumberValidation(form) {
    // Real-time validation
    form.querySelectorAll('input[type="number"]').forEach(input => {
        input.addEventListener('input', validateNumberInput);
    });

    // Submission validation
    form.addEventListener('submit', function (e) {
        let isValid = true;

        form.querySelectorAll('input[type="number"]').forEach(input => {
            if (!validateNumberInput(input)) isValid = false;
        });

        if (!isValid) {
            e.preventDefault();
            alert('Please fix the invalid fields before saving');
        }
    });
}

function validateNumberInput(input) {
    const min = parseInt(input.min);
    const max = parseInt(input.max);
    const value = parseInt(input.value);
    const isValid = !isNaN(value) && value >= min && value <= max;

    input.classList.toggle('is-invalid', !isValid);
    return isValid;
}

function setupSelectValidation(form) {
    form.addEventListener('submit', function (e) {
        let isValid = true;

        form.querySelectorAll('select[required]').forEach(select => {
            const isValidSelect = select.value !== '';
            select.classList.toggle('is-invalid', !isValidSelect);
            if (!isValidSelect) isValid = false;
        });

        if (!isValid) {
            e.preventDefault();
            alert('Please complete all required fields before saving');
        }
    });
}

// Utility function
function getUrlParameter(name) {
    name = name.replace(/[\[\]]/g, '\\$&');
    const regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)');
    const results = regex.exec(window.location.href);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}