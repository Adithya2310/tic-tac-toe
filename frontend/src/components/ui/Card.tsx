import { type HTMLAttributes } from 'react'

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  /** 'default' = solid surface | 'raised' = elevated surface with ambient shadow */
  variant?: 'default' | 'raised'
}

/**
 * Card — the standard surface container.
 * Matches DESIGN.md §Elevation Layer 1 and Layer 2 specs.
 */
export default function Card({ variant = 'default', className = '', children, ...rest }: CardProps) {
  const base = 'rounded-2xl border border-border'

  const variantClass =
    variant === 'raised'
      ? 'bg-surface-raised shadow-[0_4px_16px_-4px_rgba(0,0,0,0.45)]'
      : 'bg-surface'

  return (
    <div className={`${base} ${variantClass} ${className}`} {...rest}>
      {children}
    </div>
  )
}
