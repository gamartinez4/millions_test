import handler from '../../auth/login'
import axios from 'axios'

jest.mock('axios')

describe('/api/auth/login', () => {
  let req, res

  beforeEach(() => {
    req = {
      method: 'POST',
      body: {
        username: 'testuser',
        password: 'testpass'
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

  it('should handle successful login', async () => {
    const mockResponse = {
      data: {
        token: 'jwt-token',
        owner: { id: 1, username: 'testuser' }
      }
    }
    axios.post.mockResolvedValueOnce(mockResponse)

    await handler(req, res)

    expect(axios.post).toHaveBeenCalledWith(
      'http://backend:5249/api/owners/login',
      {
        username: 'testuser',
        password: 'testpass'
      }
    )
    expect(res.status).toHaveBeenCalledWith(200)
    expect(res.json).toHaveBeenCalledWith(mockResponse.data)
  })

  it('should handle login error with specific status', async () => {
    const mockError = {
      response: {
        status: 401,
        data: { message: 'Invalid credentials' }
      }
    }
    axios.post.mockRejectedValueOnce(mockError)

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(401)
    expect(res.json).toHaveBeenCalledWith({ message: 'Invalid credentials' })
  })

  it('should handle login error without response', async () => {
    const mockError = new Error('Network error')
    axios.post.mockRejectedValueOnce(mockError)

    await handler(req, res)

    expect(res.status).toHaveBeenCalledWith(500)
    expect(res.json).toHaveBeenCalledWith({ message: 'Network error' })
  })

  it('should handle login error without specific message', async () => {
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

  it('should reject non-POST requests', async () => {
    req.method = 'GET'

    await handler(req, res)

    expect(res.setHeader).toHaveBeenCalledWith('Allow', ['POST'])
    expect(res.status).toHaveBeenCalledWith(405)
    expect(res.end).toHaveBeenCalledWith('Method GET Not Allowed')
    expect(axios.post).not.toHaveBeenCalled()
  })

  it('should handle PUT request', async () => {
    req.method = 'PUT'

    await handler(req, res)

    expect(res.setHeader).toHaveBeenCalledWith('Allow', ['POST'])
    expect(res.status).toHaveBeenCalledWith(405)
    expect(res.end).toHaveBeenCalledWith('Method PUT Not Allowed')
  })

  it('should handle DELETE request', async () => {
    req.method = 'DELETE'

    await handler(req, res)

    expect(res.setHeader).toHaveBeenCalledWith('Allow', ['POST'])
    expect(res.status).toHaveBeenCalledWith(405)
    expect(res.end).toHaveBeenCalledWith('Method DELETE Not Allowed')
  })
})
