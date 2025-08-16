import { renderHook, waitFor } from '@testing-library/react'
import axios from 'axios'
import { useProperties } from '../useProperties'

jest.mock('axios')

describe('useProperties', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  it('should fetch properties successfully', async () => {
    const mockProperties = [
      { id: 1, name: 'Property 1', price: 100000 },
      { id: 2, name: 'Property 2', price: 200000 }
    ]

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
    expect(axios.get).toHaveBeenCalledWith('/api/properties')
  })

  it('should handle fetch error', async () => {
    const errorMessage = 'Network Error'
    axios.get.mockRejectedValueOnce(new Error(errorMessage))

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe(errorMessage)
    expect(axios.get).toHaveBeenCalledWith('/api/properties')
  })

  it('should handle fetch error with custom error response', async () => {
    const customError = {
      response: {
        data: { message: 'Custom error message' }
      }
    }
    axios.get.mockRejectedValueOnce(customError)

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe('Failed to fetch properties')
  })

  it('should set error to default message when no error message available', async () => {
    axios.get.mockRejectedValueOnce({})

    const { result } = renderHook(() => useProperties())

    await waitFor(() => {
      expect(result.current.loading).toBe(false)
    })

    expect(result.current.properties).toEqual([])
    expect(result.current.error).toBe('Failed to fetch properties')
  })
})
