import { type HTMLAttributes } from 'react'

type BadgeVariant = 'default' | 'primary' | 'success' | 'warning' | 'danger' | 'muted'

interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  variant?: BadgeVariant
}

const variantClasses: Record<BadgeVariant, string> = {
  default:  'bg-surface-raised text-text-secondary border border-border',
  primary:  'bg-primary/15 text-primary border border-primary/30',
  success:  'bg-success/15 text-success border border-success/30',
  warning:  'bg-warning/15 text-warning border border-warning/30',
  danger:   'bg-danger/15 text-danger border border-danger/30',
  muted:    'bg-surface text-text-muted border border-border',
}

/**
 * Badge — a small pill/chip for status labels and mode indicators.
 * Uses DESIGN.md §Micro-Components geometry (rounded-md).
 */
export default function Badge({ variant = 'default', className = '', children, ...rest }: BadgeProps) {
  return (
    <span
      className={`inline-flex items-center gap-1 rounded-md px-2 py-0.5 text-xs font-semibold font-mono-tabular tracking-wide ${variantClasses[variant]} ${className}`}
      {...rest}
    >
      {children}
    </span>
  )
}
