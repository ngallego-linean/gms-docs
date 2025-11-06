// GMS Site JavaScript

// Utility functions
const Utils = {
    showToast(message, type = 'info') {
        console.log(`[${type}] ${message}`);
        // In production, this would show a toast notification
    }
};

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    console.log('GMS Application loaded');
});
