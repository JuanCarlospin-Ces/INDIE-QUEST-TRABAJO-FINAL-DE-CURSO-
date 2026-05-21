import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { createUser, uploadProfilePicture, loginUser } from '../api/client.js';
import ErrorBox from '../components/ErrorBox.jsx';
import { useAuth } from '../context/AuthContext.jsx';

export default function RegisterUserPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const [form, setForm] = useState({
    username: '',
    email: '',
    password: '',
    userBio: '',
    availableForWork: false,
  });
  const [profileFile, setProfileFile] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);

  const update = (k) => (e) => {
    const value = e.target.type === 'checkbox' ? e.target.checked : e.target.value;
    setForm((prev) => ({ ...prev, [k]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      const { userId } = await createUser({
        username:           form.username,
        email:              form.email,
        password:           form.password,
        userBio:            form.userBio || null,
        availableForWork:   form.availableForWork,
        userProfilePicture: profileFile ? profileFile.name : null,
      });
      if (profileFile && userId) {
        await uploadProfilePicture(userId, profileFile);
      }
      // Auto-login after registration
      const userData = await loginUser(form.username, form.password);
      login(userData);
      navigate('/feed');
    } catch (err) {
      setError(err);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-header">
          <img src="/logo.png" alt="IndieQuest" className="login-logo" />
          <h1>IndieQuest</h1>
          <p className="login-subtitle">Create your account</p>
        </div>

        <ErrorBox error={error} />
        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="reg-username">Username</label>
            <input
              id="reg-username"
              type="text"
              value={form.username}
              onChange={update('username')}
              required
              maxLength={40}
              placeholder="Choose a username"
              disabled={submitting}
            />
          </div>
          <div className="form-group">
            <label htmlFor="reg-email">Email</label>
            <input
              id="reg-email"
              type="email"
              value={form.email}
              onChange={update('email')}
              required
              placeholder="you@example.com"
              disabled={submitting}
            />
          </div>
          <div className="form-group">
            <label htmlFor="reg-password">Password</label>
            <input
              id="reg-password"
              type="password"
              value={form.password}
              onChange={update('password')}
              required
              minLength={4}
              placeholder="At least 4 characters"
              disabled={submitting}
            />
          </div>
          <div className="form-group">
            <label htmlFor="reg-bio">Bio</label>
            <textarea
              id="reg-bio"
              rows={3}
              value={form.userBio}
              onChange={update('userBio')}
              placeholder="Tell us about yourself..."
              disabled={submitting}
            />
          </div>
          <label className="check">
            <input
              type="checkbox"
              checked={form.availableForWork}
              onChange={update('availableForWork')}
              disabled={submitting}
            />
            <span>Available for work</span>
          </label>
          <div className="form-group">
            <label htmlFor="reg-picture">Profile picture</label>
            <input
              id="reg-picture"
              type="file"
              accept="image/*"
              onChange={(e) => setProfileFile(e.target.files?.[0] ?? null)}
              disabled={submitting}
            />
            {profileFile && (
              <span className="muted" style={{ fontSize: '0.85rem' }}>
                Selected: {profileFile.name}
              </span>
            )}
          </div>

          <button type="submit" className="btn btn-primary login-btn" disabled={submitting}>
            {submitting ? 'Creating account...' : 'Sign Up'}
          </button>
        </form>

        <div className="login-footer">
          <p>Already have an account? <Link to="/login">Sign in</Link></p>
        </div>
      </div>
    </div>
  );
}
