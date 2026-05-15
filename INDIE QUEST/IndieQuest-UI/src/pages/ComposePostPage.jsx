import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createPost, uploadPostMedia, deletePost } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import FileDropzone from '../components/FileDropzone.jsx';
import { useAuth } from '../context/AuthContext.jsx';

export default function ComposePostPage() {
  const navigate = useNavigate();
  const { user: currentUser } = useAuth();
  const [form, setForm] = useState({
    title: '',
    description: '',
    mediaContent: '',
    tags: '',
  });
  const [mediaFile, setMediaFile] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

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
        userId:       currentUser.userId,
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
      <PageHeader title="New Post" subtitle="Share something with the community" />
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
