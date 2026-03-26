import { Routes, Route, Navigate } from 'react-router-dom'
import BooksPage from './View/Pages/BooksPage'
import AddBookPage from './View/Pages/AddBookPage'

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/books" replace />} />
      <Route path="/books" element={<BooksPage />} />
      <Route path="/books/add" element={<AddBookPage />} />
      <Route path="*" element={<Navigate to="/books" replace />} />
    </Routes>
  )
}
