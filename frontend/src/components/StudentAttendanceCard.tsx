const StudentAttendanceCard = ({ id: _id }: { id: string }) => {
  const percentage = 90;
  return (
    <div className="">
      <h1 className="text-xl font-semibold">{percentage || "-"}%</h1>
      <span className="text-sm text-gray-400">Attendance</span>
    </div>
  );
};

export default StudentAttendanceCard;
