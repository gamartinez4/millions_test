import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function handler(req, res) {
	if (req.method === 'GET') {
		try {
			const token = req.headers.authorization

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			const response = await axios.get(
				`http://backend:5249/api/properties`,
				{
					headers: {
						Authorization: token
					}
				}
			)
			res.status(200).json(response.data)
		} catch (error) {
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
