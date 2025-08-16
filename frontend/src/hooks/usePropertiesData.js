import { useDisplayProperties } from '@/utils/propertyFilters'

/**
 * Hook para manejar datos de propiedades específicos del dashboard
 */
export const useDisplayPropertiesData = (allProperties, user) => {
	// Filtrar propiedades para mostrar (excluyendo las del usuario)
	const displayProperties = useDisplayProperties(allProperties, user)

	return {
		displayProperties
	}
}
