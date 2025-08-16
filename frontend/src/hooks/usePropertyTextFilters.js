import { useState, useMemo } from 'react'

/**
 * Hook to handle text-based filtering for a list of properties.
 * @param {Array} properties - The list of properties to filter.
 * @returns {Object} An object containing filter state, handlers, and the filtered list.
 */
export const usePropertyTextFilters = (properties) => {
	const [nameFilter, setNameFilter] = useState('')
	const [addressFilter, setAddressFilter] = useState('')

	const filteredProperties = useMemo(() => {
		if (!properties) return []
		return properties.filter((property) => {
			const nameMatch = !nameFilter || property.name.toLowerCase().includes(nameFilter.toLowerCase())
			const addressMatch = !addressFilter || property.address.toLowerCase().includes(addressFilter.toLowerCase())
			return nameMatch && addressMatch
		})
	}, [properties, nameFilter, addressFilter])

	const clearFilters = () => {
		setNameFilter('')
		setAddressFilter('')
	}

	return {
		nameFilter,
		addressFilter,
		setNameFilter,
		setAddressFilter,
		filteredProperties,
		clearFilters,
	}
}
