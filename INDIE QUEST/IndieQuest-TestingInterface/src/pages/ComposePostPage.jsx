import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createPost, getAllUsers } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import FileDropzone from '../components/FileDropzone.jsx';
import { pickField } from '../utils/format.js';

function randomId() {
  return (
    Date.now().toString(36) + Math.random().toString(36).slice(2, 8)
  ).toUpperCase();
}

export default function ComposePostPage() {
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [form, setForm] = useState({
    postUserId: '',
    title: '',
    description: '',
    mediaContent: '',
    tags: '',
  });
  const [mediaFile, setMediaFile] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      try {
        const u = await getAllUsers();
        const arr = Array.isArray(u) ? u : [];
        setUsers(arr);
        if (arr.length > 0) {
          setForm((f) => ({
            ...f,
            postUserId: String(pickField(arr[0], 'userId', 'UserId') || ''),
          }));
        }
      } catch (e) {
        setError(e);
      }
    })();
  }, []);

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

      await createPost({
        postId: randomId(),
        postUserId: form.postUserId,
        title: form.title,
        description: form.description || null,
        mediaContent: mediaFile ? `file://${mediaFile.name}` : '',
        tags: tags.length > 0 ? tags : null,
      });
      navigate('/feed');
    } catch (err) {
      setError(err);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <section>
      <PageHeader title="New Post" subtitle="Share something with the community" />
      <ErrorBox error={error} />
      <form onSubmit={handleSubmit} className="form">
        <label>
          <span>Author</span>
          <select value={form.postUserId} onChange={update('postUserId')} required>
            <option value="" disabled>
              Select a user...
            </option>
            {users.map((u) => {
              const id = pickField(u, 'userId', 'UserId');
              const name = pickField(u, 'username', 'Username');
              return (
                <option key={id} value={id}>
                  @{name}
                </option>
              );
            })}
          </select>
        </label>

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
          <span>Media (file)</span>
          <FileDropzone value={mediaFile} onChange={setMediaFile} />
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
            {submitting ? 'Posting...' : 'Post'}
          </button>
        </div>
      </form>
    </section>
  );
}
