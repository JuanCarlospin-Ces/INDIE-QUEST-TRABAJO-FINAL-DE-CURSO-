import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { createPost, uploadPostMedia, deletePost, getAllUsers } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import FileDropzone from '../components/FileDropzone.jsx';
import { useAuth } from '../context/AuthContext.jsx';
import { pickField } from '../utils/format.js';

export default function ComposePostPage() {
  const navigate = useNavigate();
  const { user: currentUser } = useAuth();
  const [form, setForm] = useState({
    title: '',
    description: '',
    mediaContent: '',
    tags: '',
  });
  const [selectedUserId, setSelectedUserId] = useState(currentUser?.id ?? 0);
  const [users, setUsers] = useState([]);
  const [loadingUsers, setLoadingUsers] = useState(true);
  const [mediaFile, setMediaFile] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  // Load all users for selection (admin feature)
  useEffect(() => {
    (async () => {
      try {
        setLoadingUsers(true);
        const allUsers = await getAllUsers();
        setUsers(Array.isArray(allUsers) ? allUsers : []);
        // Set default to current user if available
        if (Array.isArray(allUsers) && allUsers.length > 0) {
          setSelectedUserId(pickField(allUsers[0], 'userId', 'UserId'));
        }
      } catch (err) {
        console.error('Failed to load users:', err);
      } finally {
        setLoadingUsers(false);
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
      const tagNames = form.tags
        .split(',')
        .map((t) => t.trim())
        .filter(Boolean);

      const { postId } = await createPost({
        userId:       selectedUserId,
        title:        form.title,
        description:  form.description || null,
        mediaContent: mediaFile ? 'uploading...' : form.mediaContent || '',
        tagNames:     tagNames.length > 0 ? tagNames : null,
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
      <PageHeader title="Create Content (Admin)" subtitle="Create a post as any user" />
      <ErrorBox error={error} />
      <form onSubmit={handleSubmit} className="form">
        <label>
          <span>Create as User</span>
          <select
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(Number(e.target.value))}
            disabled={loadingUsers}
            required
          >
            {loadingUsers ? (
              <option>Loading users...</option>
            ) : users.length === 0 ? (
              <option>No users available</option>
            ) : (
              users.map((u) => {
                const id = pickField(u, 'userId', 'UserId');
                const username = pickField(u, 'username', 'Username');
                return (
                  <option key={id} value={id}>
                    {username}
                  </option>
                );
              })
            )}
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
          <button type="submit" className="btn btn-primary" disabled={submitting || loadingUsers}>
            {submitting ? 'Posting...' : 'Post'}
          </button>
        </div>
      </form>
    </section>
  );
}
