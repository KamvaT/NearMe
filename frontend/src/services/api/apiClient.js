import { ApiError } from './ApiError.js'

/**
 * Central HTTP client: every request to the NearMe API goes through here
 * (Frontend Architecture Specification §20).
 *
 * The frontend is not a security boundary. This client only shapes requests and
 * responses; all authorization is enforced by the API.
 */

export const API_V1 = '/api/v1'

const DEFAULT_TIMEOUT_MS = 10_000

const NETWORK_ERROR_MESSAGE = 'Unable to reach NearMe. Check your connection and try again.'
const TIMEOUT_MESSAGE = 'NearMe is taking too long to respond. Please try again.'
const GENERIC_ERROR_MESSAGE = 'Something went wrong. Please try again.'

export function getApiBaseUrl() {
  const baseUrl = import.meta.env.VITE_API_BASE_URL
  if (!baseUrl) {
    throw new Error('VITE_API_BASE_URL is not configured. Copy frontend/.env.example to frontend/.env.local.')
  }
  return baseUrl
}

function trimTrailingSlashes(url) {
  return url.replace(/\/+$/, '')
}

/**
 * @param {string} path Absolute API path, e.g. `/health` or `${API_V1}/categories`.
 * @param {{ method?: string, body?: unknown, signal?: AbortSignal, timeoutMs?: number, baseUrl?: string }} [options]
 */
export async function request(path, options = {}) {
  const { method = 'GET', body, signal, timeoutMs = DEFAULT_TIMEOUT_MS, baseUrl = getApiBaseUrl() } = options

  if (!path.startsWith('/')) {
    throw new Error(`API paths must start with "/": received "${path}"`)
  }
  const url = `${trimTrailingSlashes(baseUrl)}${path}`

  const controller = new AbortController()
  let timedOut = false
  const timer = setTimeout(() => {
    timedOut = true
    controller.abort()
  }, timeoutMs)
  const abortFromCaller = () => controller.abort()
  if (signal?.aborted) {
    controller.abort()
  }
  signal?.addEventListener('abort', abortFromCaller, { once: true })

  const headers = { Accept: 'application/json, application/problem+json' }
  if (body !== undefined) {
    headers['Content-Type'] = 'application/json'
  }

  let response
  try {
    response = await fetch(url, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      signal: controller.signal,
    })
  } catch (error) {
    if (signal?.aborted && !timedOut) {
      throw error // Caller cancelled (e.g. component unmounted); let them ignore it.
    }
    throw new ApiError({
      status: 0,
      message: timedOut ? TIMEOUT_MESSAGE : NETWORK_ERROR_MESSAGE,
      cause: error,
    })
  } finally {
    clearTimeout(timer)
    signal?.removeEventListener('abort', abortFromCaller)
  }

  const payload = await readJson(response)

  if (!response.ok) {
    const problem = isProblemDetails(payload) ? payload : null
    throw new ApiError({
      status: response.status,
      message: problem?.title ?? GENERIC_ERROR_MESSAGE,
      problem,
    })
  }

  return payload
}

async function readJson(response) {
  const contentType = response.headers.get('Content-Type') ?? ''
  if (response.status === 204 || !contentType.includes('json')) {
    return null
  }
  try {
    return await response.json()
  } catch {
    return null
  }
}

function isProblemDetails(payload) {
  return payload !== null && typeof payload === 'object' && typeof payload.status === 'number'
}

export const apiClient = {
  get: (path, options) => request(path, { ...options, method: 'GET' }),
  post: (path, body, options) => request(path, { ...options, method: 'POST', body }),
  put: (path, body, options) => request(path, { ...options, method: 'PUT', body }),
  patch: (path, body, options) => request(path, { ...options, method: 'PATCH', body }),
  delete: (path, options) => request(path, { ...options, method: 'DELETE' }),
}
