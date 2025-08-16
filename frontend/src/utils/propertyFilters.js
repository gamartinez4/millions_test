import { useMemo, useRef } from 'react'

/**
 * Hook para filtrar propiedades del dashboard (excluyendo propiedades del usuario)
 */
export const useDisplayProperties = (properties, user) => {
	const loggedRef = useRef(false)

	return useMemo(() => {
		if (!properties || !Array.isArray(properties)) {
			return []
		}

		if (
			properties.length > 0 &&
			!loggedRef.current
		) {
			console.log("useDisplayProperties")
			console.log(properties)
			console.log(user)
			loggedRef.current = true
		}
		
		return properties.filter((property) => {
			return property.ownerId != user?.id
		})
	}, [properties])
}

/**
 * Hook para aplicar filtros a las propiedades
 */
export const useFilteredProperties = (properties, filters) => {
	const { nameFilter, addressFilter, minPriceFilter, maxPriceFilter } = filters

	return useMemo(() => {
		if (!properties || !Array.isArray(properties)) return []

		return properties.filter((property) => {
			const nameMatch = !nameFilter || 
				property.name.toLowerCase().includes(nameFilter.toLowerCase())
			
			const addressMatch = !addressFilter || 
				property.address.toLowerCase().includes(addressFilter.toLowerCase())
			
			const minPriceMatch = !minPriceFilter || 
				property.price >= parseFloat(minPriceFilter)
			
			const maxPriceMatch = !maxPriceFilter || 
				property.price <= parseFloat(maxPriceFilter)
			
			return nameMatch && addressMatch && minPriceMatch && maxPriceMatch
		})
	}, [properties, nameFilter, addressFilter, minPriceFilter, maxPriceFilter])
}



/**
 * Utilidad para obtener la primera imagen de una propiedad
 */
export const getFirstPropertyImage = (property) => {
	return property.images && property.images.length > 0 ? property.images[0] : null
}

/**
 * Utilidad para verificar si una propiedad es del usuario
 */
export const isPropertyOwned = (property, user) => {
	return user?.properties?.some(p => p.id === property.id) || false
}

/**
 * Combina las propiedades con sus imágenes correspondientes
 * @param {Array} properties - Lista de propiedades
 * @param {Array} images - Lista de todas las imágenes
 * @returns {Array} Propiedades con sus imágenes incluidas
 */
export const mergePropertiesWithImages = (properties, images) => {
	if (!properties || !Array.isArray(properties)) return []
	if (!images || !Array.isArray(images)) return properties

	return properties.map(property => {
		// Filtrar imágenes que pertenecen a esta propiedad
		const propertyImages = images.filter(image => 
			image.propertyId === property.id && image.enabled
		)

		return {
			...property,
			images: propertyImages
		}
	})
}
