import { useState, useEffect } from 'react'
import axios from 'axios'

export const useProperties = () => {
	const [properties, setProperties] = useState([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState(null)

	const fetchProperties = async () => {
		try {
			setLoading(true)
			setError(null)
			const { data } = await axios.get('/api/properties')
			setProperties(data)
		} catch (error) {
			console.error('Failed to fetch properties', error)
			setError(error.message || 'Failed to fetch properties')
		} finally {
			setLoading(false)
		}
	}

	useEffect(() => {
		fetchProperties()
	}, [])


	return {
		properties,
		loading,
		error,

	}
}
