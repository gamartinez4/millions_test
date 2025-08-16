import { renderHook, waitFor } from '@testing-library/react'
import axios from 'axios'
import { useProperties } from '../useProperties'

jest.mock('axios')

// Mock localStorage
const localStorageMock = {
  getItem: jest.fn(),
  setItem: jest.fn(),
  removeItem: jest.fn(),
  clear: jest.fn()
}

Object.defineProperty(window, 'localStorage', {
  value: localStorageMock
})

describe('useProperties', () => {
  beforeEach(() => {
    jest.clearAllMocks()
    localStorageMock.getItem.mockClear()
  })

  it('should fetch properties successfully', async () => {
    const mockProperties = [
      { id: 1, name: 'Property 1', price: 100000 },
      { id: 2, name: 'Property 2', price: 200000 }
    ]

    // Mock token in localStorage
    localStorageMock.getItem.mockReturnValue('mock-token-123')
    axios.get.mockResolvedValueOnce({ data: mockProperties })

    const { result } = renderHook(() => useProperties())

    // Initially loading should be true
    expect(result.current.loading).toBe(true)
    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBeNull()

    // Wait for the effect to complete
    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual(mockProperties)
    expect(result.current.error).toBeNull()
    expect(axios.get).toHaveBeenCalledWith('/api/properties', {
      headers: {
        Authorization: 'Bearer mock-token-123'
      }
    })
  })

  it('should handle fetch error when token exists', async () => {
    const errorMessage = 'Network Error'
    localStorageMock.getItem.mockReturnValue('mock-token-123')
    axios.get.mockRejectedValueOnce(new Error(errorMessage))

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe(errorMessage)
    expect(axios.get).toHaveBeenCalledWith('/api/properties', {
      headers: {
        Authorization: 'Bearer mock-token-123'
      }
    })
  })

  it('should handle authentication error when no token', async () => {
    // No token in localStorage
    localStorageMock.getItem.mockReturnValue(null)

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe('No authentication token found')
    expect(axios.get).not.toHaveBeenCalled()
  })

  it('should handle fetch error with custom error response', async () => {
    const customError = {
      response: {
        data: { message: 'Custom error message' }
      }
    }
    localStorageMock.getItem.mockReturnValue('mock-token-123')
    axios.get.mockRejectedValueOnce(customError)

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe('Failed to fetch properties')
  })

  it('should set error to default message when no error message available', async () => {
    localStorageMock.getItem.mockReturnValue('mock-token-123')
    axios.get.mockRejectedValueOnce({})

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe('Failed to fetch properties')
  })
})
