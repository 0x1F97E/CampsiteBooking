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
    }
};

