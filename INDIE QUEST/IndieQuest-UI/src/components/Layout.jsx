import { NavLink, Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext.jsx';

const navItems = [
  { to: '/feed', label: 'Home', icon: '🏠' },
  { to: '/search', label: 'Search', icon: '🔍' },
  { to: '/users', label: 'Users', icon: '👥' },
];

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <Link to="/feed" className="brand">
          <img src="/logo.png" alt="IndieQuest" className="brand-logo-img" />
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

        {user && (
          <div className="sidebar-user">
            <Link to={`/users/${user.userId}`} className="sidebar-user-info">
              <span className="sidebar-username">{user.username}</span>
            </Link>
            <button onClick={handleLogout} className="btn-logout" title="Logout">
              ⏻
            </button>
          </div>
        )}
      </aside>

      <main className="main">
        <Outlet />
      </main>

      <aside className="rightbar">
        <div className="card">
          <h3>About</h3>
          <p>
            IndieQuest – Share your indie game development journey.
          </p>
        </div>
        <div className="card">
          <h3>Tips</h3>
          <ul>
            <li>Make sure the API is running on port 5063.</li>
            <li>Use <code>.env</code> to override the API URL.</li>
          </ul>
        </div>
      </aside>
    </div>
  );
}
