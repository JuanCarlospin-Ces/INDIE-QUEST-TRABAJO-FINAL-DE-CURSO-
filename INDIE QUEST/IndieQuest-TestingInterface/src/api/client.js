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
// getAllPosts fetches all items (used by Search, Compose dropdowns, etc.)
export const getAllPosts = () =>
  request('/Post?pageNumber=1&pageSize=999').then((r) => r?.data ?? []);
// getPostsPaged returns the full PagedResult { data, pageNumber, pageSize, totalCount, totalPages }
export const getPostsPaged = (pageNumber = 1, pageSize = 10) =>
  request(`/Post?pageNumber=${pageNumber}&pageSize=${pageSize}`);
export const getPostById = (id) => request(`/Post/${id}`);
export const getPostsByUserId = (userId) => request(`/Post/user/${userId}`);
export const createPost = (post) =>
  request('/Post', { method: 'POST', body: JSON.stringify(post) });
export const uploadPostMedia = (id, file) => {
  const form = new FormData();
  form.append('file', file);
  return request(`/Post/${id}/media`, { method: 'POST', body: form, headers: {} });
};
export const updatePost = (id, post) =>
  request(`/Post/${id}`, { method: 'PUT', body: JSON.stringify(post) });
export const deletePost = (id) =>
  request(`/Post/${id}`, { method: 'DELETE' });

// Users
// getAllUsers fetches all items (used by Search, author lookups, etc.)
export const getAllUsers = () =>
  request('/User?pageNumber=1&pageSize=999').then((r) => r?.data ?? []);
// getUsersPaged returns the full PagedResult { data, pageNumber, pageSize, totalCount, totalPages }
export const getUsersPaged = (pageNumber = 1, pageSize = 10) =>
  request(`/User?pageNumber=${pageNumber}&pageSize=${pageSize}`);
export const getUserById = (id) => request(`/User/${id}`);
export const createUser = (user) =>
  request('/User', { method: 'POST', body: JSON.stringify(user) });
// headers: {} anula Content-Type para que el browser lo ponga con boundary multipart
export const uploadProfilePicture = (id, file) => {
  const form = new FormData();
  form.append('file', file);
  return request(`/User/${id}/picture`, { method: 'POST', body: form, headers: {} });
};
export const updateUser = (id, user) =>
  request(`/User/${id}`, { method: 'PUT', body: JSON.stringify(user) });
export const deleteUser = (id) =>
  request(`/User/${id}`, { method: 'DELETE' });

export const API_BASE_URL = BASE_URL;
