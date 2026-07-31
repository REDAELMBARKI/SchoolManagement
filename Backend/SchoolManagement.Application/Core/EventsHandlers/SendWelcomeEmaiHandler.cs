using MediatR;
using SchoolManagement.Domain.Core.DomainEvents;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Academic.Interfaces;
using SchoolManagement.Domain.Core.Interfaces;
using SchoolManagement.Domain.Common.Interfaces;

namespace SchoolManagement.Application.Core.EventsHandlers;
    public class SendWelcomeEmaiHandler : INotificationHandler<NewStudentAssignedDomainEvent>
    { 
        readonly IStudentRepository _studentRepository;
        readonly IEnrollmentRepository _enrollmentRepository;
        public SendWelcomeEmaiHandler(IStudentRepository studentRepository , IEnrollmentRepository enrollmentRepository) 
        {
           _enrollmentRepository = enrollmentRepository;
           _studentRepository = studentRepository;
        }
        public async Task Handle(NewStudentAssignedDomainEvent e, CancellationToken cancellationToken)
        {
            Student student = (await _studentRepository.GetByIdAsync(e.StudentId))!;
            Enrollment enrollment = (await _enrollmentRepository.GetByIdAsync(e.EnrollmentId))!;
            
           // mail server later 
        }
}

