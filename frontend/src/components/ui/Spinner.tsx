interface SpinnerProps {
  /** Size in pixels (applied to both width and height). Default: 20. */
  size?: number
  className?: string
  label?: string
}

/**
 * Spinner — accessible loading indicator.
 * Uses the CSS 'spinner' keyframe defined in index.css.
 */
export default function Spinner({ size = 20, className = '', label = 'Loading…' }: SpinnerProps) {
  return (
    <span
      role="status"
      aria-label={label}
      className={`inline-block rounded-full border-2 border-text-muted border-t-primary spinner ${className}`}
      style={{ width: size, height: size, flexShrink: 0 }}
    />
  )
}
