export default function Avatar({ size = 48, username = '' }) {
  const initial = (username || '?').trim().charAt(0).toUpperCase() || '?';
  const style = {
    width: size,
    height: size,
    fontSize: Math.round(size * 0.45),
  };
  return (
    <div className="avatar" style={style} aria-label={`Avatar for ${username || 'user'}`}>
      <svg viewBox="0 0 24 24" width={size} height={size} className="avatar-svg" aria-hidden>
        <circle cx="12" cy="12" r="12" fill="#1d9bf0" />
        <text
          x="50%"
          y="55%"
          textAnchor="middle"
          dominantBaseline="middle"
          fill="#fff"
          fontFamily="system-ui, sans-serif"
          fontSize="11"
          fontWeight="700"
        >
          {initial}
        </text>
      </svg>
    </div>
  );
}
