import { useRef, useState } from 'react';

/**
 * MOCKUP file picker. Lets the user drag & drop or click to select a file,
 * but the file is not uploaded anywhere yet — we just keep its metadata
 * and call onChange with `{ name, size, type }`.
 */
export default function FileDropzone({
  value,
  onChange,
  accept = 'image/*,video/*',
}) {
  const inputRef = useRef(null);
  const [isDragging, setIsDragging] = useState(false);

  const pickFile = (file) => {
    if (!file) return;
    onChange &&
      onChange({
        name: file.name,
        size: file.size,
        type: file.type,
      });
  };

  const handleDrop = (e) => {
    e.preventDefault();
    setIsDragging(false);
    const file = e.dataTransfer?.files?.[0];
    pickFile(file);
  };

  const handleSelect = (e) => {
    const file = e.target.files?.[0];
    pickFile(file);
  };

  const clear = (e) => {
    e.stopPropagation();
    onChange && onChange(null);
    if (inputRef.current) inputRef.current.value = '';
  };

  const formatSize = (bytes) => {
    if (!bytes && bytes !== 0) return '';
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  };

  return (
    <div
      className={'dropzone' + (isDragging ? ' dropzone--drag' : '')}
      onClick={() => inputRef.current?.click()}
      onDragOver={(e) => {
        e.preventDefault();
        setIsDragging(true);
      }}
      onDragLeave={() => setIsDragging(false)}
      onDrop={handleDrop}
      role="button"
      tabIndex={0}
    >
      <input
        ref={inputRef}
        type="file"
        accept={accept}
        onChange={handleSelect}
        hidden
      />

      {value ? (
        <div className="dropzone-file">
          <div className="dropzone-file-icon">📎</div>
          <div className="dropzone-file-info">
            <strong>{value.name}</strong>
            <span className="muted">
              {value.type || 'unknown type'} · {formatSize(value.size)}
            </span>
            <span className="dropzone-note">
              Mockup only — uploads not implemented yet.
            </span>
          </div>
          <button
            type="button"
            className="btn btn-sm btn-danger"
            onClick={clear}
          >
            Remove
          </button>
        </div>
      ) : (
        <div className="dropzone-empty">
          <div className="dropzone-icon">⬆</div>
          <div>
            <strong>Drag & drop a file here</strong>
            <div className="muted">or click to browse</div>
          </div>
          <div className="dropzone-note">
            Images / video. Mockup only — uploads not implemented yet.
          </div>
        </div>
      )}
    </div>
  );
}
