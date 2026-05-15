import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getUserById, getPostsByUserId, updateUser, uploadProfilePicture } from '../api/client.js';
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
  const [toggling, setToggling] = useState(false);
  const [uploadingPic, setUploadingPic] = useState(false);
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

  const handlePictureUpload = async (e) => {
    const file = e.target.files?.[0];
    if (!file || !user) return;
    setUploadingPic(true);
    try {
      const userId = pickField(user, 'userId', 'UserId');
      await uploadProfilePicture(userId, file);
      // Reload user data from server to reflect the new picture
      const updatedUser = await getUserById(userId);
      setUser(updatedUser);
    } catch (e) {
      alert(e.message);
    } finally {
      setUploadingPic(false);
    }
  };

  const handleToggleAvailability = async () => {
    if (!user || toggling) return;
    setToggling(true);
    try {
      const currentAvailable = pickField(user, 'availableForWork', 'AvailableForWork');
      await updateUser(pickField(user, 'userId', 'UserId'), {
        username:           pickField(user, 'username', 'Username'),
        password:           pickField(user, 'password', 'Password'),
        email:              pickField(user, 'email', 'Email'),
        userBio:            pickField(user, 'userBio', 'UserBio') ?? null,
        userProfilePicture: pickField(user, 'userProfilePicture', 'UserProfilePicture') ?? null,
        availableForWork:   !currentAvailable,
      });
      setUser((prev) => ({ ...prev, availableForWork: !currentAvailable }));
    } catch (e) {
      alert(e.message);
    } finally {
      setToggling(false);
    }
  };

  if (loading) return <Spinner />;
  if (error) return <ErrorBox error={error} />;
  if (!user) return <div className="empty">User not found</div>;

  const username = pickField(user, 'username', 'Username');
  const bio = pickField(user, 'userBio', 'UserBio');
  const email = pickField(user, 'email', 'Email');
  const available = pickField(user, 'availableForWork', 'AvailableForWork');
  const profilePicture = pickField(user, 'userProfilePicture', 'UserProfilePicture');

  return (
    <section>
      <div className="profile-header">
        <Avatar username={username} size={88} profilePicture={profilePicture} />
        <div>
          <h1 className="profile-name">@{username}</h1>
          {available && <span className="badge">Available for work</span>}
          {bio && <p>{bio}</p>}
          {email && <p className="muted">{email}</p>}
          <div className="profile-actions">
            <Link to="/users" className="btn">← Back to users</Link>
            <button
              className={`btn ${available ? 'btn-danger' : 'btn-primary'}`}
              onClick={handleToggleAvailability}
              disabled={toggling}
            >
              {toggling
                ? 'Updating...'
                : available
                  ? 'Set unavailable'
                  : 'Set available for work'}
            </button>
            <label className="btn" style={{ cursor: 'pointer' }}>
              {uploadingPic ? 'Uploading...' : '📷 Change photo'}
              <input
                type="file"
                accept="image/*"
                style={{ display: 'none' }}
                onChange={handlePictureUpload}
                disabled={uploadingPic}
              />
            </label>
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
