import { renderHook, waitFor } from '@testing-library/react'
import { useBuyProperty } from '../useBuyProperty'
import { propertyService } from '@/services/propertyService'

jest.mock('@/services/propertyService')

describe('useBuyProperty', () => {
	beforeEach(() => {
		jest.clearAllMocks()
	})

	it('should initially have correct default state', () => {
		const { result } = renderHook(() => useBuyProperty())

		expect(result.current.loading).toBe(false)
		expect(result.current.error).toBeNull()
		expect(typeof result.current.buyProperty).toBe('function')
		expect(typeof result.current.clearError).toBe('function')
	})

	it('should successfully buy a property', async () => {
		propertyService.buyProperty.mockResolvedValueOnce()

		const { result } = renderHook(() => useBuyProperty())

		const propertyId = 1
		const newOwnerId = 123

		let buyResult
		await waitFor(async () => {
			buyResult = await result.current.buyProperty(propertyId, newOwnerId)
		})

		expect(buyResult).toBe(true)
		expect(result.current.loading).toBe(false)
		expect(result.current.error).toBeNull()
		expect(propertyService.buyProperty).toHaveBeenCalledWith(propertyId, newOwnerId)
	})

	it('should handle buy property error', async () => {
		const errorMessage = 'Failed to buy property'
		propertyService.buyProperty.mockRejectedValueOnce(new Error(errorMessage))

		const { result } = renderHook(() => useBuyProperty())

		const propertyId = 1
		const newOwnerId = 123

		let buyResult
		await waitFor(async () => {
			buyResult = await result.current.buyProperty(propertyId, newOwnerId)
		})

		expect(buyResult).toBe(false)
		expect(result.current.loading).toBe(false)
		expect(result.current.error).toBe(errorMessage)
		expect(propertyService.buyProperty).toHaveBeenCalledWith(propertyId, newOwnerId)
	})

	it('should handle validation error for missing parameters', async () => {
		const { result } = renderHook(() => useBuyProperty())

		let buyResult
		await waitFor(async () => {
			buyResult = await result.current.buyProperty(null, null)
		})

		expect(buyResult).toBe(false)
		expect(result.current.loading).toBe(false)
		expect(result.current.error).toBe('Property ID and new owner ID are required')
		expect(propertyService.buyProperty).not.toHaveBeenCalled()
	})

	it('should clear error state', () => {
		const { result } = renderHook(() => useBuyProperty())

		// Manually set an error to test clearing
		result.current.clearError()

		expect(result.current.error).toBeNull()
	})

	it('should set loading state during purchase', async () => {
		// Create a promise that we can control
		let resolvePromise
		const controlledPromise = new Promise((resolve) => {
			resolvePromise = resolve
		})

		propertyService.buyProperty.mockReturnValueOnce(controlledPromise)

		const { result } = renderHook(() => useBuyProperty())

		// Start the buy process but don't await it yet
		result.current.buyProperty(1, 123)

		// Wait for loading to become true
		await waitFor(() => {
			expect(result.current.loading).toBe(true)
		})

		// Now resolve the promise
		resolvePromise()
		await waitFor(() => {
			expect(result.current.loading).toBe(false)
		})
	})

	it('should handle service error with default message', async () => {
		propertyService.buyProperty.mockRejectedValueOnce({})

		const { result } = renderHook(() => useBuyProperty())

		let buyResult
		await waitFor(async () => {
			buyResult = await result.current.buyProperty(1, 123)
		})

		expect(buyResult).toBe(false)
		expect(result.current.loading).toBe(false)
		expect(result.current.error).toBe('Error al comprar la propiedad')
		expect(propertyService.buyProperty).toHaveBeenCalledWith(1, 123)
	})
})
