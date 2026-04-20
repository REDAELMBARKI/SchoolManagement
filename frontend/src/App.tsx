import { Navigate, Route, Routes } from "react-router-dom";
import DashboardLayout from "./layouts/DashboardLayout";
import LoginPage from "./pages/LoginPage";
import AdminPage from "./pages/AdminPage";
import StudentPage from "./pages/StudentPage";
import TeacherPage from "./pages/TeacherPage";
import ParentPage from "./pages/ParentPage";
import ProfilePage from "./pages/ProfilePage";
import SettingsPage from "./pages/SettingsPage";
import LogoutPage from "./pages/LogoutPage";
import NotFoundPage from "./pages/NotFoundPage";
import StudentListPage from "./pages/list/StudentListPage";
import SingleStudentPage from "./pages/list/SingleStudentPage";
import TeacherListPage from "./pages/list/TeacherListPage";
import SingleTeacherPage from "./pages/list/SingleTeacherPage";
import ParentListPage from "./pages/list/ParentListPage";
import SubjectListPage from "./pages/list/SubjectListPage";
import ClassListPage from "./pages/list/ClassListPage";
import LessonListPage from "./pages/list/LessonListPage";
import ExamListPage from "./pages/list/ExamListPage";
import AssignmentListPage from "./pages/list/AssignmentListPage";
import ResultListPage from "./pages/list/ResultListPage";
import EventListPage from "./pages/list/EventListPage";
import AnnouncementListPage from "./pages/list/AnnouncementListPage";
import AttendanceListPage from "./pages/list/AttendanceListPage";
import MessagesPage from "./pages/list/MessagesPage";
import IntakeListPage from "./pages/list/IntakeListPage";
import TableCreatePage from "./pages/forms/TableCreatePage";
import TableDeletePage from "./pages/forms/TableDeletePage";
import TableEditPage from "./pages/forms/TableEditPage";

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<LoginPage />} />
      <Route path="/sign-in/*" element={<Navigate to="/" replace />} />
      <Route element={<DashboardLayout />}>
        <Route path="admin" element={<AdminPage />} />
        <Route path="student" element={<StudentPage />} />
        <Route path="teacher" element={<TeacherPage />} />
        <Route path="parent" element={<ParentPage />} />
        <Route path="profile" element={<ProfilePage />} />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="logout" element={<LogoutPage />} />
        <Route path="list/intakes" element={<IntakeListPage />} />
        <Route path="list/students" element={<StudentListPage />} />
        <Route path="list/students/:id" element={<SingleStudentPage />} />
        <Route path="list/teachers" element={<TeacherListPage />} />
        <Route path="list/teachers/:id" element={<SingleTeacherPage />} />
        <Route path="list/parents" element={<ParentListPage />} />
        <Route path="list/subjects" element={<SubjectListPage />} />
        <Route path="list/classes" element={<ClassListPage />} />
        <Route path="list/lessons" element={<LessonListPage />} />
        <Route path="list/exams" element={<ExamListPage />} />
        <Route path="list/assignments" element={<AssignmentListPage />} />
        <Route path="list/results" element={<ResultListPage />} />
        <Route path="list/events" element={<EventListPage />} />
        <Route path="list/announcements" element={<AnnouncementListPage />} />
        <Route path="list/attendance" element={<AttendanceListPage />} />
        <Route path="list/messages" element={<MessagesPage />} />
        <Route path="list/intakes" element={<IntakeListPage />} />
      <Route path="list/:table/new" element={<TableCreatePage />} />
      <Route path="list/:table/:id/edit" element={<TableEditPage />} />
      <Route path="list/:table/:id/delete" element={<TableDeletePage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
