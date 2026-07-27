using Bogus;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Moq;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Services.Students;
using SchoolManagement.Domain.Exceptions;
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
        private StudentCommand _studentCommand { get; set; }

        private readonly IStudentService _sut;
        public StudentServiceTest() {

            _studentRepoMock = new Mock<IStudentRepository>();
            _studentQueryMock = new Mock<IStudentQueryService>();
            _mediatorMock = new Mock<IMediator>();
            _faker = new Faker();
            _sut = new StudentService(_studentRepoMock.Object , _studentQueryMock.Object , _mediatorMock.Object);
            _studentCommand = new StudentCommand
            {   
                FirstName = _faker.Name.FirstName() , 
                LastName = _faker.Name.LastName(),
                Phone = _faker.Phone.PhoneNumber() , 
                Email = _faker.Internet.Email() ,
                DateOfBirth = _faker.Date.PastDateOnly() ,
                Slug =  ""
            };
        }

        // student registration test 

        [Fact]
        public async Task Test_CreateAsync_throws_if_phone_is_duplicated()
        {
             _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone)).ReturnsAsync(true);
             var result = () => _sut.CreateAsync(_studentCommand);

             await result.Should().ThrowAsync<DomainException>();
            _studentQueryMock.Verify(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone) , Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(_studentCommand.Email) , Times.Never);
        }

        [Fact]
        public async Task Test_CreateAsync_if_email_is_duplicated()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(_studentCommand.Email)).ReturnsAsync(true);
       
            var result = () => _sut.CreateAsync(_studentCommand);

            await result.Should().ThrowAsync<DomainException>();
            _studentQueryMock.Verify(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(_studentCommand.Email), Times.Once);

        }


        [Fact]
        public async Task Test_CreateAsync_Is_Student_Duplicated_By_Fullname_And_DateOfBirth()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(_studentCommand.Email)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(_studentCommand.FirstName , _studentCommand.LastName , _studentCommand.DateOfBirth)).ReturnsAsync(true);

            var result = () => _sut.CreateAsync(_studentCommand);

            await result.Should().ThrowAsync<DomainException>();
           
            
            _studentQueryMock.Verify(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(_studentCommand.Email), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByNameDobAsync(_studentCommand.FirstName, _studentCommand.LastName , _studentCommand.DateOfBirth), Times.Once);

        }


        [Fact]
        public async Task Test_CreateAsync_Doesnot_Throw_If_BrandNewStudent()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(_studentCommand.Email)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(_studentCommand.FirstName, _studentCommand.LastName, _studentCommand.DateOfBirth)).ReturnsAsync(false);

            var result = () => _sut.CreateAsync(_studentCommand);

            await result.Should().ThrowAsync<DomainException>();


            _studentQueryMock.Verify(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(_studentCommand.Email), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByNameDobAsync(_studentCommand.FirstName, _studentCommand.LastName, _studentCommand.DateOfBirth), Times.Once);

        }



        [Fact]

        public async Task Test_Generates_Slug()
        {
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(It.IsAny<string>())).ReturnsAsync(false);
            var result = () => _sut.CreateAsync(_studentCommand);
            
        }


        [Fact]

        public async Task Test_Generates_New_Slug_If_Slug_Exists()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(_studentCommand.FirstName, _studentCommand.LastName, _studentCommand.DateOfBirth)).ReturnsAsync(false);

            var result = () => _sut.CreateAsync(_studentCommand);

        }
    }
}
