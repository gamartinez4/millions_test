import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function handler(req, res) {
	if (req.method === 'PATCH') {
		try {
			const { propertyId } = req.query
			const { price } = req.body
			const token = req.headers.authorization

			console.log('UpdatePrice API - propertyId:', propertyId, 'price:', price, 'type:', typeof price)

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			if (!propertyId) {
				return res.status(400).json({ message: 'Property ID is required' })
			}

			if (typeof price !== 'number' || price < 0) {
				return res.status(400).json({ message: 'Price must be a valid positive number' })
			}

			const response = await axios.patch(
				`http://backend:5249/api/properties/${propertyId}/price`,
				{ price },
				{
					headers: {
						Authorization: token,
						'Content-Type': 'application/json'
					}
				}
			)

			console.log('Backend response status:', response.status)
			console.log('Backend response data:', response.data)

			res.status(204).end()
		} catch (error) {
			console.error('Error in update-price API:', error.response?.data || error.message)
			const status = error.response?.status || 500
			const message = error.response?.data?.message || error.message || 'Internal Server Error'
			res.status(status).json({ message })
		}
	} else {
		res.setHeader('Allow', ['PATCH'])
		res.status(405).end(`Method ${req.method} Not Allowed`)
	}
}
