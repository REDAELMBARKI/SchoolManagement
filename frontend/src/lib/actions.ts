import {
  ClassSchema,
  ExamSchema,
  StudentSchema,
  SubjectSchema,
  TeacherSchema,
} from "./formValidationSchemas";

type CurrentState = { success: boolean; error: boolean };

export const createSubject = async (
  currentState: CurrentState,
  data: SubjectSchema
) => ({ success: true, error: false });
export const updateSubject = async (
  currentState: CurrentState,
  data: SubjectSchema
) => ({ success: true, error: false });
export const deleteSubject = async (
  currentState: CurrentState,
  data: FormData
) => ({ success: true, error: false });

export const createClass = async (
  currentState: CurrentState,
  data: ClassSchema
) => ({ success: true, error: false });
export const updateClass = async (
  currentState: CurrentState,
  data: ClassSchema
) => ({ success: true, error: false });
export const deleteClass = async (
  currentState: CurrentState,
  data: FormData
) => ({ success: true, error: false });

export const createTeacher = async (
  currentState: CurrentState,
  data: TeacherSchema
) => ({ success: true, error: false });
export const updateTeacher = async (
  currentState: CurrentState,
  data: TeacherSchema
) => ({ success: true, error: false });
export const deleteTeacher = async (
  currentState: CurrentState,
  data: FormData
) => ({ success: true, error: false });

export const createStudent = async (
  currentState: CurrentState,
  data: StudentSchema
) => ({ success: true, error: false });
export const updateStudent = async (
  currentState: CurrentState,
  data: StudentSchema
) => ({ success: true, error: false });
export const deleteStudent = async (
  currentState: CurrentState,
  data: FormData
) => ({ success: true, error: false });

export const createExam = async (
  currentState: CurrentState,
  data: ExamSchema
) => ({ success: true, error: false });
export const updateExam = async (
  currentState: CurrentState,
  data: ExamSchema
) => ({ success: true, error: false });
export const deleteExam = async (
  currentState: CurrentState,
  data: FormData
) => ({ success: true, error: false });
