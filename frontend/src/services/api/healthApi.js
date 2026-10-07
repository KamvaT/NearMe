import { apiClient } from './apiClient.js'

/** Liveness probe. Unversioned by design (docs/adr/0003). Resolves to `{ status: 'healthy' }`. */
export function getHealth(options) {
  return apiClient.get('/health', options)
}
