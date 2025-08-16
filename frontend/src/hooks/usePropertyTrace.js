import { useState } from 'react'
import axios from 'axios'

export const usePropertyTrace = () => {
	const [loading, setLoading] = useState(false)
	const [error, setError] = useState(null)

	const createPropertyTrace = async (traceData) => {
		setLoading(true)
		setError(null)

		try {
			const token = localStorage.getItem('token')
			
			if (!token) {
				throw new Error('No authentication token found')
			}

			const response = await axios.post('/api/property-traces/', traceData, {
				headers: {
					Authorization: `Bearer ${token}`,
					'Content-Type': 'application/json'
				}
			})

			return response.data
		} catch (error) {
			const errorMessage = error.response?.data?.message || error.message || 'Error creating property trace'
			setError(errorMessage)
			throw error
		} finally {
			setLoading(false)
		}
	}

	const createPurchaseTrace = async (propertyData, buyerName) => {
		const traceData = {
			dateSale: new Date().toISOString(),
			name: `Compra de propiedad por ${buyerName}`,
			value: propertyData.price,
			tax: propertyData.price * 0.1, // 10% tax rate - adjust as needed
			propertyId: propertyData.id
		}

		return await createPropertyTrace(traceData)
	}

	const getAllPropertyTraces = async () => {
		setLoading(true)
		setError(null)

		try {
			const token = localStorage.getItem('token')
			
			if (!token) {
				throw new Error('No authentication token found')
			}

			const response = await axios.get('/api/property-traces/', {
				headers: {
					Authorization: `Bearer ${token}`
				}
			})

			return response.data
		} catch (error) {
			const errorMessage = error.response?.data?.message || error.message || 'Error fetching property traces'
			setError(errorMessage)
			throw error
		} finally {
			setLoading(false)
		}
	}

	return {
		loading,
		error,
		createPropertyTrace,
		createPurchaseTrace,
		getAllPropertyTraces
	}
}
