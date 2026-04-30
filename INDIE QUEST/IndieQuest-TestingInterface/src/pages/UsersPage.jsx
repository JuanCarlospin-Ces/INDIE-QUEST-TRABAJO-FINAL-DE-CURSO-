import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getAllUsers, deleteUser } from '../api/client.js';
import Avatar from '../components/Avatar.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import PageHeader from '../components/PageHeader.jsx';
import { pickField } from '../utils/format.js';

export default function UsersPage() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      try {
        const u = await getAllUsers();
        setUsers(Array.isArray(u) ? u : []);
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  const handleDelete = async (id) => {
    if (!confirm('Delete user?')) return;
    try {
      await deleteUser(id);
      setUsers((prev) =>
        prev.filter((u) => String(pickField(u, 'userId', 'UserId')) !== String(id))
      );
    } catch (e) {
      alert(e.message);
    }
  };

  return (
    <section>
      <PageHeader
        title="Users"
        subtitle="People in the IndieQuest community"
        right={
          <Link to="/register" className="btn btn-primary">
            + Register
          </Link>
        }
      />
      {loading && <Spinner />}
      <ErrorBox error={error} />

      <div className="users-grid">
        {users.map((u) => {
          const id = pickField(u, 'userId', 'UserId');
          const username = pickField(u, 'username', 'Username');
          const bio = pickField(u, 'userBio', 'UserBio');
          const available = pickField(u, 'availableForWork', 'AvailableForWork');
          return (
            <div key={id} className="user-card">
              <Avatar username={username} size={56} />
              <div className="user-card-body">
                <Link to={`/users/${id}`} className="user-name">
                  @{username}
                </Link>
                {available && <span className="badge">Available</span>}
                {bio && <p className="muted">{bio}</p>}
              </div>
              <button
                className="btn btn-danger btn-sm"
                onClick={() => handleDelete(id)}
              >
                Delete
              </button>
            </div>
          );
        })}
      </div>
    </section>
  );
}
