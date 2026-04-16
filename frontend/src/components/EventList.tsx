import { eventsData } from "@/lib/data";

const EventList = ({
  dateParam: _dateParam,
}: {
  dateParam: string | undefined;
}) => {
  const data = eventsData;

  return (
    <>
      {data.map((event) => (
        <div
          className="p-5 rounded-md border-2 border-gray-100 border-t-4 odd:border-t-lamaSky even:border-t-lamaPurple"
          key={event.id}
        >
          <div className="flex items-center justify-between">
            <h1 className="font-semibold text-gray-600">{event.title}</h1>
            <span className="text-xs text-gray-400">
              {event.startTime} – {event.endTime}
            </span>
          </div>
          <p className="mt-2 text-gray-400 text-sm">
            Class {event.class} · {event.date}
          </p>
        </div>
      ))}
    </>
  );
};

export default EventList;
