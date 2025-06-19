// Login page JavaScript functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all login page functionality
    initFormSubmission();
    initValidationAnimations();
    initRememberMe();
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

// Remember me functionality
function initRememberMe() {
    const rememberMe = document.getElementById('rememberMe');
    const usernameInput = document.querySelector('input[name="UserName"]');
    
    if (rememberMe && usernameInput) {
        // Load saved username
        if (localStorage.getItem('rememberMe') === 'true') {
            rememberMe.checked = true;
            usernameInput.value = localStorage.getItem('savedUsername') || '';
        }
        
        // Save username when remember me is checked
        rememberMe.addEventListener('change', function() {
            if (this.checked) {
                localStorage.setItem('rememberMe', 'true');
                localStorage.setItem('savedUsername', usernameInput.value);
            } else {
                localStorage.removeItem('rememberMe');
                localStorage.removeItem('savedUsername');
            }
        });
        
        // Update saved username as user types
        usernameInput.addEventListener('input', function() {
            if (rememberMe.checked) {
                localStorage.setItem('savedUsername', this.value);
            }
        });
    }
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