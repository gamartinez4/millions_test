import { useState, useEffect } from 'react'
import axios from 'axios'

/**
 * Hook para obtener todas las imágenes de propiedades
 */
export const useImages = () => {
	const [images, setImages] = useState([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState(null)

	const fetchImages = async () => {
		try {
			setLoading(true)
			setError(null)
			
			// Get token from localStorage
			const token = localStorage.getItem('token')
			
			if (!token) {
				throw new Error('No authentication token found')
			}
			
			// Llamar a la API SSR que a su vez llama al backend
			const { data } = await axios.get('/api/property-images/', {
				headers: {
					Authorization: `Bearer ${token}`
				}
			})
			
			setImages(data)
		} catch (error) {
			console.error('Failed to fetch property images:', error)
			setError(error.message || 'Failed to fetch property images')
		} finally {
			setLoading(false)
		}
	}

	useEffect(() => {
		fetchImages()
	}, [])

	return {
		images,
		loading,
		error,
		refetch: fetchImages
	}
}