import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function handler(req, res) {

	if (req.method === 'GET') {
		try {
			const token = req.headers.authorization

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			// Hardcoded URL for Docker network communication
			const backendUrl = 'http://backend:5249'
			// Llamar al endpoint del backend para obtener todas las imágenes
			const response = await axios.post(
				`${backendUrl}/api/PropertyImages/all`,
				{}, // Body vacío
				{
					headers: {
						Authorization: token
					}
				}
			)

			res.status(200).json(response.data)
		} catch (error) {
			console.error('Error fetching all property images:', error)
			const status = error.response?.status || 500
			const message =
				error.response?.data?.message || error.message || 'Internal Server Error'
			res.status(status).json({ message })
		}
	} else {
		res.setHeader('Allow', ['GET'])
		res.status(405).end(`Method ${req.method} Not Allowed`)
	}
}
