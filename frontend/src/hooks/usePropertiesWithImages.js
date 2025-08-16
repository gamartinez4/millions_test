import { useMemo } from 'react'
import { mergePropertiesWithImages } from '@/utils/propertyFilters'

/**
 * Hook para combinar propiedades con sus imágenes correspondientes
 * @param {Array} properties - Lista de propiedades
 * @param {Array} images - Lista de todas las imágenes
 * @returns {Array} Propiedades con sus imágenes incluidas
 */
export const usePropertiesWithImages = (properties, images) => {
	const propertiesWithImages = useMemo(() => {
		const merged = mergePropertiesWithImages(properties, images)
		console.log('Propiedades con imágenes:', merged)
		return merged
	}, [properties, images])

	return propertiesWithImages
}
