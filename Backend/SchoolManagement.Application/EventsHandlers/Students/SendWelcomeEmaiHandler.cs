using MediatR;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Entities.EnrollmentAggregate;
using SchoolManagement.Domain.Interfaces.Repositories;

namespace SchoolManagement.Application.EventsHandlers.Students;
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

