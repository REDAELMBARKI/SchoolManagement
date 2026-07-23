using MediatR;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Services.Registrations;

    public class StudentRegistrationService
    {
        
        IEnrollmentService _enrollmentService;
        IStudentService _studentService;
        IChargeService _chargeService;
        IPaymentService _paymentService;
        ITransaction _transaction;
        IMediator _mediator;
        public StudentRegistrationService(
            IStudentService studentService, 
            IEnrollmentService enrollmentService, 
            IChargeService chargeService, 
            IPaymentService paymentService, 
            ITransaction transaction, 
            IMediator mediator)
        {
              _studentService = studentService;
              _enrollmentService = enrollmentService;
              _chargeService = chargeService;
              _paymentService = paymentService;
              _transaction = transaction;
              _mediator = mediator;
        }


        public async Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto registrationRequestDto)
        {
            try
            {

                await _transaction.BeginTransactionAsync();
              
                var studentResponse = await _studentService.CreateAsync(registrationRequestDto.StudentReqReg);
                registrationRequestDto.EnrollmentReqReg.StudentId = studentResponse.Id; 
                var enrollmentResponse = await _enrollmentService.CreateAsync(registrationRequestDto.EnrollmentReqReg);
                
                ChargeResponseDto? chargeResponse = null;
                if (registrationRequestDto.ChargeReq != null)
                {
                    registrationRequestDto.ChargeReq.StudentId = studentResponse.Id;
                    registrationRequestDto.ChargeReq.SourceId = enrollmentResponse.Id; // Link charge to enrollment
                    chargeResponse = await _chargeService.CreateAsync(registrationRequestDto.ChargeReq);
                }

                PaymentResponseDto? paymentResponse = null;
                if (registrationRequestDto.PaymentReq != null)
                {
                    registrationRequestDto.PaymentReq.EnrollmentId = enrollmentResponse.Id;
                    paymentResponse = await _paymentService.CreateAsync(registrationRequestDto.PaymentReq);
                }
               
                await _transaction.CommitTransactionAsync();
                await _mediator.Publish(new NewStudentAssignedDomainEvent(studentResponse.Id, enrollmentResponse.Id));  
                return new StudentRegistrationResponseDto
                {
                    StudentRegRes = studentResponse,
                    EnrollmentRegRes = enrollmentResponse,
                    ChargeRegRes = chargeResponse,
                    PaymentRegRes = paymentResponse
                };

            }
            catch (Exception)
            {
                await _transaction.RollbackTransactionAsync();
                throw;
            }
        }


}
