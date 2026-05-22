import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link, Navigate } from 'react-router-dom';
import { getUserById, updateUser, deleteUser } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import Spinner from '../components/Spinner.jsx';
import { pickField } from '../utils/format.js';
import { useAuth } from '../context/AuthContext.jsx';

export default function EditUserPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user: currentUser, logout } = useAuth();

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [error, setError] = useState(null);
  const [showPassword, setShowPassword] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [originalUser, setOriginalUser] = useState(null);
  const [form, setForm] = useState({
    username: '',
    email: '',
    userBio: '',
    password: '',
  });

  // Only allow editing own profile
  if (currentUser && String(currentUser.userId) !== String(id)) {
    return <Navigate to={`/users/${id}`} replace />;
  }

  useEffect(() => {
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const u = await getUserById(id);
        if (!u) throw new Error('User not found');
        setOriginalUser(u);
        setForm({
          username: pickField(u, 'username', 'Username') || '',
          email: pickField(u, 'email', 'Email') || '',
          userBio: pickField(u, 'userBio', 'UserBio') || '',
          password: pickField(u, 'password', 'Password') || '',
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
      await updateUser(id, {
        username: form.username,
        email: form.email,
        userBio: form.userBio || null,
        password: form.password,
        userProfilePicture: pickField(originalUser, 'userProfilePicture', 'UserProfilePicture') || null,
        availableForWork: pickField(originalUser, 'availableForWork', 'AvailableForWork') || false,
      });
      navigate(`/users/${id}`);
    } catch (err) {
      setError(err);
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteAccount = async () => {
    setError(null);
    setDeleting(true);
    try {
      await deleteUser(id);
      logout();
      navigate('/login');
    } catch (err) {
      setError(err);
      setDeleting(false);
    }
  };

  if (loading) return <Spinner />;

  return (
    <section>
      <PageHeader
        title="Edit Profile"
        subtitle="Update your profile information"
        right={
          <Link to={`/users/${id}`} className="btn">
            Cancel
          </Link>
        }
      />
      <ErrorBox error={error} />
      <form onSubmit={handleSubmit} className="form">
        <label>
          <span>Username</span>
          <input
            type="text"
            value={form.username}
            onChange={update('username')}
            maxLength={50}
            required
          />
        </label>

        <label>
          <span>Email</span>
          <input
            type="email"
            value={form.email}
            onChange={update('email')}
            required
          />
        </label>

        <label>
          <span>Bio</span>
          <textarea
            value={form.userBio}
            onChange={update('userBio')}
            rows={4}
            placeholder="Tell us about yourself..."
            maxLength={500}
          />
        </label>

        <label>
          <span>Password</span>
          <div className="input-password-wrapper">
            <input
              type={showPassword ? 'text' : 'password'}
              value={form.password}
              onChange={update('password')}
              maxLength={100}
              required
            />
            <button
              type="button"
              className="btn-toggle-password"
              onClick={() => setShowPassword(!showPassword)}
              title={showPassword ? 'Hide password' : 'Show password'}
            >
              {showPassword ? (
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/><path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/><line x1="1" y1="1" x2="23" y2="23"/>
                </svg>
              ) : (
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/>
                </svg>
              )}
            </button>
          </div>
        </label>

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={submitting}>
            {submitting ? 'Saving...' : 'Save changes'}
          </button>
        </div>
      </form>

      {/* Danger Zone */}
         <h3>Delete account</h3>
        <p>Once you delete your account, there is no going back. Please be certain.</p>
        <button 
          type="button" 
          className="btn btn-danger" 
          onClick={() => setShowDeleteConfirm(true)}
          disabled={deleting}
        >
          Delete Account
        </button>
      </section>

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm && (
        <div className="modal-overlay" onClick={() => setShowDeleteConfirm(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Delete Account?</h2>
            <p>
              This will permanently delete your account, all your posts, and all associated media content. 
              This action cannot be undone.
            </p>
            <div className="modal-actions">
              <button 
                className="btn" 
                onClick={() => setShowDeleteConfirm(false)}
                disabled={deleting}
              >
                Cancel
              </button>
              <button 
                className="btn btn-danger" 
                onClick={handleDeleteAccount}
                disabled={deleting}
              >
                {deleting ? 'Deleting...' : 'Yes, delete my account'}
              </button>
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
