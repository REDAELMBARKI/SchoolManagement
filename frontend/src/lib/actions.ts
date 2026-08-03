import {
  ClassSchema,
  ExamSchema,
  IntakeConvertSchema,
  StudentSchema,
  SubjectSchema,
  TeacherSchema,
} from "./formValidationSchemas";

type CurrentState = { success: boolean; error: boolean };

export const createSubject = async (
  _currentState: CurrentState,
  _data: SubjectSchema
) => ({ success: true, error: false });
export const updateSubject = async (
  _currentState: CurrentState,
  _data: SubjectSchema
) => ({ success: true, error: false });
export const deleteSubject = async (
  _currentState: CurrentState,
  _data: FormData
) => ({ success: true, error: false });

export const createClass = async (
  _currentState: CurrentState,
  _data: ClassSchema
) => ({ success: true, error: false });
export const updateClass = async (
  _currentState: CurrentState,
  _data: ClassSchema
) => ({ success: true, error: false });
export const deleteClass = async (
  _currentState: CurrentState,
  _data: FormData
) => ({ success: true, error: false });

export const createTeacher = async (
  _currentState: CurrentState,
  _data: TeacherSchema
) => ({ success: true, error: false });
export const updateTeacher = async (
  _currentState: CurrentState,
  _data: TeacherSchema
) => ({ success: true, error: false });
export const deleteTeacher = async (
  _currentState: CurrentState,
  _data: FormData
) => ({ success: true, error: false });

export const createStudent = async (
  _currentState: CurrentState,
  _data: StudentSchema
) => ({ success: true, error: false });
export const updateStudent = async (
  _currentState: CurrentState,
  _data: StudentSchema
) => ({ success: true, error: false });
export const deleteStudent = async (
  _currentState: CurrentState,
  _data: FormData
) => ({ success: true, error: false });

export const createExam = async (
  _currentState: CurrentState,
  _data: ExamSchema
) => ({ success: true, error: false });
export const updateExam = async (
  _currentState: CurrentState,
  _data: ExamSchema
) => ({ success: true, error: false });
export const convertIntakeToStudent = async (
  _currentState: CurrentState,
  _data: IntakeConvertSchema & { intakeId: number }
) => ({ success: true, error: false });

export const deleteExam = async (
  _currentState: CurrentState,
  _data: FormData
) => ({ success: true, error: false });
