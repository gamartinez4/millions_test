import { useMemo } from 'react'

/**
 * Hook to filter properties that belong to the current user.
 * @param {Array} propertiesWithImages - The full list of properties, including their images.
 * @param {Object} user - The current user object.
 * @returns {Array} An array of properties owned by the user.
 */
export const useUserProperties = (propertiesWithImages, user) => {
	const userProperties = useMemo(() => {
		if (!propertiesWithImages || !user) return []
		return propertiesWithImages.filter(p => p.ownerId == user.id)
	}, [propertiesWithImages, user])

	return userProperties
}
