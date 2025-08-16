import { useState, useEffect } from 'react'
import { useRouter } from 'next/router'
import { useAuth } from '@/context/AuthContext'
import { useProperties } from './useProperties'
import { 
	useDisplayProperties, 
	useFilteredProperties, 
	usePropertyStats 
} from '@/utils/propertyFilters'

export const useDashboard = () => {
	const { logout } = useAuth()
	const router = useRouter()
	const [user, setUser] = useState(null)
	
	// Filter states
	const [filters, setFilters] = useState({
		nameFilter: '',
		addressFilter: '',
		minPriceFilter: '',
		maxPriceFilter: ''
	})

	// Properties data
	const { properties, loading, error, refetchProperties } = useProperties()

	// Load user from localStorage
	useEffect(() => {
		const storedUser = localStorage.getItem('user')
		if (storedUser) {
			setUser(JSON.parse(storedUser))
		}
	}, [])

	// Get display properties (excluding user's properties)
	const allDisplayProperties = useDisplayProperties(properties, user)

	// Apply filters
	const filteredProperties = useFilteredProperties(allDisplayProperties, filters)

	// Get statistics
	const stats = usePropertyStats(allDisplayProperties, user, filteredProperties)

	// Filter handlers
	const handleFilterChange = (filterName, value) => {
		setFilters(prev => ({
			...prev,
			[filterName]: value
		}))
	}

	const clearFilters = () => {
		setFilters({
			nameFilter: '',
			addressFilter: '',
			minPriceFilter: '',
			maxPriceFilter: ''
		})
	}

	// Navigation handlers
	const handleViewMyProperties = () => {
		router.push('/my-properties')
	}

	const handleLogout = () => {
		logout()
	}

	return {
		// User data
		user,
		
		// Properties data
		allDisplayProperties,
		filteredProperties,
		loading,
		error,
		refetchProperties,
		
		// Filters
		filters,
		handleFilterChange,
		clearFilters,
		
		// Statistics
		stats,
		
		// Navigation
		handleViewMyProperties,
		handleLogout
	}
}
