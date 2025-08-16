import { useState } from 'react'
import { propertyService } from '@/services/propertyService'

/**
 * Custom hook for handling property purchase functionality
 * @returns {object} - Hook state and methods
 */
export const useBuyProperty = () => {
	const [loading, setLoading] = useState(false)
	const [error, setError] = useState(null)

	/**
	 * Buy a property by changing forSale to false and updating owner
	 * @param {number} propertyId - ID of the property to buy
	 * @param {number} newOwnerId - ID of the new owner (current user)
	 * @returns {Promise<boolean>} - Success status
	 */
	const buyProperty = async (propertyId, newOwnerId) => {
		try {
			setLoading(true)
			setError(null)

			if (!propertyId || !newOwnerId) {
				throw new Error('Property ID and new owner ID are required')
			}

			// Call the property service to buy the property
			await propertyService.buyProperty(propertyId, newOwnerId)

			setLoading(false)
			return true
		} catch (err) {
			console.error('Buy property error:', err.message)
			setError(err.message || 'Error al comprar la propiedad')
			setLoading(false)
			return false
		}
	}

	/**
	 * Clear error state
	 */
	const clearError = () => {
		setError(null)
	}

	return {
		buyProperty,
		loading,
		error,
		clearError
	}
}
