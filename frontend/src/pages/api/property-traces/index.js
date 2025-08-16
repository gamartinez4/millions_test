import axios from 'axios'
import { API_BASE_URL } from '@/lib/config'

export default async function handler(req, res) {
	if (req.method === 'POST') {
		try {
			const { dateSale, name, value, tax, propertyId } = req.body
			const token = req.headers.authorization

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			if (!name || !propertyId) {
				return res.status(400).json({ message: 'Name and PropertyId are required' })
			}

			if (typeof value !== 'number' || value <= 0) {
				return res.status(400).json({ message: 'Value must be a positive number' })
			}

			const response = await axios.post(
				`http://backend:5249/api/PropertyTraces`,
				{
					dateSale: dateSale || new Date().toISOString(),
					name,
					value,
					tax: tax || 0,
					propertyId
				},
				{
					headers: {
						Authorization: token,
						'Content-Type': 'application/json'
					}
				}
			)

			console.log('PropertyTrace created successfully:', response.data)
			res.status(201).json(response.data)
		} catch (error) {
			console.error('Error in property-traces POST API:', error.response?.data || error.message)
			const status = error.response?.status || 500
			const message = error.response?.data?.message || error.message || 'Internal Server Error'
			res.status(status).json({ message })
		}
	} else if (req.method === 'GET') {
		try {
			const token = req.headers.authorization

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			const response = await axios.get(
				`http://backend:5249/api/PropertyTraces`,
				{
					headers: {
						Authorization: token
					}
				}
			)

			console.log('PropertyTraces retrieved successfully. Count:', response.data?.length || 0)
			console.log('PropertyTraces data:', response.data)
			res.status(200).json(response.data)
		} catch (error) {
			console.error('Error in property-traces GET API:', error.response?.data || error.message)
			const status = error.response?.status || 500
			const message = error.response?.data?.message || error.message || 'Internal Server Error'
			res.status(status).json({ message })
		}
	} else {
		res.setHeader('Allow', ['GET', 'POST'])
		res.status(405).end(`Method ${req.method} Not Allowed`)
	}
}
