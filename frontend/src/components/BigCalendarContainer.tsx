import BigCalendar from "./BigCalender";

const BigCalendarContainer = ({
  type: _type,
  id: _id,
}: {
  type: "teacherId" | "classId";
  id: string | number;
}) => {
  const schedule = [
    {
      title: "Math",
      start: new Date(new Date().setHours(8, 0, 0, 0)),
      end: new Date(new Date().setHours(8, 45, 0, 0)),
    },
  ];

  return (
    <div className="">
      <BigCalendar data={schedule} />
    </div>
  );
};

export default BigCalendarContainer;
