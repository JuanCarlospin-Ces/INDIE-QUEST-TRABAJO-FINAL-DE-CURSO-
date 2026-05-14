import { useEffect, useState, useRef, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { getUsersPaged, deleteUser } from '../api/client.js';
import Avatar from '../components/Avatar.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import PageHeader from '../components/PageHeader.jsx';
import { pickField } from '../utils/format.js';

const PAGE_SIZE = 10;

export default function UsersPage() {
  const [users, setUsers] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState(null);
  const sentinelRef = useRef(null);

  // Initial load: first page
  useEffect(() => {
    (async () => {
      try {
        const paged = await getUsersPaged(1, PAGE_SIZE);
        setUsers(paged?.data ?? []);
        setTotalPages(paged?.totalPages ?? 1);
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  // Fetch the next page and append
  const loadMore = useCallback(async () => {
    if (loadingMore || page >= totalPages) return;
    const nextPage = page + 1;
    setLoadingMore(true);
    try {
      const paged = await getUsersPaged(nextPage, PAGE_SIZE);
      setUsers((prev) => [...prev, ...(paged?.data ?? [])]);
      setTotalPages(paged?.totalPages ?? totalPages);
      setPage(nextPage);
    } catch (e) {
      setError(e);
    } finally {
      setLoadingMore(false);
    }
  }, [loadingMore, page, totalPages]);

  // IntersectionObserver: loads next page 300px before hitting the bottom
  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel) return;
    const observer = new IntersectionObserver(
      ([entry]) => { if (entry.isIntersecting) loadMore(); },
      { rootMargin: '300px', threshold: 0 }
    );
    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [loadMore]);

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

  const hasMore = page < totalPages;

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

      {/* Sentinel: triggers next page load before reaching the bottom */}
      <div ref={sentinelRef} style={{ height: 1 }} />

      {loadingMore && <Spinner />}

      {!loading && !loadingMore && !hasMore && users.length > 0 && (
        <p className="muted" style={{ textAlign: 'center', padding: '1.5rem 0' }}>
          You&apos;ve seen all users
        </p>
      )}
    </section>
  );
}
