import { useEffect, useState, useMemo } from 'react';
import { Link } from 'react-router-dom';
import { getAllPosts, getAllUsers } from '../api/client.js';
import PostCard from '../components/PostCard.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import PageHeader from '../components/PageHeader.jsx';
import { pickField } from '../utils/format.js';

export default function FeedPage() {
  const [posts, setPosts] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const [p, u] = await Promise.all([getAllPosts(), getAllUsers()]);
      setPosts(Array.isArray(p) ? p : []);
      setUsers(Array.isArray(u) ? u : []);
    } catch (e) {
      setError(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const usersById = useMemo(() => {
    const map = new Map();
    for (const u of users) {
      const id = pickField(u, 'userId', 'UserId');
      if (id) map.set(String(id), u);
    }
    return map;
  }, [users]);

  const sorted = useMemo(() => {
    return [...posts].sort((a, b) => {
      const da = new Date(pickField(a, 'creationDate', 'CreationDate') || 0);
      const db = new Date(pickField(b, 'creationDate', 'CreationDate') || 0);
      return db - da;
    });
  }, [posts]);

  return (
    <section>
      <PageHeader
        title="Home"
        subtitle="Latest posts from the community"
        right={
          <Link to="/compose" className="btn btn-primary">
            + New Post
          </Link>
        }
      />

      {loading && <Spinner />}
      <ErrorBox error={error} />

      {!loading && sorted.length === 0 && !error && (
        <div className="empty">No posts yet. Be the first to post!</div>
      )}

      <div className="feed">
        {sorted.map((post) => {
          const userId = String(pickField(post, 'postUserId', 'PostUserId') || '');
          return (
            <PostCard
              key={pickField(post, 'postId', 'PostId')}
              post={post}
              author={usersById.get(userId)}
              onDeleted={(id) =>
                setPosts((prev) =>
                  prev.filter(
                    (p) => String(pickField(p, 'postId', 'PostId')) !== String(id)
                  )
                )
              }
            />
          );
        })}
      </div>
    </section>
  );
}
