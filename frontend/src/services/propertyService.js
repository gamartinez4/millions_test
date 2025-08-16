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

export const propertyService = {
	/**
	 * Get all properties
	 * @returns {Promise<Array>}
	 */
	async getAllProperties() {
		try {
			// Call Next.js SSR intermediary API instead of backend directly
			const response = await axios.get('/api/properties', {
				headers: {
					Authorization: `Bearer ${this.getToken()}`
				}
			})
			return response.data
		} catch (error) {
			console.error('PropertyService: Get all properties failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Get stored token for authentication
	 * @returns {string|null}
	 */
	getToken() {
		if (typeof window !== 'undefined') {
			return localStorage.getItem('token')
		}
		return null
	},

	/**
	 * Get property by ID
	 * @param {number} id 
	 * @returns {Promise<object>}
	 */
	async getPropertyById(id) {
		try {
			const response = await backendApi.get(`/api/properties/${id}`)
			return response.data
		} catch (error) {
			console.error('PropertyService: Get property by ID failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Create new property
	 * @param {object} propertyData 
	 * @returns {Promise<object>}
	 */
	async createProperty(propertyData) {
		try {
			const response = await backendApi.post('/api/properties', propertyData)
			return response.data
		} catch (error) {
			console.error('PropertyService: Create property failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Update property
	 * @param {number} id 
	 * @param {object} propertyData 
	 * @returns {Promise<void>}
	 */
	async updateProperty(id, propertyData) {
		try {
			const response = await backendApi.put(`/api/properties/${id}`, propertyData)
			return response.data
		} catch (error) {
			console.error('PropertyService: Update property failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Update property price
	 * @param {number} id 
	 * @param {object} priceData 
	 * @returns {Promise<void>}
	 */
	async updatePropertyPrice(id, priceData) {
		try {
			const response = await backendApi.put(`/api/properties/update-price/${id}`, priceData)
			return response.data
		} catch (error) {
			console.error('PropertyService: Update property price failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Update property owner
	 * @param {number} id 
	 * @param {object} ownerData 
	 * @returns {Promise<void>}
	 */
	async updatePropertyOwner(id, ownerData) {
		try {
			const response = await backendApi.put(`/api/properties/update-owner/${id}`, ownerData)
			return response.data
		} catch (error) {
			console.error('PropertyService: Update property owner failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Update property for sale status
	 * @param {number} id 
	 * @param {object} forSaleData 
	 * @returns {Promise<void>}
	 */
	async updatePropertyForSale(id, forSaleData) {
		try {
			const response = await backendApi.put(`/api/properties/update-for-sale/${id}`, forSaleData)
			return response.data
		} catch (error) {
			console.error('PropertyService: Update property for sale failed:', error.response?.data || error.message)
			throw error
		}
	},

	/**
	 * Delete property
	 * @param {number} id 
	 * @returns {Promise<void>}
	 */
	async deleteProperty(id) {
		try {
			await backendApi.delete(`/api/properties/${id}`)
		} catch (error) {
			console.error('PropertyService: Delete property failed:', error.response?.data || error.message)
			throw error
		}
	}
}
