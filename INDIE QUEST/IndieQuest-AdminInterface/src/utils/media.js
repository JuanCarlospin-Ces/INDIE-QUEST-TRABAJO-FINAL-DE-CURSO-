// Helpers to detect what kind of MediaContent a post has.
// MediaContent can be an HTTP URL or a relative local path (IndieQuest-LocalData/...).

const IMAGE_RE = /\.(png|jpe?g|gif|webp|bmp|svg)(\?.*)?$/i;
const VIDEO_RE = /\.(mp4|webm|ogg|mov)(\?.*)?$/i;
const LOCAL_DATA_RE = /^IndieQuest-LocalData\//i;

function getBaseUrl() {
  const apiBase = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5063/api';
  return apiBase.replace(/\/api$/, '');
}

export function classifyMedia(mediaContent) {
  if (!mediaContent || typeof mediaContent !== 'string') {
    return { kind: 'none' };
  }
  const value = mediaContent.trim();
  if (!value) return { kind: 'none' };

  const isUrl = /^https?:\/\//i.test(value);
  const isLocalPath = LOCAL_DATA_RE.test(value);

  // Resolve local paths to full server URLs
  const src = isLocalPath ? `${getBaseUrl()}/${value}` : value;

  if ((isUrl || isLocalPath) && IMAGE_RE.test(value)) return { kind: 'image', src };
  if ((isUrl || isLocalPath) && VIDEO_RE.test(value)) return { kind: 'video', src };
  if (isUrl) return { kind: 'unknown-url', src };
  if (isLocalPath) return { kind: 'unknown-local', src, value };
  return { kind: 'unknown', value };
}
