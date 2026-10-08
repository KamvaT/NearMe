import { Link } from 'react-router'

export function SiteHeader() {
  return (
    <header className="border-b border-slate-200">
      <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-4">
        <Link to="/" className="text-xl font-semibold tracking-tight text-brand-700">
          NearMe
        </Link>
      </div>
    </header>
  )
}
