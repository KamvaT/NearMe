import { ApiStatus } from '../components/common/ApiStatus.jsx'

export function HomePage() {
  return (
    <section aria-labelledby="home-heading" className="space-y-6">
      <h1 id="home-heading" className="text-3xl font-bold tracking-tight sm:text-4xl">
        Local services. Right where you are.
      </h1>
      <p className="max-w-2xl text-lg text-slate-700">
        NearMe helps students discover affordable services offered by student entrepreneurs and small
        businesses in their own community.
      </p>
      <ApiStatus />
    </section>
  )
}
