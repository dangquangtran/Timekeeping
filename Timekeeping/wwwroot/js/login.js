// Login page JavaScript functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all login page functionality
    initFormSubmission();
    initValidationAnimations();
    initFocusAnimations();
});



// Form submission with loading state
function initFormSubmission() {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const loginBtn = document.getElementById('loginBtn');
            const btnText = document.getElementById('btnText');
            const btnLoading = document.getElementById('btnLoading');
            
            // Check if form is valid
            if (this.checkValidity()) {
                loginBtn.disabled = true;
                btnText.style.display = 'none';
                btnLoading.style.display = 'inline-block';
            }
        });
    }
}

// Add shake animation on validation error
function initValidationAnimations() {
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('invalid', function() {
            this.parentElement.parentElement.classList.add('error-shake');
            setTimeout(() => {
                this.parentElement.parentElement.classList.remove('error-shake');
            }, 500);
        });
    });
}



// Focus animation
function initFocusAnimations() {
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement.style.transform = 'scale(1.02)';
        });
        
        input.addEventListener('blur', function() {
            this.parentElement.style.transform = 'scale(1)';
        });
    });
}

// Utility function to show/hide loading state
function setLoadingState(isLoading) {
    const loginBtn = document.getElementById('loginBtn');
    const btnText = document.getElementById('btnText');
    const btnLoading = document.getElementById('btnLoading');
    
    if (loginBtn && btnText && btnLoading) {
        loginBtn.disabled = isLoading;
        btnText.style.display = isLoading ? 'none' : 'inline-block';
        btnLoading.style.display = isLoading ? 'inline-block' : 'none';
    }
}

// Function to reset form state (useful for AJAX submissions)
function resetFormState() {
    setLoadingState(false);
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.parentElement.parentElement.classList.remove('error-shake');
    });
} 