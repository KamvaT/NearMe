/**
 * Error raised for every failed API call, so UI code handles one shape.
 *
 * `status` is the HTTP status, or 0 when no response was received (network failure, timeout).
 * `message` is safe to show to users: it comes from the API's Problem Details `title`
 * (which the API keeps generic) or from a fixed client-side message — never from raw
 * response bodies or JavaScript exception text.
 */
export class ApiError extends Error {
  constructor({ status, message, problem = null, cause } = {}) {
    super(message, cause ? { cause } : undefined)
    this.name = 'ApiError'
    this.status = status
    this.problem = problem
  }

  get isNetworkError() {
    return this.status === 0
  }

  /** Field-level validation messages from a 400 Problem Details response, if any. */
  get fieldErrors() {
    return this.problem?.errors ?? {}
  }
}
