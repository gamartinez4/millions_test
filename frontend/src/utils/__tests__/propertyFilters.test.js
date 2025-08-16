import { renderHook } from '@testing-library/react'
import {
  useDisplayProperties,
  useFilteredProperties,
  getFirstPropertyImage,
  isPropertyOwned,
  mergePropertiesWithImages
} from '../propertyFilters'

describe('propertyFilters', () => {
  describe('useDisplayProperties', () => {
    const mockUser = { id: 1 }
    const mockProperties = [
      { id: 1, name: 'Property 1', ownerId: 1 },
      { id: 2, name: 'Property 2', ownerId: 2 },
      { id: 3, name: 'Property 3', ownerId: 3 }
    ]

    it('should filter out properties owned by user', () => {
      const { result } = renderHook(() => 
        useDisplayProperties(mockProperties, mockUser)
      )

      expect(result.current).toHaveLength(2)
      expect(result.current.map(p => p.id)).toEqual([2, 3])
    })

    it('should return all properties when user is null', () => {
      const { result } = renderHook(() => 
        useDisplayProperties(mockProperties, null)
      )

      expect(result.current).toHaveLength(3)
    })

    it('should return empty array when properties is null', () => {
      const { result } = renderHook(() => 
        useDisplayProperties(null, mockUser)
      )

      expect(result.current).toEqual([])
    })

    it('should return empty array when properties is not an array', () => {
      const { result } = renderHook(() => 
        useDisplayProperties('not an array', mockUser)
      )

      expect(result.current).toEqual([])
    })
  })

  describe('useFilteredProperties', () => {
    const mockProperties = [
      { id: 1, name: 'Beach House', address: 'Miami Beach', price: 500000 },
      { id: 2, name: 'Mountain Cabin', address: 'Colorado Mountains', price: 300000 },
      { id: 3, name: 'City Apartment', address: 'New York City', price: 800000 }
    ]

    it('should filter by name', () => {
      const filters = { nameFilter: 'Beach' }
      const { result } = renderHook(() => 
        useFilteredProperties(mockProperties, filters)
      )

      expect(result.current).toHaveLength(1)
      expect(result.current[0].name).toBe('Beach House')
    })

    it('should filter by address', () => {
      const filters = { addressFilter: 'New York' }
      const { result } = renderHook(() => 
        useFilteredProperties(mockProperties, filters)
      )

      expect(result.current).toHaveLength(1)
      expect(result.current[0].address).toBe('New York City')
    })

    it('should filter by minimum price', () => {
      const filters = { minPriceFilter: '400000' }
      const { result } = renderHook(() => 
        useFilteredProperties(mockProperties, filters)
      )

      expect(result.current).toHaveLength(2)
      expect(result.current.map(p => p.price)).toEqual([500000, 800000])
    })

    it('should filter by maximum price', () => {
      const filters = { maxPriceFilter: '400000' }
      const { result } = renderHook(() => 
        useFilteredProperties(mockProperties, filters)
      )

      expect(result.current).toHaveLength(1)
      expect(result.current[0].price).toBe(300000)
    })

    it('should apply multiple filters', () => {
      const filters = { 
        nameFilter: 'house',
        minPriceFilter: '200000',
        maxPriceFilter: '600000'
      }
      const { result } = renderHook(() => 
        useFilteredProperties(mockProperties, filters)
      )

      expect(result.current).toHaveLength(1)
      expect(result.current[0].name).toBe('Beach House')
    })

    it('should return empty array when properties is null', () => {
      const filters = { nameFilter: 'test' }
      const { result } = renderHook(() => 
        useFilteredProperties(null, filters)
      )

      expect(result.current).toEqual([])
    })

    it('should be case insensitive', () => {
      const filters = { nameFilter: 'BEACH' }
      const { result } = renderHook(() => 
        useFilteredProperties(mockProperties, filters)
      )

      expect(result.current).toHaveLength(1)
      expect(result.current[0].name).toBe('Beach House')
    })
  })

  describe('getFirstPropertyImage', () => {
    it('should return first image when images exist', () => {
      const property = {
        images: [
          { id: 1, url: 'image1.jpg' },
          { id: 2, url: 'image2.jpg' }
        ]
      }

      const result = getFirstPropertyImage(property)
      expect(result).toEqual({ id: 1, url: 'image1.jpg' })
    })

    it('should return null when no images exist', () => {
      const property = { images: [] }
      const result = getFirstPropertyImage(property)
      expect(result).toBeNull()
    })

    it('should return null when images is undefined', () => {
      const property = {}
      const result = getFirstPropertyImage(property)
      expect(result).toBeNull()
    })
  })

  describe('isPropertyOwned', () => {
    const mockUser = {
      properties: [
        { id: 1, name: 'My Property' },
        { id: 2, name: 'My Other Property' }
      ]
    }

    it('should return true when user owns the property', () => {
      const property = { id: 1, name: 'Some Property' }
      const result = isPropertyOwned(property, mockUser)
      expect(result).toBe(true)
    })

    it('should return false when user does not own the property', () => {
      const property = { id: 3, name: 'Some Property' }
      const result = isPropertyOwned(property, mockUser)
      expect(result).toBe(false)
    })

    it('should return false when user is null', () => {
      const property = { id: 1, name: 'Some Property' }
      const result = isPropertyOwned(property, null)
      expect(result).toBe(false)
    })

    it('should return false when user has no properties', () => {
      const property = { id: 1, name: 'Some Property' }
      const userWithoutProperties = {}
      const result = isPropertyOwned(property, userWithoutProperties)
      expect(result).toBe(false)
    })
  })

  describe('mergePropertiesWithImages', () => {
    const mockProperties = [
      { id: 1, name: 'Property 1' },
      { id: 2, name: 'Property 2' }
    ]

    const mockImages = [
      { id: 1, propertyId: 1, url: 'image1.jpg', enabled: true },
      { id: 2, propertyId: 1, url: 'image2.jpg', enabled: false },
      { id: 3, propertyId: 2, url: 'image3.jpg', enabled: true }
    ]

    it('should merge properties with their enabled images', () => {
      const result = mergePropertiesWithImages(mockProperties, mockImages)
      
      expect(result).toHaveLength(2)
      expect(result[0].images).toHaveLength(1)
      expect(result[0].images[0].url).toBe('image1.jpg')
      expect(result[1].images).toHaveLength(1)
      expect(result[1].images[0].url).toBe('image3.jpg')
    })

    it('should return properties as-is when images is null', () => {
      const result = mergePropertiesWithImages(mockProperties, null)
      expect(result).toEqual(mockProperties)
    })

    it('should return empty array when properties is null', () => {
      const result = mergePropertiesWithImages(null, mockImages)
      expect(result).toEqual([])
    })

    it('should return properties as-is when images is not an array', () => {
      const result = mergePropertiesWithImages(mockProperties, 'not an array')
      expect(result).toEqual(mockProperties)
    })

    it('should only include enabled images', () => {
      const result = mergePropertiesWithImages(mockProperties, mockImages)
      
      // Property 1 should only have 1 enabled image out of 2 total
      expect(result[0].images).toHaveLength(1)
      expect(result[0].images[0].enabled).toBe(true)
    })
  })
})
