import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { getPostById, updatePost } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import Spinner from '../components/Spinner.jsx';
import { pickField } from '../utils/format.js';

function randomId() {
  return (
    Date.now().toString(36) + Math.random().toString(36).slice(2, 8)
  ).toUpperCase();
}

function tagsToString(tags) {
  if (!Array.isArray(tags)) return '';
  return tags
    .map((t) => pickField(t, 'tagName', 'TagName'))
    .filter(Boolean)
    .join(', ');
}

export default function EditPostPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [form, setForm] = useState({
    title: '',
    description: '',
    mediaContent: '',
    tags: '',
  });

  useEffect(() => {
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const p = await getPostById(id);
        if (!p) throw new Error('Post not found');
        setForm({
          title: pickField(p, 'title', 'Title') || '',
          description: pickField(p, 'description', 'Description') || '',
          mediaContent: pickField(p, 'mediaContent', 'MediaContent') || '',
          tags: tagsToString(pickField(p, 'tags', 'Tags')),
        });
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  const update = (k) => (e) =>
    setForm((prev) => ({ ...prev, [k]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      const tags = form.tags
        .split(',')
        .map((t) => t.trim())
        .filter(Boolean)
        .map((name) => ({ tagId: randomId(), tagName: name }));

      await updatePost(id, {
        postId: id,
        title: form.title,
        description: form.description || null,
        mediaContent: form.mediaContent || '',
        tags: tags.length > 0 ? tags : null,
      });
      navigate(`/posts/${id}`);
    } catch (err) {
      setError(err);
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) return <Spinner />;

  return (
    <section>
      <PageHeader
        title="Edit Post"
        subtitle="Update your post content"
        right={
          <Link to={`/posts/${id}`} className="btn">
            Cancel
          </Link>
        }
      />
      <ErrorBox error={error} />
      <form onSubmit={handleSubmit} className="form">
        <label>
          <span>Title</span>
          <input
            type="text"
            value={form.title}
            onChange={update('title')}
            maxLength={120}
            required
          />
        </label>

        <label>
          <span>Description</span>
          <textarea
            value={form.description}
            onChange={update('description')}
            rows={4}
            placeholder="What's on your mind?"
          />
        </label>

        <label>
          <span>Media (URL)</span>
          <input
            type="text"
            value={form.mediaContent}
            onChange={update('mediaContent')}
            placeholder="https://..."
          />
        </label>

        <label>
          <span>Tags (comma separated)</span>
          <input
            type="text"
            value={form.tags}
            onChange={update('tags')}
            placeholder="indiedev, pixelart"
          />
        </label>

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={submitting}>
            {submitting ? 'Saving...' : 'Save changes'}
          </button>
        </div>
      </form>
    </section>
  );
}
