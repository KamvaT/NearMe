import { afterEach, describe, expect, it, vi } from 'vitest'
import { ApiError } from './ApiError.js'
import { request } from './apiClient.js'

const BASE_URL = 'http://api.test'

function jsonResponse(body, { status = 200, contentType = 'application/json' } = {}) {
  return new Response(JSON.stringify(body), { status, headers: { 'Content-Type': contentType } })
}

function mockFetch(implementation) {
  const fetchMock = vi.fn(implementation)
  vi.stubGlobal('fetch', fetchMock)
  return fetchMock
}

afterEach(() => {
  vi.useRealTimers()
})

describe('apiClient.request', () => {
  it('calls the configured base URL and returns parsed JSON', async () => {
    const fetchMock = mockFetch(async () => jsonResponse({ status: 'healthy' }))

    const result = await request('/health', { baseUrl: `${BASE_URL}/` })

    expect(result).toEqual({ status: 'healthy' })
    expect(fetchMock).toHaveBeenCalledWith(
      'http://api.test/health',
      expect.objectContaining({ method: 'GET' }),
    )
  })

  it('reads the base URL from VITE_API_BASE_URL', async () => {
    vi.stubEnv('VITE_API_BASE_URL', 'http://configured.test')
    const fetchMock = mockFetch(async () => jsonResponse({}))

    await request('/health')

    expect(fetchMock.mock.calls[0][0]).toBe('http://configured.test/health')
  })

  it('serialises a JSON body and sets the content type', async () => {
    const fetchMock = mockFetch(async () => jsonResponse({ id: 1 }, { status: 201 }))

    await request('/api/v1/example', { baseUrl: BASE_URL, method: 'POST', body: { name: 'Cuts' } })

    const [, init] = fetchMock.mock.calls[0]
    expect(init.body).toBe('{"name":"Cuts"}')
    expect(init.headers['Content-Type']).toBe('application/json')
  })

  it('throws ApiError carrying Problem Details for error responses', async () => {
    const problem = {
      title: 'Validation failed',
      status: 400,
      errors: { businessName: ['Business name is required.'] },
      traceId: '00-abc',
    }
    mockFetch(async () => jsonResponse(problem, { status: 400, contentType: 'application/problem+json' }))

    const error = await request('/api/v1/example', { baseUrl: BASE_URL }).catch((e) => e)

    expect(error).toBeInstanceOf(ApiError)
    expect(error.status).toBe(400)
    expect(error.message).toBe('Validation failed')
    expect(error.fieldErrors).toEqual({ businessName: ['Business name is required.'] })
  })

  it('uses a generic message when the error body is not Problem Details', async () => {
    mockFetch(async () => new Response('<html>Internal stack trace…</html>', { status: 502 }))

    const error = await request('/health', { baseUrl: BASE_URL }).catch((e) => e)

    expect(error.status).toBe(502)
    expect(error.message).toBe('Something went wrong. Please try again.')
    expect(error.message).not.toContain('stack trace')
  })

  it('maps network failures to a status-0 ApiError without exposing the raw error', async () => {
    mockFetch(async () => {
      throw new TypeError('Failed to fetch: ECONNREFUSED 10.0.0.5:5080')
    })

    const error = await request('/health', { baseUrl: BASE_URL }).catch((e) => e)

    expect(error).toBeInstanceOf(ApiError)
    expect(error.isNetworkError).toBe(true)
    expect(error.message).not.toContain('10.0.0.5')
  })

  it('aborts and reports a timeout when the API does not respond in time', async () => {
    vi.useFakeTimers()
    mockFetch(
      (_url, init) =>
        new Promise((_resolve, reject) => {
          init.signal.addEventListener('abort', () => reject(new DOMException('Aborted', 'AbortError')))
        }),
    )

    const pending = request('/health', { baseUrl: BASE_URL, timeoutMs: 50 }).catch((e) => e)
    await vi.advanceTimersByTimeAsync(50)
    const error = await pending

    expect(error).toBeInstanceOf(ApiError)
    expect(error.status).toBe(0)
    expect(error.message).toMatch(/taking too long/i)
  })

  it('rejects relative paths to avoid accidental requests to the wrong origin', async () => {
    await expect(request('health', { baseUrl: BASE_URL })).rejects.toThrow(/must start with/)
  })
})
