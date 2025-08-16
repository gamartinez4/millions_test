// Centralized app configuration (no .env as requested)
// For server-side API routes (runs in Docker container)
export const API_BASE_URL = process.env.API_BASE_URL || 'http://backend:5249'

// For client-side API calls (runs in browser)
export const CLIENT_API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5249'


