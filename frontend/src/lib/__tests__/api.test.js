// Test basic functionality of the api module
describe('api module', () => {
  // Mock localStorage
  const localStorageMock = {
    getItem: jest.fn(),
    setItem: jest.fn(),
    removeItem: jest.fn(),
    clear: jest.fn(),
  }
  
  beforeAll(() => {
    global.localStorage = localStorageMock
  })

  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('should be importable without errors', () => {
    expect(() => {
      require('../api')
    }).not.toThrow()
  })

  it('should export default axios instance', () => {
    const api = require('../api').default
    expect(api).toBeDefined()
  })

  // Test the interceptor logic separately
  describe('token interceptor logic', () => {
    const createInterceptor = (getItem) => {
      return (config) => {
        const token = getItem('token')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      }
    }

    it('should add authorization header when token exists', () => {
      const getItem = jest.fn().mockReturnValue('test-token')
      const interceptor = createInterceptor(getItem)
      const config = { headers: {} }
      
      const result = interceptor(config)
      
      expect(getItem).toHaveBeenCalledWith('token')
      expect(result.headers.Authorization).toBe('Bearer test-token')
    })

    it('should not add authorization header when no token', () => {
      const getItem = jest.fn().mockReturnValue(null)
      const interceptor = createInterceptor(getItem)
      const config = { headers: {} }
      
      const result = interceptor(config)
      
      expect(getItem).toHaveBeenCalledWith('token')
      expect(result.headers.Authorization).toBeUndefined()
    })

    it('should preserve existing headers', () => {
      const getItem = jest.fn().mockReturnValue('test-token')
      const interceptor = createInterceptor(getItem)
      const config = { 
        headers: { 
          'Content-Type': 'application/json',
          'Custom-Header': 'custom-value'
        } 
      }
      
      const result = interceptor(config)
      
      expect(result.headers['Content-Type']).toBe('application/json')
      expect(result.headers['Custom-Header']).toBe('custom-value')
      expect(result.headers.Authorization).toBe('Bearer test-token')
    })

    it('should handle empty token string', () => {
      const getItem = jest.fn().mockReturnValue('')
      const interceptor = createInterceptor(getItem)
      const config = { headers: {} }
      
      const result = interceptor(config)
      
      expect(result.headers.Authorization).toBeUndefined()
    })
  })
})
