import axios from 'axios'

const API_BASE_URL = process.env.NODE_ENV === 'production' ? 'http://backend:5249' : 'http://localhost:5249'

// Create axios instance for backend communication
const backendApi = axios.create({
	baseURL: API_BASE_URL,
})

// Add request interceptor to include token in headers automatically
backendApi.interceptors.request.use(
	(config) => {
		if (typeof window !== 'undefined') {
			const token = localStorage.getItem('token')
			if (token) {
				config.headers.Authorization = `Bearer ${token}`
			}
		}
		return config
	},
	(error) => {
		return Promise.reject(error)
	}
)

// Add response interceptor to handle token expiration
backendApi.interceptors.response.use(
	(response) => response,
	(error) => {
		if (error.response?.status === 401) {
			// Token expired or invalid, clear local storage
			if (typeof window !== 'undefined') {
				localStorage.removeItem('token')
				localStorage.removeItem('user')
				delete axios.defaults.headers.common['Authorization']
				// Redirect to login if needed
				window.location.href = '/login'
			}
		}
		return Promise.reject(error)
	}
)

export const authService = {
	/**
	 * Authenticate user with username and password
	 * @param {string} username 
	 * @param {string} password 
	 * @returns {Promise<{token: string, owner: object}>}
	 */
	async login(username, password) {
		try {
			// Call Next.js SSR intermediary API instead of backend directly
			const response = await axios.post('/api/auth/login', {
				username,
				password,
			})
			
			return response.data
		} catch (error) {
			console.error('AuthService: Login failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Get current user information
	 * @returns {Promise<object>}
	 */
	async getCurrentUser() {
		try {
			// This would need a backend endpoint, for now return from localStorage
			if (typeof window !== 'undefined') {
				const user = localStorage.getItem('user')
				return user ? JSON.parse(user) : null
			}
			return null
		} catch (error) {
			console.error('AuthService: Get current user failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Logout user
	 * @returns {Promise<void>}
	 */
	async logout() {
		try {
			// Clear token and user data
			if (typeof window !== 'undefined') {
				localStorage.removeItem('token')
				localStorage.removeItem('user')
				delete axios.defaults.headers.common['Authorization']
			}
		} catch (error) {
			console.error('AuthService: Logout failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Check if user is authenticated
	 * @returns {boolean}
	 */
	isAuthenticated() {
		if (typeof window !== 'undefined') {
			const token = localStorage.getItem('token')
			const user = localStorage.getItem('user')
			return !!(token && user)
		}
		return false
	},

	/**
	 * Get stored token
	 * @returns {string|null}
	 */
	getToken() {
		if (typeof window !== 'undefined') {
			return localStorage.getItem('token')
		}
		return null
	}
}
