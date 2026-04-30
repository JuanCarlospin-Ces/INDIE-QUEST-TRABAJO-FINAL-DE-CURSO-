import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return (
    <section className="empty">
      <h1>404</h1>
      <p>That page doesn't exist.</p>
      <Link to="/feed" className="btn btn-primary">Go home</Link>
    </section>
  );
}
