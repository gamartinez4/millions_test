import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function handler(req, res) {
	if (req.method === 'PATCH') {
		try {
			const { propertyId } = req.query
			const { forSale } = req.body
			const token = req.headers.authorization

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			if (!propertyId) {
				return res.status(400).json({ message: 'Property ID is required' })
			}

			if (typeof forSale !== 'boolean') {
				return res.status(400).json({ message: 'forSale must be a boolean value' })
			}

			const response = await axios.patch(
				`http://backend:5249/api/properties/${propertyId}/for-sale`,
				{ forSale },
				{
					headers: {
						Authorization: token,
						'Content-Type': 'application/json'
					}
				}
			)

			res.status(204).end()
		} catch (error) {
			const status = error.response?.status || 500
			const message =
				error.response?.data?.message || error.message || 'Internal Server Error'
			res.status(status).json({ message })
		}
	} else {
		res.setHeader('Allow', ['PATCH'])
		res.status(405).end(`Method ${req.method} Not Allowed`)
	}
}
