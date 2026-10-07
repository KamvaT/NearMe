import { useApiHealth } from '../../hooks/useApiHealth.js'

const STATES = {
  checking: { label: 'Checking connection…', dot: 'bg-slate-400' },
  online: { label: 'Connected to NearMe API', dot: 'bg-emerald-600' },
  offline: { label: 'NearMe API unavailable', dot: 'bg-red-600' },
}

/** Foundation-phase indicator proving the frontend can reach the API. */
export function ApiStatus() {
  const state = useApiHealth()
  const { label, dot } = STATES[state]

  return (
    <p role="status" className="inline-flex items-center gap-2 text-sm text-slate-700">
      <span aria-hidden="true" className={`h-2.5 w-2.5 rounded-full ${dot}`} />
      {label}
    </p>
  )
}
