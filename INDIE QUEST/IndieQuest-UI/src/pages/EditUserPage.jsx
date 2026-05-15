import { useEffect, useState } from 'react';
import { useNavigate, useParams, Link, Navigate } from 'react-router-dom';
import { getUserById, updateUser } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import Spinner from '../components/Spinner.jsx';
import { pickField } from '../utils/format.js';
import { useAuth } from '../context/AuthContext.jsx';

export default function EditUserPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user: currentUser } = useAuth();

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);
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
          <input
            type="password"
            value={form.password}
            onChange={update('password')}
            maxLength={100}
            required
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
