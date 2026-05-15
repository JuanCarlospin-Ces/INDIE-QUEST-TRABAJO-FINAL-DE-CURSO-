export default function Avatar({ size = 48, username = '', profilePicture = null }) {
  const initial = (username || '?').trim().charAt(0).toUpperCase() || '?';
  const containerStyle = {
    position: 'relative',
    width: size,
    height: size,
    borderRadius: '50%',
    overflow: 'hidden',
  };

  const imageStyle = {
    position: 'absolute',
    width: '100%',
    height: '100%',
    objectFit: 'cover',
    top: 0,
    left: 0,
  };

  const svgStyle = {
    position: 'absolute',
    top: 0,
    left: 0,
    width: '100%',
    height: '100%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  };

  // If there's a profile picture, build the URL and show the image
  if (profilePicture) {
    // Build the full URL: http://localhost:5063/IndieQuest-LocalData/user/1/profile.jpg
    const apiBase = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5063/api';
    // Extract base URL without /api suffix
    const baseUrl = apiBase.replace('/api', '');
    const imageUrl = `${baseUrl}/${profilePicture}`;

    return (
      <div
        className="avatar avatar-image"
        style={containerStyle}
        aria-label={`Profile picture for ${username || 'user'}`}
        title={username}
      >
        <img
          src={imageUrl}
          alt={username}
          style={imageStyle}
          onError={(e) => {
            // If image fails to load, show the fallback avatar
            e.target.style.display = 'none';
            if (e.target.nextElementSibling) {
              e.target.nextElementSibling.style.display = 'flex';
            }
          }}
        />
        {/* Fallback SVG avatar - hidden by default */}
        <svg
          viewBox="0 0 24 24"
          className="avatar-svg"
          aria-hidden
          style={{ 
            ...svgStyle,
            display: 'none',
          }}
        >
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

  // Default: show initials avatar
  return (
    <div 
      className="avatar" 
      style={containerStyle}
      aria-label={`Avatar for ${username || 'user'}`}
    >
      <svg 
        viewBox="0 0 24 24" 
        className="avatar-svg" 
        aria-hidden
        style={svgStyle}
      >
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
