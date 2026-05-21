import { NavLink, Outlet, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';
import Avatar from './Avatar.jsx';

const IconHome = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/>
  </svg>
);

const IconSearch = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
  </svg>
);

const IconUsers = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/>
  </svg>
);

const IconSettings = () => (
  <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <circle cx="12" cy="12" r="3"/><path d="M12 1v6m0 6v6M4.22 4.22l4.24 4.24m6.08 0l4.24-4.24M1 12h6m6 0h6m-1.78 7.78l-4.24-4.24m-6.08 0l-4.24 4.24"/>
  </svg>
);

const navItems = [
  { to: '/feed', label: 'Feed', icon: <IconHome /> },
  { to: '/search', label: 'Search', icon: <IconSearch /> },
  { to: '/users', label: 'Users', icon: <IconUsers /> },
];

export default function Layout() {
  const { user } = useAuth();

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand-block">
          <Link to="/feed" className="brand">
            <img src="/logo.png" alt="IndieQuest" className="brand-logo-img" />
            <span className="brand-name">IndieQuest</span>
          </Link>
          <div className="admin-mode-badge">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 1l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 1z"/>
            </svg>
            <span>ADMIN MODE</span>
          </div>
          <span className="brand-tagline">Administration Panel</span>
        </div>
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
          <IconSettings />
          Create Content
        </Link>

        {user && (
          <div className="sidebar-user">
            <div className="sidebar-user-info">
              <Avatar username={user.displayName} />
              <div>
                <div className="sidebar-username">{user.displayName}</div>
                <div className="sidebar-role">Administrator</div>
              </div>
            </div>
          </div>
        )}
      </aside>

      <main className="main">
        <Outlet />
      </main>
    </div>
  );
}
