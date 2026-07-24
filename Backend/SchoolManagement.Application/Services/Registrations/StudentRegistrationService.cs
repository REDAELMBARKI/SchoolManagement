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

        
        
        private async Task<EvaluatePaymentPlanResult> EvaluatePaymentPlanAsync(Guid PlanId , decimal amountPaid)
        {
            var evaluatePaymentPlanResult = new EvaluatePaymentPlanResult();
            Plan plan = (await _planQueryService.GetByIdAsync(PlanId))!;
            bool isFullyPaid = amountPaid >= plan.Amount ;
            if (isFullyPaid) { 
               evaluatePaymentPlanResult.IsFullyPaid = true;
            }
            else
            {
               evaluatePaymentPlanResult.Amount = plan.Amount;
               evaluatePaymentPlanResult.AmountRemainingDueDays = plan.RemainingAmountDueDays;
               evaluatePaymentPlanResult.PaidAmount = amountPaid ;
               
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

                var studentCommand = new StudentCommand
                {
                    FirstName = registrationRequestDto.StudentRegReq.FirstName,
                    LastName = registrationRequestDto.StudentRegReq.LastName,
                    Email = registrationRequestDto.StudentRegReq.Email,
                    Phone = registrationRequestDto.StudentRegReq.Phone,
                    DateOfBirth = registrationRequestDto.StudentRegReq.DateOfBirth,
                    GenderId = registrationRequestDto.StudentRegReq.GenderId,
                    LevelId = registrationRequestDto.StudentRegReq.LevelId,
                    IntakeId = registrationRequestDto.StudentRegReq.IntakeId,
                    IsDirectRegistration = registrationRequestDto.StudentRegReq.IsDirectRegistration
                };
              
                var studentResponse = await _studentService.CreateAsync(studentCommand);

                var enrollmentCommand = new EnrollmentCommand
                {
                    StudentId = studentResponse.Id,
                    LevelId = registrationRequestDto.EnrollmentRegReq.LevelId,
                    SubjectId = registrationRequestDto.EnrollmentRegReq.SubjectId,
                    PlanId = registrationRequestDto.EnrollmentRegReq.PlanId,
                    Notes = registrationRequestDto.EnrollmentRegReq.Notes,
                    BranchId = registrationRequestDto.EnrollmentRegReq.BranchId,
                    PreferedScheduleId = registrationRequestDto.EnrollmentRegReq.PreferedScheduleId,
                    GroupId = registrationRequestDto.EnrollmentRegReq.GroupId ?? Guid.Empty
                };

                var enrollmentResponse = await _enrollmentService.CreateAsync(enrollmentCommand);
                plan = (await _planQueryService.GetByIdAsync(enrollmentResponse.PlanId))!;
                var evaluatePaymentPlan = await EvaluatePaymentPlanAsync(enrollmentResponse.PlanId , registrationRequestDto.PaymentRegReq.Amount);

                if (!evaluatePaymentPlan.IsFullyPaid)
                {
                    var chargeCommand = new ChargeCommand
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
                        chargeCommand.Status = ChargeStatus.PartiallyPaid;
                    }
                    else
                    {
                        chargeCommand.Status = ChargeStatus.Unpaid;
                    }


                     chargeResponse = await _chargeService.CreateAsync(chargeCommand);
                }


                PaymentResponseDto? paymentResponse = null;
                if (registrationRequestDto.PaymentRegReq != null)
                {
                    var paymentCommand = new PaymentCommand
                    {
                        EnrollmentId = enrollmentResponse.Id,
                        Amount = registrationRequestDto.PaymentRegReq.Amount,
                        TransferFees = registrationRequestDto.PaymentRegReq.TransferFees,
                        Method = registrationRequestDto.PaymentRegReq.Method,
                        ExternalReferenceCode = registrationRequestDto.PaymentRegReq.ExternalReferenceCode,
                        MethodDetailsJson = registrationRequestDto.PaymentRegReq.MethodDetailsJson ?? "{}",
                        BranchId = _currentUserContext.BranchId,
                        ReceivedByStaffId = _currentUserContext.UserId
                    };

                    paymentResponse = await _paymentService.CreateAsync(paymentCommand);
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
