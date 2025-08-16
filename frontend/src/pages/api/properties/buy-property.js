import axios from 'axios'

export default async function handler(req, res) {
	if (req.method === 'PATCH') {
		try {
			const { propertyId, newOwnerId } = req.body
			const token = req.headers.authorization

			console.log('Buy property API called:', { propertyId, newOwnerId, hasToken: !!token })

			if (!token) {
				return res.status(401).json({ message: 'Unauthorized' })
			}

			if (!propertyId) {
				return res.status(400).json({ message: 'Property ID is required' })
			}

			if (!newOwnerId) {
				return res.status(400).json({ message: 'New owner ID is required' })
			}

			// First, change forSale to false
			console.log('Step 1: Updating forSale to false')
			await axios.patch(
				`http://backend:5249/api/properties/${propertyId}/for-sale`,
				{ forSale: false },
				{
					headers: {
						Authorization: token,
						'Content-Type': 'application/json'
					}
				}
			)

			console.log('Step 2: Updating owner to:', newOwnerId)
			// Then, update the owner
			await axios.patch(
				`http://backend:5249/api/properties/${propertyId}/owner`,
				{ ownerId: newOwnerId },
				{
					headers: {
						Authorization: token,
						'Content-Type': 'application/json'
					}
				}
			)

			console.log('Property purchase completed successfully')
			res.status(200).json({ message: 'Property purchased successfully' })
		} catch (error) {
			console.error('Buy property API error:', {
				status: error.response?.status,
				statusText: error.response?.statusText,
				data: error.response?.data,
				message: error.message
			})
			
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
