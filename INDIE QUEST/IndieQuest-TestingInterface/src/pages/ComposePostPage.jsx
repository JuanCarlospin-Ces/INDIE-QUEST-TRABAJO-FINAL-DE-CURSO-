import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createPost, getAllUsers, uploadPostMedia, deletePost } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import FileDropzone from '../components/FileDropzone.jsx';
import { pickField } from '../utils/format.js';

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
    let createdPostId = null;

    try {
      // Create post with placeholder media if file exists
      const { postId } = await createPost({
        userId:       parseInt(form.postUserId, 10),
        title:        form.title,
        description:  form.description || null,
        mediaContent: mediaFile ? 'uploading...' : form.mediaContent || '',
      });
      createdPostId = postId;

      // If there's a file, upload it after post creation
      if (mediaFile && postId) {
        try {
          await uploadPostMedia(postId, mediaFile);
        } catch (uploadErr) {
          // If upload fails, delete the post to avoid orphaned posts
          console.error('Upload failed, deleting post:', uploadErr);
          try {
            await deletePost(postId);
          } catch (deleteErr) {
            console.error('Failed to delete post after upload failure:', deleteErr);
          }
          throw uploadErr;
        }
      }

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

        <div className="form-field">
          <span>Media (file)</span>
          <FileDropzone value={mediaFile} onChange={setMediaFile} />
        </div>

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
