import { useEffect, useState } from 'react'
import { getHealth } from '../services/api/healthApi.js'

/** @typedef {'checking' | 'online' | 'offline'} ApiHealthState */

/** Checks once, on mount, whether the API is reachable. */
export function useApiHealth() {
  const [state, setState] = useState(/** @type {ApiHealthState} */ ('checking'))

  useEffect(() => {
    const controller = new AbortController()

    getHealth({ signal: controller.signal })
      .then((result) => {
        if (!controller.signal.aborted) {
          setState(result?.status === 'healthy' ? 'online' : 'offline')
        }
      })
      .catch(() => {
        if (!controller.signal.aborted) {
          setState('offline')
        }
      })

    return () => controller.abort()
  }, [])

  return state
}
