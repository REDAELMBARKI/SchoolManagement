using Bogus;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Moq;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Services.Students;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SchoolManagement.Tests.UnitTests.Students
{
    public  class StudentServiceTest
    {
        private readonly Mock<IStudentRepository> _studentRepoMock;
        private readonly Mock<IStudentQueryService> _studentQueryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private Faker _faker ;

        private readonly IStudentService _sut;
        public StudentServiceTest() {

            _studentRepoMock = new Mock<IStudentRepository>();
            _studentQueryMock = new Mock<IStudentQueryService>();
            _mediatorMock = new Mock<IMediator>();
            _faker = new Faker();
            _sut = new StudentService(_studentRepoMock.Object , _studentQueryMock.Object , _mediatorMock.Object);
        }


        [Fact]
        public void Ensure_throws_if_duplicat_by_phone_test() {
            StudentCommand command = new StudentCommand
            {
                Phone = _faker.Phone.PhoneNumber()
            };
            _studentQueryMock.Setup(q =>  q.HasDuplicateByPhoneAsync(command.Phone)).ReturnsAsync(true);
        
            _sut.EnsureNoDuplicateStudentAsync(command);
        }





    }
}
