import { useRouter } from 'next/router'
import { useAuth } from '@/context/AuthContext'

/**
 * Hook para manejar la navegación del dashboard
 */
export const useNavigation = () => {
	const router = useRouter()
	const { logout, setSelectedProperty } = useAuth()

	// Navegar a mis propiedades
	const handleViewMyProperties = () => {
		router.push('/my-properties')
	}

	// Navegar al dashboard
	const handleViewDashboard = () => {
		router.push('/dashboard')
	}

	// Navegar a una propiedad específica
	const handleViewProperty = (propertyId) => {
		router.push(`/properties/${propertyId}`)
	}

	// Navegar a los detalles de una propiedad sin pasar datos por URL
	const handleViewPropertyDetails = (property) => {
		setSelectedProperty(property)
		router.push('/property-details')
	}

	// Navegar al perfil del usuario
	const handleViewProfile = () => {
		router.push('/profile')
	}

	// Manejar logout
	const handleLogout = async () => {
		try {
			await logout()
		} catch (error) {
			console.error('Error during logout:', error)
		}
	}

	// Navegar hacia atrás
	const handleGoBack = () => {
		router.back()
	}

	// Verificar si estamos en una ruta específica
	const isCurrentRoute = (route) => {
		return router.pathname === route
	}

	return {
		// Handlers de navegación
		handleViewMyProperties,
		handleViewDashboard,
		handleViewProperty,
		handleViewPropertyDetails,
		handleViewProfile,
		handleLogout,
		handleGoBack,
		
		// Utilidades
		isCurrentRoute,
		currentPath: router.pathname,
		
		// Router directo para casos especiales
		router
	}
}
