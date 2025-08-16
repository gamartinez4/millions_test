import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function login(req, res) {

	if (req.method === 'POST') {
		try {
			const { username, password } = req.body
			// Hardcoded URL for Docker network communication
			const backendUrl = 'http://backend:5249'
			
			console.log('Frontend API: Attempting login with:', { username, backendUrl })
			
			const response = await axios.post(
				`${backendUrl}/api/owners/login`,
				{
					username,
					password,
				}
			)
			
			console.log('Frontend API: Login successful')
			res.status(200).json(response.data)
		} catch (error) {
			console.error('Frontend API: Login error:', {
				status: error.response?.status,
				statusText: error.response?.statusText,
				data: error.response?.data,
				message: error.message,
				code: error.code
			})
			
			const status = error.response?.status || 500
			const message =
				error.response?.data?.message || error.message || 'Internal Server Error'
			res.status(status).json({ message })
		}
	} else {
		res.setHeader('Allow', ['POST'])
		res.status(405).end(`Method ${req.method} Not Allowed`)
	}
}



