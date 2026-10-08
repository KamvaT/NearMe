import { render, screen } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { ApiError } from '../../services/api/ApiError.js'
import { getHealth } from '../../services/api/healthApi.js'
import { ApiStatus } from './ApiStatus.jsx'

vi.mock('../../services/api/healthApi.js', () => ({ getHealth: vi.fn() }))

describe('ApiStatus', () => {
  it('announces that the API is reachable when the health check succeeds', async () => {
    vi.mocked(getHealth).mockResolvedValue({ status: 'healthy' })

    render(<ApiStatus />)

    expect(screen.getByRole('status')).toHaveTextContent(/checking/i)
    expect(await screen.findByText('Connected to NearMe API')).toBeInTheDocument()
  })

  it('announces that the API is unavailable when the health check fails', async () => {
    vi.mocked(getHealth).mockRejectedValue(new ApiError({ status: 0, message: 'offline' }))

    render(<ApiStatus />)

    expect(await screen.findByText('NearMe API unavailable')).toBeInTheDocument()
  })
})
