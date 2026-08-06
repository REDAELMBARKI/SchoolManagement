import { Check, ChevronDown, X } from "lucide-react";
import { useEffect, useId, useRef, useState } from "react";

export type SelectObjectOption<TKey extends string | number = string> = {
  key: TKey;
  value: string;
};

export type ObjectSelectProps<TKey extends string | number = string> = {
  options: readonly SelectObjectOption<TKey>[];
  value?: TKey | TKey[] | null;
  onChange: (value: TKey | TKey[]) => void;
  onBlur?: () => void;
  isMulty: boolean;
  label?: string;
  placeholder?: string;
  error?: string;
  disabled?: boolean;
  className?: string;
};

/**
 * A controlled, non-native select for `{ key, value }` options.
 *
 * The form always receives keys. The component maps keys to display values,
 * so API foreign keys can stay in the form while users see friendly labels.
 */
export default function ObjectSelect<TKey extends string | number = string>({
  options,
  value,
  onChange,
  onBlur,
  isMulty,
  label,
  placeholder = "Select an option",
  error,
  disabled = false,
  className = "",
}: ObjectSelectProps<TKey>) {
  const [open, setOpen] = useState(false);
  const rootRef = useRef<HTMLDivElement>(null);
  const listboxId = useId();
  const selected = (isMulty
    ? Array.isArray(value)
      ? value
      : []
    : value === null || value === undefined || Array.isArray(value)
      ? []
      : [value]) as TKey[];
  const selectedOptions = selected
    .map((key) => options.find((option) => Object.is(option.key, key)))
    .filter((option): option is SelectObjectOption<TKey> => Boolean(option));

  useEffect(() => {
    const closeOnOutsideClick = (event: MouseEvent) => {
      if (rootRef.current && !rootRef.current.contains(event.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener("mousedown", closeOnOutsideClick);
    return () => document.removeEventListener("mousedown", closeOnOutsideClick);
  }, []);

  const isSelected = (option: SelectObjectOption<TKey>) =>
    selected.some((key) => Object.is(key, option.key));

  const selectOption = (option: SelectObjectOption<TKey>) => {
    if (isMulty) {
      const next = isSelected(option)
        ? selected.filter((key) => !Object.is(key, option.key))
        : [...selected, option.key];
      onChange(next);
      return;
    }

    onChange(option.key);
    setOpen(false);
  };

  const clearOption = (key: TKey) => {
    onChange(selected.filter((item) => !Object.is(item, key)));
  };

  const displayValue = isMulty
    ? selectedOptions.length
      ? `${selectedOptions.length} selected`
      : placeholder
    : selectedOptions[0]?.value ?? placeholder;

  return (
    <div ref={rootRef} className={`relative flex flex-col gap-2 ${className}`}>
      {label && <label className="text-xs font-medium text-gray-500">{label}</label>}

      <button
        type="button"
        disabled={disabled}
        aria-haspopup="listbox"
        aria-expanded={open}
        aria-controls={listboxId}
        onClick={() => {
          setOpen((current) => !current);
          onBlur?.();
        }}
        className={`flex min-h-[42px] w-full items-center justify-between gap-3 rounded-lg border bg-white px-3 text-left text-sm transition-all ${
          error
            ? "border-red-300 ring-1 ring-red-100"
            : open
              ? "border-[#9ddced] ring-2 ring-[#e4f5fa]"
              : "border-[#d9d7d2] hover:border-[#b9dce6]"
        } disabled:cursor-not-allowed disabled:bg-gray-50 disabled:text-gray-400`}
      >
        <span className={selectedOptions.length ? "text-[#465156]" : "text-[#a29c93]"}>
          {displayValue}
        </span>
        <ChevronDown
          size={16}
          className={`shrink-0 text-[#8f9aa0] transition-transform ${open ? "rotate-180" : ""}`}
        />
      </button>

      {isMulty && selectedOptions.length > 0 && (
        <div className="flex flex-wrap gap-1.5">
          {selectedOptions.map((option) => (
            <span
              key={String(option.key)}
              className="inline-flex items-center gap-1 rounded-full bg-[#edf9fd] px-2 py-1 text-[11px] text-sky-800"
            >
              {option.value}
              <button
                type="button"
                aria-label={`Remove ${option.value}`}
                onClick={() => clearOption(option.key)}
                className="rounded-full hover:bg-white/70"
              >
                <X size={12} />
              </button>
            </span>
          ))}
        </div>
      )}

      {open && !disabled && (
        <div
          id={listboxId}
          role="listbox"
          aria-multiselectable={isMulty}
          className="absolute left-0 top-[calc(100%+6px)] z-30 max-h-60 w-full overflow-y-auto rounded-xl border border-[#e4dfd6] bg-white p-1.5 shadow-xl"
        >
          {options.length === 0 ? (
            <p className="px-3 py-2 text-xs text-gray-400">No options available</p>
          ) : (
            options.map((option) => {
              const selectedOption = isSelected(option);
              return (
                <button
                  type="button"
                  role="option"
                  aria-selected={selectedOption}
                  key={String(option.key)}
                  onClick={() => selectOption(option)}
                  className={`flex w-full items-center justify-between rounded-lg px-3 py-2 text-left text-sm transition-colors ${
                    selectedOption
                      ? "bg-[#edf9fd] text-sky-900"
                      : "text-[#586268] hover:bg-[#fbf8f2]"
                  }`}
                >
                  <span>{option.value}</span>
                  {selectedOption && <Check size={15} className="text-sky-600" />}
                </button>
              );
            })
          )}
        </div>
      )}

      {error && <p className="text-xs text-red-400">{error}</p>}
    </div>
  );
}