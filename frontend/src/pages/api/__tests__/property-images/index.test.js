import handler from '../../property-images/index'
import axios from 'axios'

jest.mock('axios')

describe('/api/property-images', () => {
  let req, res

  beforeEach(() => {
    req = {
      method: 'GET',
      headers: {
        authorization: 'Bearer test-token'
      }
    }
    res = {
      status: jest.fn().mockReturnThis(),
      json: jest.fn(),
      setHeader: jest.fn(),
      end: jest.fn()
    }
    jest.clearAllMocks()
  })

  it('should fetch property images successfully', async () => {
    const mockImages = [
      { id: 1, propertyId: 1, fileUrl: 'image1.jpg', enabled: true },
      { id: 2, propertyId: 2, fileUrl: 'image2.jpg', enabled: true }
    ]
    axios.post.mockResolvedValueOnce({ data: mockImages })

    await handler(req, res)

    expect(axios.post).toHaveBeenCalledWith(
      'http://backend:5249/api/PropertyImages/all',
      {},
      {
        headers: {
          Authorization: 'Bearer test-token'
        }
      }
    )
    expect(res.status).toHaveBeenCalledWith(200)
    expect(res.json).toHaveBeenCalledWith(mockImages)
  })

  it('should return 401 when no authorization token', async () => {
    req.headers = {}

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(401)
    expect(res.json).toHaveBeenCalledWith({ message: 'Unauthorized' })
    expect(axios.post).not.toHaveBeenCalled()
  })

  it('should return 401 when authorization token is null', async () => {
    req.headers.authorization = null

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(401)
    expect(res.json).toHaveBeenCalledWith({ message: 'Unauthorized' })
    expect(axios.post).not.toHaveBeenCalled()
  })

  it('should handle backend error with specific status', async () => {
    const mockError = {
      response: {
        status: 403,
        data: { message: 'Forbidden' }
      }
    }
    axios.post.mockRejectedValueOnce(mockError)

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(403)
    expect(res.json).toHaveBeenCalledWith({ message: 'Forbidden' })
  })

  it('should handle backend error without response', async () => {
    const mockError = new Error('Network error')
    axios.post.mockRejectedValueOnce(mockError)

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(500)
    expect(res.json).toHaveBeenCalledWith({ message: 'Network error' })
  })

  it('should handle backend error without specific message', async () => {
    const mockError = {
      response: {
        status: 500
      }
    }
    axios.post.mockRejectedValueOnce(mockError)

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(500)
    expect(res.json).toHaveBeenCalledWith({ message: 'Internal Server Error' })
  })

  it('should reject non-GET requests', async () => {
    req.method = 'POST'

    await handler(req, res)

    expect(res.setHeader).toHaveBeenCalledWith('Allow', ['GET'])
    expect(res.status).toHaveBeenCalledWith(405)
    expect(res.end).toHaveBeenCalledWith('Method POST Not Allowed')
    expect(axios.post).not.toHaveBeenCalled()
  })

  it('should handle PUT request', async () => {
    req.method = 'PUT'

    await handler(req, res)

    expect(res.setHeader).toHaveBeenCalledWith('Allow', ['GET'])
    expect(res.status).toHaveBeenCalledWith(405)
    expect(res.end).toHaveBeenCalledWith('Method PUT Not Allowed')
  })

  it('should use hardcoded backend URL for Docker network', async () => {
    const mockImages = []
    axios.post.mockResolvedValueOnce({ data: mockImages })

    await handler(req, res)

    expect(axios.post).toHaveBeenCalledWith(
      'http://backend:5249/api/PropertyImages/all',
      {},
      {
        headers: {
          Authorization: 'Bearer test-token'
        }
      }
    )
  })
})
