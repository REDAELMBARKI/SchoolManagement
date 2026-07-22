using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Services.Registrations
{
    public class StudentRegistrationService
    {
        
        IEnrollmentService _enrollmentService;
        IStudentService _studentService;


        public StudentRegistrationService(IStudentService studentService , IEnrollmentService enrollmentService)
        {
              _studentService = studentService;
              _enrollmentService = enrollmentService;
        }


        public async Task<StudentRegistrationResponseDto> Register(StudentRegistrationRequestDto registrationRequestDto)
        {
            StudentResponseDto studentRes = await _studentService.CreateAsync(registrationRequestDto.StudentReqReg);
            EnrollmentResponseDto enrollmentRes = await _enrollmentService.CreateAsync(registrationRequestDto.EnrollmentReqReg);

            return new StudentRegistrationResponseDto
            {
                Student = studentRes,
                Enrollment = enrollmentRes
            };
        }
    }
}
