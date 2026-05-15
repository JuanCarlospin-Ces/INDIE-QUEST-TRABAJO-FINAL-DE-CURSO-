import { useState } from 'react';
import { classifyMedia } from '../utils/media.js';

function DownloadButton({ src, filename }) {
  if (!src) return null;

  const handleDownload = async (e) => {
    e.stopPropagation();
    try {
      const response = await fetch(src);
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = filename || 'file';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Download failed:', error);
      alert(`Download failed: ${error.message}`);
    }
  };

  return (
    <button
      type="button"
      className="media-download-btn"
      onClick={handleDownload}
      title="Download file"
    >
      ⬇ Download
    </button>
  );
}

export default function MediaPreview({ mediaContent }) {
  const [imgError, setImgError] = useState(false);
  const info = classifyMedia(mediaContent);

  if (info.kind === 'none') {
    return null;
  }

  const filename = info.src
    ? info.src.split('/').pop().split('?')[0]
    : info.value || 'file';

  if (info.kind === 'image') {
    return (
      <div className="media">
        {imgError ? (
          <div className="media-fallback">
            <div className="media-fallback-title">MEDIA CONTENT NOT VISIBLE</div>
            <div className="media-fallback-sub">{filename}</div>
          </div>
        ) : (
          <img
            src={info.src}
            alt="post media"
            className="media-img"
            onError={() => setImgError(true)}
          />
        )}
        <DownloadButton src={info.src} filename={filename} />
      </div>
    );
  }

  if (info.kind === 'video') {
    return (
      <div className="media">
        <video src={info.src} controls className="media-video" />
        <DownloadButton src={info.src} filename={filename} />
      </div>
    );
  }

  // unknown-url, unknown-local, or unknown plain text — file exists but can't preview
  const downloadSrc = info.src || null;
  return (
    <div className="media">
      <div className="media-fallback">
        <div className="media-fallback-title">MEDIA CONTENT NOT VISIBLE</div>
        <div className="media-fallback-sub">{filename}</div>
      </div>
      <DownloadButton src={downloadSrc} filename={filename} />
    </div>
  );
}

