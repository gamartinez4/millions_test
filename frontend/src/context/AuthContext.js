import { createContext, useState, useContext, useEffect } from 'react'
import axios from 'axios'
import { useRouter } from 'next/router'

const AuthContext = createContext()

export function AuthProvider({ children }) {
	const [user, setUser] = useState(null)
	const [token, setToken] = useState(null)
	const [loading, setLoading] = useState(true)
	const [sharedProperties, setSharedProperties] = useState(null)
	const [selectedProperty, setSelectedProperty] = useState(null)

	const router = useRouter()

	// Safe localStorage access function
	const getStoredAuth = () => {
		if (typeof window !== 'undefined') {
			try {
				const storedToken = localStorage.getItem('token')
				const storedUser = localStorage.getItem('user')
				return !!(storedToken && storedUser)
			} catch (error) {
				console.error('Error accessing localStorage:', error)
				return false
			}
		}
		return false
	}

    useEffect(() => {
        setLoading(false)
    }, [])

    const login = async (username, password) => {
        const { data } = await axios.post('/api/auth/login', {
            username,
            password,
        })

        const authToken = data.token
        const ownerData = data.owner

        // Persist token in plain form (no JSON wrapper)
       
        axios.defaults.headers.common['Authorization'] = `Bearer ${authToken}`
       

        // Create Owner model instance
        const ownerModel = {
            id: ownerData.id,
            name: ownerData.name,
            address: ownerData.address,
            photo: ownerData.photo,
            birthday: ownerData.birthday,
            username: ownerData.username
		}
        

        // Update user in context and localStorage
        setToken(authToken)
        setUser(ownerModel)
        localStorage.setItem('token', authToken)
        localStorage.setItem('user', JSON.stringify(ownerModel))
    }


	const logout = async () => {
		try {
			await axios.post('/api/auth/logout')
			setUser(null)
			setToken(null)
			localStorage.removeItem('token')
            localStorage.removeItem('user')
			delete axios.defaults.headers.common['Authorization']
			router.push('/')
		} catch (error) {
			console.error('Logout failed', error)
		}
	}

	return (
		<AuthContext.Provider
			value={{ 
				isAuthenticated: getStoredAuth(), 
				user, 
				token, 
				login, 
				logout, 
				loading,
				sharedProperties,
				setSharedProperties,
				selectedProperty,
				setSelectedProperty
			}}
		>
			{children}
		</AuthContext.Provider>
	)
}

export const useAuth = () => useContext(AuthContext)



