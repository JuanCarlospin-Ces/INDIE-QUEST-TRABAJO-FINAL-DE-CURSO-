import { classifyMedia } from '../utils/media.js';

export default function MediaPreview({ mediaContent }) {
  const info = classifyMedia(mediaContent);

  if (info.kind === 'none') return null;

  if (info.kind === 'image') {
    return (
      <div className="media">
        <img
          src={info.src}
          alt="post media"
          className="media-img"
          onError={(e) => {
            e.currentTarget.replaceWith(
              Object.assign(document.createElement('div'), {
                className: 'media-fallback',
                innerText: 'MEDIA CANNOT BE LOADED',
              })
            );
          }}
        />
      </div>
    );
  }

  if (info.kind === 'video') {
    return (
      <div className="media">
        <video src={info.src} controls className="media-video" />
      </div>
    );
  }

  return (
    <div className="media">
      <div className="media-fallback">
        <div className="media-fallback-title">MEDIA CANNOT BE LOADED</div>
        <div className="media-fallback-sub">
          {info.kind === 'unknown-url' ? info.src : info.value}
        </div>
      </div>
    </div>
  );
}
