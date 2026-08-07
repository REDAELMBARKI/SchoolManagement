import { Calendar, ChevronDown, Clock3, X } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import ReactCalendar from "react-calendar";
import "react-calendar/dist/Calendar.css";

type DateTimePickerProps = {
  label: string;
  value?: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  error?: string;
  required?: boolean;
  placeholder?: string;
  className?: string;
};

function parseDateTime(value?: string) {
  if (!value) return null;

  const [datePart, timePart = "00:00"] = value.split("T");
  const [year, month, day] = datePart.split("-").map(Number);
  const [hours, minutes] = timePart.split(":").map(Number);
  if (!year || !month || !day || Number.isNaN(hours) || Number.isNaN(minutes)) return null;

  const date = new Date(year, month - 1, day, hours, minutes);
  return Number.isNaN(date.getTime()) ? null : date;
}

function formatInputDateTime(date: Date) {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function formatDisplayDateTime(date: Date) {
  return new Intl.DateTimeFormat("en", {
    month: "short",
    day: "numeric",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  }).format(date);
}

const HOURS = Array.from({ length: 24 }, (_, index) => index);
const MINUTES = Array.from({ length: 12 }, (_, index) => index * 5);

export default function DateTimePicker({
  label,
  value = "",
  onChange,
  onBlur,
  error,
  required = false,
  placeholder = "Select date and time",
  className = "",
}: DateTimePickerProps) {
  const [open, setOpen] = useState(false);
  const [timeMenu, setTimeMenu] = useState<"hour" | "minute" | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);
  const selectedDate = parseDateTime(value);

  useEffect(() => {
    if (!open) return;

    const handleOutsideClick = (event: MouseEvent) => {
      if (!containerRef.current?.contains(event.target as Node)) {
        setOpen(false);
        setTimeMenu(null);
        onBlur?.();
      }
    };

    document.addEventListener("mousedown", handleOutsideClick);
    return () => document.removeEventListener("mousedown", handleOutsideClick);
  }, [onBlur, open]);

  const updateDateTime = (date: Date) => {
    onChange(formatInputDateTime(date));
  };

  const handleCalendarChange = (
    nextValue: Date | null | [Date | null, Date | null],
  ) => {
    if (nextValue instanceof Date) {
      const nextDate = selectedDate
        ? new Date(
            nextValue.getFullYear(),
            nextValue.getMonth(),
            nextValue.getDate(),
            selectedDate.getHours(),
            selectedDate.getMinutes(),
          )
        : new Date(
            nextValue.getFullYear(),
            nextValue.getMonth(),
            nextValue.getDate(),
            9,
            0,
          );
      updateDateTime(nextDate);
    }
  };

  const handleTimeChange = (kind: "hour" | "minute", nextValue: number) => {
    const nextDate = selectedDate
      ? new Date(selectedDate)
      : new Date(2000, 0, 1, 9, 0);

    if (kind === "hour") nextDate.setHours(nextValue);
    else nextDate.setMinutes(nextValue);

    if (!selectedDate) {
      const today = new Date();
      nextDate.setFullYear(today.getFullYear(), today.getMonth(), today.getDate());
    }

    updateDateTime(nextDate);
    setTimeMenu(null);
  };

  const handleClear = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.stopPropagation();
    onChange("");
    onBlur?.();
  };

  const selectedHour = selectedDate?.getHours() ?? 9;
  const selectedMinute = selectedDate?.getMinutes() ?? 0;

  return (
    <div
      ref={containerRef}
      className={`registration-date-picker relative flex w-full flex-col gap-2 md:w-1/4 ${className}`}
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
          {selectedDate ? formatDisplayDateTime(selectedDate) : placeholder}
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

          <div className="mt-2 border-t border-gray-100 pt-3">
            <div className="mb-2 flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wide text-gray-400">
              <Clock3 size={13} />
              Time
            </div>
            <div className="flex gap-2">
              <TimeMenu
                label="Hour"
                value={selectedHour}
                options={HOURS}
                formatValue={(option) => String(option).padStart(2, "0")}
                open={timeMenu === "hour"}
                onToggle={() => setTimeMenu(timeMenu === "hour" ? null : "hour")}
                onChange={(nextValue) => handleTimeChange("hour", nextValue)}
              />
              <span className="flex items-center pb-1 text-sm font-semibold text-gray-400">:</span>
              <TimeMenu
                label="Minute"
                value={selectedMinute}
                options={MINUTES}
                formatValue={(option) => String(option).padStart(2, "0")}
                open={timeMenu === "minute"}
                onToggle={() => setTimeMenu(timeMenu === "minute" ? null : "minute")}
                onChange={(nextValue) => handleTimeChange("minute", nextValue)}
              />
            </div>
          </div>

          <div className="mt-3 flex items-center justify-between border-t border-gray-100 pt-2">
            <span className="text-[11px] text-gray-400">
              {selectedDate ? `Selected: ${formatDisplayDateTime(selectedDate)}` : "Choose a date and time"}
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

function TimeMenu({
  label,
  value,
  options,
  formatValue,
  open,
  onToggle,
  onChange,
}: {
  label: string;
  value: number;
  options: number[];
  formatValue: (value: number) => string;
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
        {formatValue(value)}
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
              {formatValue(option)}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}