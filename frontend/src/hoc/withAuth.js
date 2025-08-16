import { useAuth } from '@/context/AuthContext'
import { useRouter } from 'next/router'
import { useEffect } from 'react'

const WithAuth = (WrappedComponent) => {
	return (props) => {
		const { isAuthenticated, loading } = useAuth()
		const router = useRouter()

		useEffect(() => {
			if (!loading && !isAuthenticated) {
				router.push('/login')
			}
		}, [isAuthenticated, loading, router])

		if (loading || !isAuthenticated) {
			return <div>Loading...</div>
		}

		return <WrappedComponent {...props} />
	}
}

export default WithAuth

