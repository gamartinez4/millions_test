import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function handler(req, res) {
	if (req.method === 'PATCH') {
		try {
			const { propertyId } = req.query
			const { ownerId } = req.body
			const token = req.headers.authorization

			console.log('UpdateOwner API - propertyId:', propertyId, 'ownerId:', ownerId, 'type:', typeof ownerId)

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			if (!propertyId) {
				return res.status(400).json({ message: 'Property ID is required' })
			}

			// Validate ownerId: must be an integer or null
			if (ownerId !== null && !Number.isInteger(ownerId)) {
				return res.status(400).json({ message: 'Invalid "ownerId" value. Must be an integer or null.' })
			}

			const response = await axios.patch(
				`http://backend:5249/api/properties/${propertyId}/owner`,
				{ ownerId },
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
			console.error('Error in update-owner API:', error.response?.data || error.message)
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
