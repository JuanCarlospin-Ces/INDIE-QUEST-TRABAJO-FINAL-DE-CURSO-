// Helpers to detect what kind of MediaContent a post has.
// MediaContent in the API is currently a string. Until proper media uploads
// exist, we try to guess from extension/url; otherwise we render a fallback.

const IMAGE_RE = /\.(png|jpe?g|gif|webp|bmp|svg)(\?.*)?$/i;
const VIDEO_RE = /\.(mp4|webm|ogg|mov)(\?.*)?$/i;

export function classifyMedia(mediaContent) {
  if (!mediaContent || typeof mediaContent !== 'string') {
    return { kind: 'none' };
  }
  const value = mediaContent.trim();
  if (!value) return { kind: 'none' };

  const isUrl = /^https?:\/\//i.test(value);
  if (isUrl && IMAGE_RE.test(value)) return { kind: 'image', src: value };
  if (isUrl && VIDEO_RE.test(value)) return { kind: 'video', src: value };
  if (isUrl) return { kind: 'unknown-url', src: value };
  return { kind: 'unknown', value };
}
