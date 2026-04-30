import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createUser } from '../api/client.js';
import PageHeader from '../components/PageHeader.jsx';
import ErrorBox from '../components/ErrorBox.jsx';

function randomId() {
  return (
    Date.now().toString(36) + Math.random().toString(36).slice(2, 8)
  ).toUpperCase();
}

export default function RegisterUserPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    username: '',
    email: '',
    password: '',
    userBio: '',
    availableForWork: false,
  });
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
      await createUser({
        userId: randomId(),
        username: form.username,
        email: form.email,
        password: form.password,
        userBio: form.userBio || null,
        availableForWork: form.availableForWork,
        userProfilePicture: null,
      });
      navigate('/users');
    } catch (err) {
      setError(err);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <section>
      <PageHeader title="Register" subtitle="Create a new account" />
      <ErrorBox error={error} />
      <form onSubmit={handleSubmit} className="form">
        <label>
          <span>Username</span>
          <input
            type="text"
            value={form.username}
            onChange={update('username')}
            required
            maxLength={40}
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
          <span>Password</span>
          <input
            type="password"
            value={form.password}
            onChange={update('password')}
            required
            minLength={4}
          />
        </label>
        <label>
          <span>Bio</span>
          <textarea
            rows={3}
            value={form.userBio}
            onChange={update('userBio')}
            placeholder="Tell us about yourself..."
          />
        </label>
        <label className="check">
          <input
            type="checkbox"
            checked={form.availableForWork}
            onChange={update('availableForWork')}
          />
          <span>Available for work</span>
        </label>

        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={submitting}>
            {submitting ? 'Registering...' : 'Register'}
          </button>
        </div>
      </form>
    </section>
  );
}
