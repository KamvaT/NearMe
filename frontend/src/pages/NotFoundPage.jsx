import { Link } from 'react-router'

export function NotFoundPage() {
  return (
    <section aria-labelledby="not-found-heading" className="space-y-4">
      <h1 id="not-found-heading" className="text-2xl font-bold">
        Page not found
      </h1>
      <p className="text-slate-700">The page you are looking for does not exist or has moved.</p>
      <Link to="/" className="font-medium text-brand-700 underline underline-offset-4">
        Back to home
      </Link>
    </section>
  )
}
