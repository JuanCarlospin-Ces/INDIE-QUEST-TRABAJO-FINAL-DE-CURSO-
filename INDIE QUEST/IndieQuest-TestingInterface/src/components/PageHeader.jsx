export default function PageHeader({ title, subtitle, right }) {
  return (
    <div className="page-header">
      <div>
        <h1>{title}</h1>
        {subtitle && <p className="muted">{subtitle}</p>}
      </div>
      {right && <div>{right}</div>}
    </div>
  );
}
