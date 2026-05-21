export default function ErrorBox({ error }) {
  if (!error) return null;
  return (
    <div className="error-box">
      <strong>Something went wrong</strong>
      <div>{error.message || String(error)}</div>
    </div>
  );
}
