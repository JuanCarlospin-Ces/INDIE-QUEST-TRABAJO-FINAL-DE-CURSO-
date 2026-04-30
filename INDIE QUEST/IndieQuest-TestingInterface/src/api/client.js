const BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:5063/api';

async function request(path, options = {}) {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });

  if (!res.ok) {
    const text = await res.text().catch(() => '');
    throw new Error(`API ${res.status}: ${text || res.statusText}`);
  }

  if (res.status === 204) return null;
  const contentType = res.headers.get('content-type') || '';
  if (!contentType.includes('application/json')) return null;
  return res.json();
}

// Posts
export const getAllPosts = () => request('/Post');
export const getPostById = (id) => request(`/Post/${id}`);
export const getPostsByUserId = (userId) => request(`/Post/user/${userId}`);
export const createPost = (post) =>
  request('/Post', { method: 'POST', body: JSON.stringify(post) });
export const updatePost = (id, post) =>
  request(`/Post/${id}`, { method: 'PUT', body: JSON.stringify(post) });
export const deletePost = (id) =>
  request(`/Post/${id}`, { method: 'DELETE' });

// Users
export const getAllUsers = () => request('/User');
export const getUserById = (id) => request(`/User/${id}`);
export const createUser = (user) =>
  request('/User', { method: 'POST', body: JSON.stringify(user) });
export const updateUser = (id, user) =>
  request(`/User/${id}`, { method: 'PUT', body: JSON.stringify(user) });
export const deleteUser = (id) =>
  request(`/User/${id}`, { method: 'DELETE' });

export const API_BASE_URL = BASE_URL;
