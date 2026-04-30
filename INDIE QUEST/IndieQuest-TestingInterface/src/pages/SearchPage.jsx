import { useEffect, useMemo, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { getAllPosts, getAllUsers } from '../api/client.js';
import Avatar from '../components/Avatar.jsx';
import PostCard from '../components/PostCard.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import PageHeader from '../components/PageHeader.jsx';
import { pickField } from '../utils/format.js';

const TABS = [
  { key: 'all', label: 'All' },
  { key: 'users', label: 'Users' },
  { key: 'posts', label: 'Posts' },
  { key: 'tags', label: 'Tags' },
];

function matches(haystack, needle) {
  if (!haystack) return false;
  return String(haystack).toLowerCase().includes(needle);
}

export default function SearchPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const initialQ = searchParams.get('q') || '';
  const initialTab = searchParams.get('tab') || 'all';

  const [query, setQuery] = useState(initialQ);
  const [tab, setTab] = useState(initialTab);

  const [users, setUsers] = useState([]);
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const [u, p] = await Promise.all([getAllUsers(), getAllPosts()]);
        setUsers(Array.isArray(u) ? u : []);
        setPosts(Array.isArray(p) ? p : []);
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  // Sync URL with state
  useEffect(() => {
    const next = {};
    if (query) next.q = query;
    if (tab && tab !== 'all') next.tab = tab;
    setSearchParams(next, { replace: true });
  }, [query, tab, setSearchParams]);

  const usersById = useMemo(() => {
    const map = new Map();
    for (const u of users) {
      const id = pickField(u, 'userId', 'UserId');
      if (id) map.set(String(id), u);
    }
    return map;
  }, [users]);

  const q = query.trim().toLowerCase().replace(/^[#@]/, '');

  const matchedUsers = useMemo(() => {
    if (!q) return [];
    return users.filter(
      (u) =>
        matches(pickField(u, 'username', 'Username'), q) ||
        matches(pickField(u, 'userBio', 'UserBio'), q) ||
        matches(pickField(u, 'email', 'Email'), q)
    );
  }, [users, q]);

  const matchedPosts = useMemo(() => {
    if (!q) return [];
    return posts.filter(
      (p) =>
        matches(pickField(p, 'title', 'Title'), q) ||
        matches(pickField(p, 'description', 'Description'), q)
    );
  }, [posts, q]);

  const matchedTags = useMemo(() => {
    if (!q) return [];
    const seen = new Map(); // tagName -> { tagName, count, postIds }
    for (const p of posts) {
      const tags = pickField(p, 'tags', 'Tags') || [];
      for (const t of tags) {
        const name = pickField(t, 'tagName', 'TagName');
        if (!name) continue;
        if (!matches(name, q)) continue;
        const key = name.toLowerCase();
        const entry = seen.get(key) || { tagName: name, count: 0, postIds: [] };
        entry.count += 1;
        entry.postIds.push(pickField(p, 'postId', 'PostId'));
        seen.set(key, entry);
      }
    }
    return [...seen.values()].sort((a, b) => b.count - a.count);
  }, [posts, q]);

  const showUsers = tab === 'all' || tab === 'users';
  const showPosts = tab === 'all' || tab === 'posts';
  const showTags = tab === 'all' || tab === 'tags';

  const totalResults =
    matchedUsers.length + matchedPosts.length + matchedTags.length;

  return (
    <section>
      <PageHeader
        title="Search"
        subtitle="Find users, posts, and tags"
      />

      <div className="search-bar">
        <span className="search-icon">🔍</span>
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search IndieQuest..."
          autoFocus
        />
        {query && (
          <button
            type="button"
            className="search-clear"
            onClick={() => setQuery('')}
            title="Clear"
          >
            ✕
          </button>
        )}
      </div>

      <div className="tabs">
        {TABS.map((t) => (
          <button
            key={t.key}
            className={'tab' + (tab === t.key ? ' tab--active' : '')}
            onClick={() => setTab(t.key)}
            type="button"
          >
            {t.label}
          </button>
        ))}
      </div>

      {loading && <Spinner />}
      <ErrorBox error={error} />

      {!loading && !q && (
        <div className="empty">Type something to start searching.</div>
      )}

      {!loading && q && totalResults === 0 && (
        <div className="empty">
          No results for <strong>"{query}"</strong>.
        </div>
      )}

      {!loading && q && showUsers && matchedUsers.length > 0 && (
        <div className="search-section">
          <h2 className="section-title">People</h2>
          <div className="users-grid">
            {matchedUsers.map((u) => {
              const id = pickField(u, 'userId', 'UserId');
              const username = pickField(u, 'username', 'Username');
              const bio = pickField(u, 'userBio', 'UserBio');
              const available = pickField(u, 'availableForWork', 'AvailableForWork');
              return (
                <Link key={id} to={`/users/${id}`} className="user-card user-card--link">
                  <Avatar username={username} size={56} />
                  <div className="user-card-body">
                    <span className="user-name">@{username}</span>
                    {available && <span className="badge">Available</span>}
                    {bio && <p className="muted">{bio}</p>}
                  </div>
                </Link>
              );
            })}
          </div>
        </div>
      )}

      {!loading && q && showTags && matchedTags.length > 0 && (
        <div className="search-section">
          <h2 className="section-title">Tags</h2>
          <div className="tag-grid">
            {matchedTags.map((t) => (
              <button
                key={t.tagName}
                type="button"
                className="tag-chip"
                onClick={() => {
                  setQuery(t.tagName);
                  setTab('posts');
                }}
                title="Search posts with this tag"
              >
                <span className="tag-chip-name">#{t.tagName}</span>
                <span className="tag-chip-count">{t.count}</span>
              </button>
            ))}
          </div>
        </div>
      )}

      {!loading && q && showPosts && matchedPosts.length > 0 && (
        <div className="search-section">
          <h2 className="section-title">Posts</h2>
          <div className="feed">
            {matchedPosts.map((post) => {
              const userId = String(
                pickField(post, 'postUserId', 'PostUserId') || ''
              );
              return (
                <PostCard
                  key={pickField(post, 'postId', 'PostId')}
                  post={post}
                  author={usersById.get(userId)}
                  onDeleted={(id) =>
                    setPosts((prev) =>
                      prev.filter(
                        (p) =>
                          String(pickField(p, 'postId', 'PostId')) !== String(id)
                      )
                    )
                  }
                />
              );
            })}
          </div>
        </div>
      )}
    </section>
  );
}
