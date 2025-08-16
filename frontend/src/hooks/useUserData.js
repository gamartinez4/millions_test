import { useState, useEffect } from 'react'

/**
 * Hook para manejar los datos del usuario
 */
export const useUserData = () => {
	const [user, setUser] = useState(null)
	const [loading, setLoading] = useState(true)

	useEffect(() => {
		const loadUserData = () => {
			try {
				console.log('Loading user data')
				const storedUser = localStorage.getItem('user')
				if (storedUser) {
					setUser(JSON.parse(storedUser))
				}
			} catch (error) {
				console.error('Error loading user data:', error)
			} finally {
				setLoading(false)
			}
		}

		loadUserData()
	}, [])

	const updateUser = (userData) => {
		setUser(userData)
		if (userData) {
			localStorage.setItem('user', JSON.stringify(userData))
		} else {
			localStorage.removeItem('user')
		}
	}

	return {
		user,
		loading,
		updateUser
	}
}
