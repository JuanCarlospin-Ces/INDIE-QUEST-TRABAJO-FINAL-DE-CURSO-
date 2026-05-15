import { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getPostById, getUserById } from '../api/client.js';
import PostCard from '../components/PostCard.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import { pickField } from '../utils/format.js';

export default function PostDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [post, setPost] = useState(null);
  const [author, setAuthor] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const p = await getPostById(id);
        setPost(p);
        const userId = pickField(p || {}, 'postUserId', 'PostUserId');
        if (userId) {
          try {
            setAuthor(await getUserById(userId));
          } catch {
            setAuthor(null);
          }
        }
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  if (loading) return <Spinner />;
  if (error) return <ErrorBox error={error} />;
  if (!post) return <div className="empty">Post not found</div>;

  return (
    <section>
      <div className="page-header">
        <h1>Post</h1>
        <Link to="/feed" className="btn">← Back to feed</Link>
      </div>
      <PostCard
        post={post}
        author={author}
        onDeleted={() => navigate('/feed')}
      />
    </section>
  );
}
