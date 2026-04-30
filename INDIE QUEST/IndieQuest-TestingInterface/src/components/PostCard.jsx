import { Link, useNavigate } from 'react-router-dom';
import Avatar from './Avatar.jsx';
import MediaPreview from './MediaPreview.jsx';
import { formatDate, pickField } from '../utils/format.js';
import { deletePost } from '../api/client.js';

export default function PostCard({ post, author, onDeleted }) {
  const navigate = useNavigate();

  const postId = pickField(post, 'postId', 'PostId');
  const userId = pickField(post, 'postUserId', 'PostUserId');
  const title = pickField(post, 'title', 'Title');
  const description = pickField(post, 'description', 'Description');
  const media = pickField(post, 'mediaContent', 'MediaContent');
  const date = pickField(post, 'creationDate', 'CreationDate');
  const tags = pickField(post, 'tags', 'Tags') || [];

  const username =
    pickField(author || {}, 'username', 'Username') || `user-${userId || ''}`;

  const handleDelete = async (e) => {
    e.stopPropagation();
    if (!postId) return;
    if (!confirm('Delete this post?')) return;
    try {
      await deletePost(postId);
      onDeleted && onDeleted(postId);
    } catch (err) {
      alert(err.message);
    }
  };

  const goPost = () => postId && navigate(`/posts/${postId}`);

  const handleEdit = (e) => {
    e.stopPropagation();
    if (postId) navigate(`/posts/${postId}/edit`);
  };

  const goAuthor = (e) => {
    e.stopPropagation();
    if (userId) navigate(`/users/${userId}`);
  };

  return (
    <article className="post" onClick={goPost} role="link" tabIndex={0}>
      <div
        className="post-avatar post-avatar--link"
        onClick={goAuthor}
        title={`Go to @${username}`}
      >
        <Avatar username={username} />
      </div>
      <div className="post-body">
        <header className="post-header">
          <Link
            to={userId ? `/users/${userId}` : '#'}
            className="post-author"
            onClick={(e) => e.stopPropagation()}
          >
            @{username}
          </Link>
          <span className="post-dot">·</span>
          <span className="post-date">{formatDate(date)}</span>
          <div className="post-actions">
            <button
              type="button"
              className="post-action"
              onClick={handleEdit}
              title="Edit post"
            >
              ✎
            </button>
            <button
              type="button"
              className="post-action post-action--danger"
              onClick={handleDelete}
              title="Delete post"
            >
              ✕
            </button>
          </div>
        </header>
        {title && <h3 className="post-title">{title}</h3>}
        {description && <p className="post-desc">{description}</p>}
        <MediaPreview mediaContent={media} />
        {tags && tags.length > 0 && (
          <div className="tags">
            {tags.map((t, i) => (
              <span key={i} className="tag">
                #{pickField(t, 'tagName', 'TagName') || pickField(t, 'tagId', 'TagId')}
              </span>
            ))}
          </div>
        )}
      </div>
    </article>
  );
}
