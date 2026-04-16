import { useSearchParams } from "react-router-dom";
import { useEffect, useState } from "react";
import Calendar from "react-calendar";
import "react-calendar/dist/Calendar.css";

type ValuePiece = Date | null;

type Value = ValuePiece | [ValuePiece, ValuePiece];

const EventCalendar = () => {
  const [value, onChange] = useState<Value>(new Date());
  const [, setSearchParams] = useSearchParams();

  useEffect(() => {
    if (value instanceof Date) {
      setSearchParams(
        (prev) => {
          const next = new URLSearchParams(prev);
          next.set("date", String(value));
          return next;
        },
        { replace: true }
      );
    }
  }, [value, setSearchParams]);

  return <Calendar onChange={onChange} value={value} />;
};

export default EventCalendar;
