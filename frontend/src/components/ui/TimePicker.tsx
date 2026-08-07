import { ChevronDown, Clock3, X } from "lucide-react";
import { useEffect, useRef, useState } from "react";

type TimePickerProps = {
  label: string;
  value?: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  error?: string;
  required?: boolean;
  placeholder?: string;
  className?: string;
};

const HOURS = Array.from({ length: 24 }, (_, index) => index);
const MINUTES = Array.from({ length: 12 }, (_, index) => index * 5);

export default function TimePicker({
  label,
  value = "",
  onChange,
  onBlur,
  error,
  required = false,
  placeholder = "Select a time",
  className = "",
}: TimePickerProps) {
  const [open, setOpen] = useState(false);
  const [menu, setMenu] = useState<"hour" | "minute" | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const [hour = "", minute = ""] = value.split(":");
  const hasValue = /^\d{2}:\d{2}$/.test(value);

  useEffect(() => {
    if (!open) return;

    const handleOutsideClick = (event: MouseEvent) => {
      if (!containerRef.current?.contains(event.target as Node)) {
        setOpen(false);
        setMenu(null);
        onBlur?.();
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [onBlur, open]);

  const updatePart = (part: "hour" | "minute", nextValue: number) => {
    const nextHour = part === "hour" ? nextValue : Number(hour || 9);
    const nextMinute = part === "minute" ? nextValue : Number(minute || 0);
    onChange(`${String(nextHour).padStart(2, "0")}:${String(nextMinute).padStart(2, "0")}`);
    setMenu(null);
  };

  const clear = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();
    onChange("");
    onBlur?.();
  };

  return (
    <div
      ref={containerRef}
      className={`relative flex w-full flex-col gap-2 md:w-1/4 ${className}`}
    >
      <label className="text-xs text-gray-500">
        {label} {required && <span className="text-gray-700">*</span>}
      </label>

      <button
        type="button"
        aria-haspopup="dialog"
        aria-expanded={open}
        onClick={() => setOpen((current) => !current)}
        className={`group flex min-h-[42px] w-full items-center gap-3 rounded-md bg-white px-3 pr-16 text-left text-sm ring-[1.5px] transition ${
          error
            ? "ring-red-300 focus:ring-red-400"
            : open
              ? "ring-lamaPurple"
              : "ring-gray-300 hover:ring-gray-400"
        }`}
      >
        <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-md bg-lamaPurple/10 text-lamaPurple">
          <Clock3 size={15} strokeWidth={2} />
        </span>
        <span className={`min-w-0 flex-1 truncate ${hasValue ? "text-gray-700" : "text-gray-400"}`}>
          {hasValue ? value : placeholder}
        </span>
        <ChevronDown
          size={16}
          className={`shrink-0 text-gray-400 transition-transform ${open ? "rotate-180" : ""}`}
        />
      </button>

      {hasValue && (
        <button
          type="button"
          aria-label={`Clear ${label.toLowerCase()}`}
          onClick={clear}
          className="absolute right-9 top-[31px] rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
        >
          <X size={14} />
        </button>
      )}

      {open && (
        <div
          role="dialog"
          aria-label={`${label} selector`}
          className="absolute left-0 top-[calc(100%+8px)] z-50 w-[min(260px,calc(100vw-32px))] rounded-xl border border-gray-100 bg-white p-3 shadow-[0_16px_40px_rgba(31,41,55,0.14)]"
        >
          <div className="mb-2 flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wide text-gray-400">
            <Clock3 size={13} />
            Choose time
          </div>
          <div className="flex items-center gap-2">
            <TimeMenu
              label="Hour"
              value={Number(hour || 9)}
              options={HOURS}
              open={menu === "hour"}
              onToggle={() => setMenu(menu === "hour" ? null : "hour")}
              onChange={(nextValue) => updatePart("hour", nextValue)}
            />
            <span className="text-sm font-semibold text-gray-400">:</span>
            <TimeMenu
              label="Minute"
              value={Number(minute || 0)}
              options={MINUTES}
              open={menu === "minute"}
              onToggle={() => setMenu(menu === "minute" ? null : "minute")}
              onChange={(nextValue) => updatePart("minute", nextValue)}
            />
          </div>
          <div className="mt-3 flex items-center justify-between border-t border-gray-100 pt-2">
            <span className="text-[11px] text-gray-400">
              {hasValue ? `Selected: ${value}` : "Choose an hour and minute"}
            </span>
            {hasValue && (
              <button
                type="button"
                onClick={clear}
                className="text-[11px] font-medium text-gray-500 hover:text-gray-800"
              >
                Clear
              </button>
            )}
          </div>
        </div>
      )}

      {error && <p className="text-xs text-red-400">{error}</p>}
    </div>
  );
}

function TimeMenu({
  label,
  value,
  options,
  open,
  onToggle,
  onChange,
}: {
  label: string;
  value: number;
  options: number[];
  open: boolean;
  onToggle: () => void;
  onChange: (value: number) => void;
}) {
  return (
    <div className="relative flex-1">
      <button
        type="button"
        aria-label={`Select ${label.toLowerCase()}`}
        aria-expanded={open}
        onClick={onToggle}
        className="flex h-9 w-full items-center justify-between rounded-md bg-gray-50 px-3 text-sm font-medium text-gray-700 ring-1 ring-gray-200 hover:bg-lamaPurple/5 hover:ring-lamaPurple"
      >
        {String(value).padStart(2, "0")}
        <ChevronDown size={14} className={`text-gray-400 transition-transform ${open ? "rotate-180" : ""}`} />
      </button>
      {open && (
        <div className="absolute bottom-[calc(100%+6px)] left-0 z-10 max-h-44 w-full overflow-y-auto rounded-lg border border-gray-100 bg-white p-1 shadow-lg">
          {options.map((option) => (
            <button
              type="button"
              key={option}
              onClick={() => onChange(option)}
              className={`w-full rounded-md px-3 py-1.5 text-left text-xs hover:bg-lamaPurple/10 ${
                option === value ? "bg-lamaPurple/10 font-semibold text-gray-800" : "text-gray-600"
              }`}
            >
              {String(option).padStart(2, "0")}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}