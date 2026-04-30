import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getUserById, getPostsByUserId } from '../api/client.js';
import Avatar from '../components/Avatar.jsx';
import PostCard from '../components/PostCard.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import { pickField } from '../utils/format.js';

export default function UserProfilePage() {
  const { id } = useParams();
  const [user, setUser] = useState(null);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const [u, p] = await Promise.all([
          getUserById(id),
          getPostsByUserId(id),
        ]);
        setUser(u);
        setPosts(Array.isArray(p) ? p : []);
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  if (loading) return <Spinner />;
  if (error) return <ErrorBox error={error} />;
  if (!user) return <div className="empty">User not found</div>;

  const username = pickField(user, 'username', 'Username');
  const bio = pickField(user, 'userBio', 'UserBio');
  const email = pickField(user, 'email', 'Email');
  const available = pickField(user, 'availableForWork', 'AvailableForWork');

  return (
    <section>
      <div className="profile-header">
        <Avatar username={username} size={88} />
        <div>
          <h1 className="profile-name">@{username}</h1>
          {available && <span className="badge">Available for work</span>}
          {bio && <p>{bio}</p>}
          {email && <p className="muted">{email}</p>}
          <div className="profile-actions">
            <Link to="/users" className="btn">← Back to users</Link>
          </div>
        </div>
      </div>

      <h2 className="section-title">Posts</h2>
      {posts.length === 0 && (
        <div className="empty">This user has no posts yet.</div>
      )}
      <div className="feed">
        {posts.map((post) => (
          <PostCard
            key={pickField(post, 'postId', 'PostId')}
            post={post}
            author={user}
            onDeleted={(pid) =>
              setPosts((prev) =>
                prev.filter(
                  (p) => String(pickField(p, 'postId', 'PostId')) !== String(pid)
                )
              )
            }
          />
        ))}
      </div>
    </section>
  );
}
