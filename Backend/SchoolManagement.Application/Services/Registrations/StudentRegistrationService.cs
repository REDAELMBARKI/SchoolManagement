using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Services.Registrations;

    public class StudentRegistrationService
    {
        
        IEnrollmentService _enrollmentService;
        IStudentService _studentService;
        ITransaction _transaction;
        public StudentRegistrationService(IStudentService studentService , IEnrollmentService enrollmentService , ITransaction transaction)
        {
              _studentService = studentService;
              _enrollmentService = enrollmentService;
              _transaction = transaction;
        }


        public async Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto registrationRequestDto)
        {
            try
            {

                await _transaction.BeginTransactionAsync();
                var studentResponse = await _studentService.CreateAsync(registrationRequestDto.StudentReqReg);
                var enrollmentResponse = await _enrollmentService.CreateAsync(registrationRequestDto.EnrollmentReqReg);
                await _transaction.CommitTransactionAsync();
                return new StudentRegistrationResponseDto
                {
                    StudentRegRes = studentResponse,
                    EnrollmentRegRes = enrollmentResponse
                };

            }
            catch (Exception)
            {
                await _transaction.RollbackTransactionAsync();
                throw;
            }
        }
    }
