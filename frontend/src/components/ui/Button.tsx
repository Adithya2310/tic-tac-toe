import { type ButtonHTMLAttributes, forwardRef } from 'react'

type ButtonVariant = 'primary' | 'secondary' | 'ghost' | 'danger'
type ButtonSize = 'sm' | 'md' | 'lg'

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant
  size?: ButtonSize
  isLoading?: boolean
}

const variantClasses: Record<ButtonVariant, string> = {
  primary: [
    'bg-primary text-text-primary border border-white/10',
    'hover:bg-primary-hover hover:shadow-[0_0_16px_rgba(124,92,252,0.35)]',
    'focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-background',
    'disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:bg-primary disabled:hover:shadow-none',
    'active:scale-[0.98]',
  ].join(' '),

  secondary: [
    'bg-transparent text-text-primary border border-border',
    'hover:bg-surface-raised hover:border-[#3B4455]',
    'focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-background',
    'disabled:opacity-40 disabled:cursor-not-allowed',
    'active:scale-[0.98]',
  ].join(' '),

  ghost: [
    'bg-transparent text-text-secondary border border-transparent',
    'hover:text-text-primary hover:bg-white/[0.04]',
    'focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-background',
    'disabled:opacity-40 disabled:cursor-not-allowed',
    'active:scale-[0.98]',
  ].join(' '),

  danger: [
    'bg-transparent text-danger border border-danger/40',
    'hover:bg-danger/10 hover:border-danger/70',
    'focus-visible:ring-2 focus-visible:ring-danger focus-visible:ring-offset-2 focus-visible:ring-offset-background',
    'disabled:opacity-40 disabled:cursor-not-allowed',
    'active:scale-[0.98]',
  ].join(' '),
}

const sizeClasses: Record<ButtonSize, string> = {
  sm: 'px-3 py-1.5 text-xs font-semibold rounded-lg',
  md: 'px-4 py-2.5 text-sm font-semibold rounded-xl',
  lg: 'px-6 py-3.5 text-sm font-semibold rounded-xl',
}

/**
 * Button — the primary interactive control.
 * Supports four semantic variants and three sizes.
 * All variants include focus-visible rings for keyboard accessibility.
 */
const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ variant = 'primary', size = 'md', isLoading = false, className = '', children, disabled, ...rest }, ref) => {
    return (
      <button
        ref={ref}
        disabled={disabled || isLoading}
        className={[
          'inline-flex items-center justify-center gap-2',
          'transition-all duration-150 cursor-pointer',
          'outline-none select-none',
          variantClasses[variant],
          sizeClasses[size],
          className,
        ].join(' ')}
        {...rest}
      >
        {isLoading ? (
          <span className="spinner inline-block h-4 w-4 rounded-full border-2 border-current border-t-transparent" aria-hidden="true" />
        ) : null}
        {children}
      </button>
    )
  },
)

Button.displayName = 'Button'
export default Button
