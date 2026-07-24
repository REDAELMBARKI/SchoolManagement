using MediatR;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Mappers;
using SchoolManagement.Application.Results;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
        IPlanQueryService _planQueryService;
        private readonly ICurrentUserContext _currentUserContext;


    public StudentRegistrationService(
            IStudentService studentService, 
            IEnrollmentService enrollmentService, 
            IChargeService chargeService, 
            IPaymentService paymentService, 
            ITransaction transaction, 
            IMediator mediator, 
            IPlanQueryService planQueryService , 
            ICurrentUserContext currentUserContext
            )
        {
              _studentService = studentService;
              _enrollmentService = enrollmentService;
              _chargeService = chargeService;
              _paymentService = paymentService;
              _transaction = transaction;
              _mediator = mediator;
             _planQueryService = planQueryService;
             _currentUserContext = currentUserContext;

    }

        
        
        private async Task<EvaluatePaymentPlanResult> EvaluatePaymentPlanAsync(Guid PlanId , PaymentRequestDto paymentRequestDto)
        {
            var evaluatePaymentPlanResult = new EvaluatePaymentPlanResult();
            Plan plan = (await _planQueryService.GetByIdAsync(PlanId))!;
            bool isFullyPaid = paymentRequestDto.AmountPaid >= plan.Amount ;
            if (isFullyPaid) { 
               evaluatePaymentPlanResult.IsFullyPaid = true;
            }
            else
            {
               evaluatePaymentPlanResult.Amount = plan.Amount;
               evaluatePaymentPlanResult.AmountRemainingDueDays = plan.RemainingAmountDueDays;
               evaluatePaymentPlanResult.PaidAmount = paymentRequestDto.AmountPaid ;
               
            } 
            
            return evaluatePaymentPlanResult;
            
        }

        public async Task<StudentRegistrationResponseDto> RegisterStudentAsync(StudentRegistrationRequestDto registrationRequestDto)
        {
            try
            {
                ChargeResponseDto? chargeResponse = null;
                Plan? plan = null;

                await _transaction.BeginTransactionAsync();
              
                var studentResponse = await _studentService.CreateAsync(registrationRequestDto.StudentRegReq);
                registrationRequestDto.EnrollmentRegReq.StudentId = studentResponse.Id; 
                var enrollmentResponse = await _enrollmentService.CreateAsync(registrationRequestDto.EnrollmentRegReq);
                plan = (await _planQueryService.GetByIdAsync(enrollmentResponse.PlanId))!;
                var evaluatePaymentPlan = await EvaluatePaymentPlanAsync(enrollmentResponse.PlanId , registrationRequestDto.PaymentRegReq);

                if (!evaluatePaymentPlan.IsFullyPaid)
                {
                    registrationRequestDto.ChargeRegReq = new ChargeCommand
                    {
                        Amount = evaluatePaymentPlan.Amount,
                        AmountPaid = evaluatePaymentPlan.PaidAmount,
                        StudentId = studentResponse.Id,
                        SourceId = enrollmentResponse.Id,
                        ChargeType = ChargeType.Enrollment,
                        IssuedDate = DateTime.UtcNow ,
                        DueDate = DateTime.UtcNow.AddDays(plan.RemainingAmountDueDays),
                        BranchId = _currentUserContext.BranchId
                    };

   
                    if (evaluatePaymentPlan.PaidAmount > 0)
                    {
                        registrationRequestDto.ChargeRegReq.Status = ChargeStatus.PartiallyPaid;
                    }
                    else
                    {
                        registrationRequestDto.ChargeRegReq.Status = ChargeStatus.Unpaid;
                    }


                     chargeResponse = await _chargeService.CreateAsync(registrationRequestDto.ChargeRegReq);
                }


                PaymentResponseDto? paymentResponse = null;
                if (registrationRequestDto.PaymentRegReq != null)
                {
                    registrationRequestDto.PaymentRegReq.EnrollmentId = enrollmentResponse.Id;
                    paymentResponse = await _paymentService.CreateAsync(registrationRequestDto.PaymentRegReq);
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
