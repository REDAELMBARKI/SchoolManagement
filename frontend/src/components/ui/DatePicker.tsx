import { Calendar, ChevronDown, X } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import ReactCalendar from "react-calendar";
import "react-calendar/dist/Calendar.css";

type DatePickerProps = {
  label: string;
  value?: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  error?: string;
  required?: boolean;
  placeholder?: string;
  className?: string;
};

function parseDate(value?: string) {
  if (!value) return null;

  const [year, month, day] = value.split("-").map(Number);
  if (!year || !month || !day) return null;

  const date = new Date(year, month - 1, day);
  return Number.isNaN(date.getTime()) ? null : date;
}

function formatInputDate(date: Date) {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function formatDisplayDate(date: Date) {
  return new Intl.DateTimeFormat("en", {
    month: "short",
    day: "numeric",
    year: "numeric",
  }).format(date);
}

export default function DatePicker({
  label,
  value = "",
  onChange,
  onBlur,
  error,
  required = false,
  placeholder = "Select a date",
  className = "",
}: DatePickerProps) {
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);
  const selectedDate = parseDate(value);

  useEffect(() => {
    if (!open) return;

    const handleOutsideClick = (event: MouseEvent) => {
      if (!containerRef.current?.contains(event.target as Node)) {
        setOpen(false);
        onBlur?.();
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [onBlur, open]);

  const handleCalendarChange = (
    nextValue: Date | null | [Date | null, Date | null],
  ) => {
    if (nextValue instanceof Date) {
      onChange(formatInputDate(nextValue));
      setOpen(false);
      onBlur?.();
    }
  };

  const handleClear = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();
    onChange("");
    onBlur?.();
  };

  return (
    <div
      ref={containerRef}
      className={`registration-date-picker relative flex flex-col gap-2 w-full md:w-1/4 ${className}`}
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
          <Calendar size={15} strokeWidth={2} />
        </span>
        <span className={`min-w-0 flex-1 truncate ${selectedDate ? "text-gray-700" : "text-gray-400"}`}>
          {selectedDate ? formatDisplayDate(selectedDate) : placeholder}
        </span>
        <ChevronDown
          size={16}
          className={`shrink-0 text-gray-400 transition-transform ${open ? "rotate-180" : ""}`}
        />
      </button>
      {selectedDate && (
        <button
          type="button"
          aria-label={`Clear ${label.toLowerCase()}`}
          onClick={handleClear}
          className="absolute right-9 top-[31px] rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
        >
          <X size={14} />
        </button>
      )}

      {open && (
        <div
          role="dialog"
          aria-label={`${label} calendar`}
          className="absolute left-0 top-[calc(100%+8px)] z-50 w-[min(320px,calc(100vw-32px))] rounded-xl border border-gray-100 bg-white p-3 shadow-[0_16px_40px_rgba(31,41,55,0.14)]"
        >
          <ReactCalendar
            onChange={handleCalendarChange}
            value={selectedDate}
            calendarType="iso8601"
            next2Label={null}
            prev2Label={null}
            showNeighboringMonth={false}
          />
          <div className="mt-2 flex items-center justify-between border-t border-gray-100 pt-2">
            <span className="text-[11px] text-gray-400">
              {selectedDate ? `Selected: ${formatDisplayDate(selectedDate)}` : "Choose a date"}
            </span>
            {selectedDate && (
              <button
                type="button"
                onClick={handleClear}
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