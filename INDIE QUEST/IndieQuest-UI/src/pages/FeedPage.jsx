import { useEffect, useState, useMemo, useRef, useCallback } from 'react';
import { getPostsPaged, getAllUsers } from '../api/client.js';
import PostCard from '../components/PostCard.jsx';
import Spinner from '../components/Spinner.jsx';
import ErrorBox from '../components/ErrorBox.jsx';
import PageHeader from '../components/PageHeader.jsx';
import { pickField } from '../utils/format.js';

const PAGE_SIZE = 10;

export default function FeedPage() {
  const [posts, setPosts] = useState([]);
  const [users, setUsers] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState(null);
  const sentinelRef = useRef(null);

  // Initial load: first page of posts + all users for author info
  useEffect(() => {
    (async () => {
      setLoading(true);
      setError(null);
      try {
        const [paged, allUsers] = await Promise.all([
          getPostsPaged(1, PAGE_SIZE),
          getAllUsers(),
        ]);
        setPosts(paged?.data ?? []);
        setTotalPages(paged?.totalPages ?? 1);
        setUsers(Array.isArray(allUsers) ? allUsers : []);
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  // Fetch the next page and append to the list
  const loadMore = useCallback(async () => {
    if (loadingMore || page >= totalPages) return;
    const nextPage = page + 1;
    setLoadingMore(true);
    try {
      const paged = await getPostsPaged(nextPage, PAGE_SIZE);
      setPosts((prev) => [...prev, ...(paged?.data ?? [])]);
      setTotalPages(paged?.totalPages ?? totalPages);
      setPage(nextPage);
    } catch (e) {
      setError(e);
    } finally {
      setLoadingMore(false);
    }
  }, [loadingMore, page, totalPages]);

  // IntersectionObserver: fires 300px before the sentinel enters the viewport
  // so the next page loads before the user actually hits the bottom (Twitter-style)
  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel) return;
    const observer = new IntersectionObserver(
      ([entry]) => { if (entry.isIntersecting) loadMore(); },
      { rootMargin: '300px', threshold: 0 }
    );
    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [loadMore]);

  const usersById = useMemo(() => {
    const map = new Map();
    for (const u of users) {
      const id = pickField(u, 'userId', 'UserId');
      if (id) map.set(String(id), u);
    }
    return map;
  }, [users]);

  const hasMore = page < totalPages;

  return (
    <section>
      <PageHeader
        title="Home"
        subtitle="Latest posts from the community"
      />

      {loading && <Spinner />}
      <ErrorBox error={error} />

      {!loading && posts.length === 0 && !error && (
        <div className="empty">No posts yet. Be the first to post!</div>
      )}

      <div className="feed">
        {posts.map((post) => {
          const userId = String(pickField(post, 'postUserId', 'PostUserId') || '');
          return (
            <PostCard
              key={pickField(post, 'postId', 'PostId')}
              post={post}
              author={usersById.get(userId)}
              onDeleted={(id) =>
                setPosts((prev) =>
                  prev.filter(
                    (p) => String(pickField(p, 'postId', 'PostId')) !== String(id)
                  )
                )
              }
            />
          );
        })}
      </div>

      {/* Sentinel: IntersectionObserver target — invisible, triggers next page load */}
      <div ref={sentinelRef} style={{ height: 1 }} />

      {loadingMore && <Spinner />}

      {!loading && !loadingMore && !hasMore && posts.length > 0 && (
        <p className="muted" style={{ textAlign: 'center', padding: '1.5rem 0' }}>
          You&apos;ve seen all posts
        </p>
      )}
    </section>
  );
}
