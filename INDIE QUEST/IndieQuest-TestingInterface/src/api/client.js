const BASE_URL =
  import.meta.env.VITE_API_BASE_URL || 'http://localhost:5063/api';

async function request(path, options = {}) {
  const { timeout = 30000, ...fetchOptions } = options; // 30s default for regular requests
  
  const controller = new AbortController();
  let timeoutId = null;

  // Only set timeout if it's a positive number
  if (timeout > 0) {
    timeoutId = setTimeout(() => {
      controller.abort();
    }, timeout);
  }

  try {
    const res = await fetch(`${BASE_URL}${path}`, {
      headers: { 'Content-Type': 'application/json' },
      ...fetchOptions,
      signal: controller.signal,
    });

    if (!res.ok) {
      const text = await res.text().catch(() => '');
      throw new Error(`API ${res.status}: ${text || res.statusText}`);
    }

    if (res.status === 204) return null;
    const contentType = res.headers.get('content-type') || '';
    if (!contentType.includes('application/json')) return null;
    return res.json();
  } catch (err) {
    if (err.name === 'AbortError') {
      throw new Error('Request timeout - took too long to complete');
    }
    throw err;
  } finally {
    if (timeoutId !== null) {
      clearTimeout(timeoutId);
    }
  }
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

const MAX_FILE_SIZE = 500 * 1024 * 1024; // 500MB

export const uploadPostMedia = (id, file) => {
  // Validate file size
  if (file.size > MAX_FILE_SIZE) {
    return Promise.reject(
      new Error(`File is too large (${(file.size / 1024 / 1024).toFixed(2)}MB). Maximum allowed size is 500MB.`)
    );
  }

  console.log(`[Upload] Starting: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)}MB) to /Post/${id}/media`);

  const form = new FormData();
  form.append('file', file);
  
  return request(`/Post/${id}/media`, { 
    method: 'POST', 
    body: form, 
    headers: {},
    timeout: 0 // No timeout for uploads
  }).then((res) => {
    console.log(`[Upload] Success: ${file.name}`, res);
    return res;
  }).catch((err) => {
    console.error(`[Upload] Failed for ${file.name}:`, err);
    // Better error messages
    if (err.message && err.message.includes('413')) {
      throw new Error('File is too large. Maximum size is 500MB.');
    }
    if (err.message && err.message.includes('timeout')) {
      throw new Error('Upload timeout. Try again or use a smaller file.');
    }
    if (err.message && err.message.includes('Failed to fetch')) {
      throw new Error('Connection lost during upload. Check your internet and try again.');
    }
    // Re-throw with original message
    throw err;
  });
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
  // Validate file size (max 50MB for profile pictures)
  const MAX_PROFILE_SIZE = 50 * 1024 * 1024;
  if (file.size > MAX_PROFILE_SIZE) {
    return Promise.reject(
      new Error(`Profile picture is too large (${(file.size / 1024 / 1024).toFixed(2)}MB). Maximum allowed size is 50MB.`)
    );
  }

  const form = new FormData();
  form.append('file', file);
  return request(`/User/${id}/picture`, { 
    method: 'POST', 
    body: form, 
    headers: {},
    timeout: 1800000 // 30 minutes timeout for profile pictures
  }).catch((err) => {
    if (err.message.includes('413')) {
      throw new Error('File is too large. Please use a smaller image (max 50MB).');
    }
    throw err;
  });
};
export const updateUser = (id, user) =>
  request(`/User/${id}`, { method: 'PUT', body: JSON.stringify(user) });
export const deleteUser = (id) =>
  request(`/User/${id}`, { method: 'DELETE' });

export const API_BASE_URL = BASE_URL;
