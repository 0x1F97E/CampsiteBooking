// Authentication helper functions for Blazor
window.authHelper = {
    // Login function that makes the request from the browser (not from server)
    // This ensures the browser receives and stores the authentication cookie
    login: async function (email, password, rememberMe) {
        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: email,
                    password: password,
                    rememberMe: rememberMe
                }),
                credentials: 'include' // Important: include cookies in the request
            });

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Login error:', error);
            return {
                success: false,
                error: 'An error occurred during login. Please try again.'
            };
        }
    },

    // Logout function
    logout: async function () {
        try {
            const response = await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Logout error:', error);
            return {
                success: false,
                error: 'An error occurred during logout.'
            };
        }
    },

    // Change password function
    changePassword: async function (currentPassword, newPassword) {
        try {
            const response = await fetch('/api/auth/change-password', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    currentPassword: currentPassword,
                    newPassword: newPassword
                }),
                credentials: 'include' // Important: include cookies for authentication
            });

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Change password error:', error);
            return {
                success: false,
                error: 'An error occurred while changing your password. Please try again.'
            };
        }
    },

    // Update profile function
    updateProfile: async function (firstName, lastName, email, phone, country, preferredCommunication) {
        try {
            const response = await fetch('/api/auth/update-profile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    firstName: firstName,
                    lastName: lastName,
                    email: email,
                    phone: phone,
                    country: country,
                    preferredCommunication: preferredCommunication
                }),
                credentials: 'include' // Important: include cookies for authentication
            });

            const result = await response.json();
            return result;
        } catch (error) {
            console.error('Update profile error:', error);
            return {
                success: false,
                error: 'An error occurred while updating your profile. Please try again.'
            };
        }
    }
};

