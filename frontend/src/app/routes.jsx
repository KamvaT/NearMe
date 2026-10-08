import { PublicLayout } from '../layouts/PublicLayout.jsx'
import { HomePage } from '../pages/HomePage.jsx'
import { NotFoundPage } from '../pages/NotFoundPage.jsx'

/**
 * Route table, kept separate from the router instance so tests can mount it in a memory router.
 * Protected/role-based routes are added with authentication (Phase 17); they are a UX aid only —
 * the API remains the authorization boundary.
 */
export const routes = [
  {
    element: <PublicLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
]
