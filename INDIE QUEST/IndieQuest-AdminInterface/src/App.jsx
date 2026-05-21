import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout.jsx';
import FeedPage from './pages/FeedPage.jsx';
import UsersPage from './pages/UsersPage.jsx';
import UserProfilePage from './pages/UserProfilePage.jsx';
import EditUserPage from './pages/EditUserPage.jsx';
import PostDetailPage from './pages/PostDetailPage.jsx';
import ComposePostPage from './pages/ComposePostPage.jsx';
import EditPostPage from './pages/EditPostPage.jsx';
import SearchPage from './pages/SearchPage.jsx';
import NotFoundPage from './pages/NotFoundPage.jsx';

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Navigate to="/feed" replace />} />
        <Route path="/feed" element={<FeedPage />} />
        <Route path="/search" element={<SearchPage />} />
        <Route path="/users" element={<UsersPage />} />
        <Route path="/users/:id" element={<UserProfilePage />} />
        <Route path="/users/:id/edit" element={<EditUserPage />} />
        <Route path="/posts/:id" element={<PostDetailPage />} />
        <Route path="/posts/:id/edit" element={<EditPostPage />} />
        <Route path="/compose" element={<ComposePostPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}
