import { NavLink, Outlet, Link } from 'react-router-dom';

const navItems = [
  { to: '/feed', label: 'Home', icon: '🏠' },
  { to: '/search', label: 'Search', icon: '🔍' },
  { to: '/users', label: 'Users', icon: '👥' },
  { to: '/register', label: 'Register', icon: '＋' },
];

export default function Layout() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <Link to="/feed" className="brand">
          <span className="brand-logo">IQ</span>
          <span className="brand-name">IndieQuest</span>
        </Link>
        <nav className="nav">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                'nav-item' + (isActive ? ' nav-item--active' : '')
              }
            >
              <span className="nav-icon">{item.icon}</span>
              <span className="nav-label">{item.label}</span>
            </NavLink>
          ))}
        </nav>
        <Link to="/compose" className="compose-btn">
          New Post
        </Link>
      </aside>

      <main className="main">
        <Outlet />
      </main>

      <aside className="rightbar">
        <div className="card">
          <h3>About</h3>
          <p>
            IndieQuest testing interface. A lightweight client to exercise
            the API endpoints during development.
          </p>
        </div>
        <div className="card">
          <h3>Tips</h3>
          <ul>
            <li>Make sure the API is running on port 5063.</li>
            <li>Use <code>.env</code> to override the API URL.</li>
            <li>Media is mocked until uploads are implemented.</li>
          </ul>
        </div>
      </aside>
    </div>
  );
}
