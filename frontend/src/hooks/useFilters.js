import { useState } from 'react'
import { useFilteredProperties } from '@/utils/propertyFilters'

/**
 * Hook para manejar los filtros del dashboard
 */
export const useFilters = (properties) => {
	const [filters, setFilters] = useState({
		nameFilter: '',
		addressFilter: '',
		minPriceFilter: '',
		maxPriceFilter: ''
	})

	// Aplicar filtros a las propiedades
	const filteredProperties = useFilteredProperties(properties, filters)

	// Handler para cambiar un filtro específico
	const handleFilterChange = (filterName, value) => {
		setFilters(prev => ({
			...prev,
			[filterName]: value
		}))
	}

	// Limpiar todos los filtros
	const clearFilters = () => {
		setFilters({
			nameFilter: '',
			addressFilter: '',
			minPriceFilter: '',
			maxPriceFilter: ''
		})
	}

	// Verificar si hay filtros activos
	const hasActiveFilters = Object.values(filters).some(filter => filter !== '')

	return {
		filters,
		filteredProperties,
		handleFilterChange,
		clearFilters,
		hasActiveFilters
	}
}
