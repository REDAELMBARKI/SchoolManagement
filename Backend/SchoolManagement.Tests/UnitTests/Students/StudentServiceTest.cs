using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using SchoolManagement.Application.Dtos.Commands;
using SchoolManagement.Application.Dtos.Requests;
using SchoolManagement.Application.Dtos.Responses;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Application.Interfaces.Services;
using SchoolManagement.Application.Services.Students;
using SchoolManagement.Domain.DomainEvents.Students;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces.Queries;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;
using Xunit;

namespace SchoolManagement.Tests.UnitTests.Students
{
    public class StudentServiceTest
    {
        private readonly Mock<IStudentRepository> _studentRepoMock;
        private readonly Mock<IStudentQueryService> _studentQueryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAuditLogService> _auditLogServiceMock;
        private readonly Mock<ICurrentUserContext> _currentUserContextMock;
        private readonly Faker _faker;
        private readonly StudentCommand _studentCommand;
        private readonly IStudentService _sut;

        public StudentServiceTest()
        {
            _studentRepoMock = new Mock<IStudentRepository>();
            _studentQueryMock = new Mock<IStudentQueryService>();
            _mediatorMock = new Mock<IMediator>();
            _auditLogServiceMock = new Mock<IAuditLogService>();
            _currentUserContextMock = new Mock<ICurrentUserContext>();

            _faker = new Faker();
            _sut = new StudentService(_studentRepoMock.Object, _studentQueryMock.Object, _mediatorMock.Object, _auditLogServiceMock.Object , _currentUserContextMock.Object);
            _studentCommand = new StudentCommand
            {
                IntakeId = Guid.NewGuid(),
                IsDirectRegistration = false,
                BranchId = Guid.NewGuid(),
                GenderId = Guid.NewGuid(),
                FirstName = "Reda",
                LastName = "Elmbarki",
                Phone = "+213-555-0100",
                Email = "reda.elmbarki@example.com",
                DateOfBirth = new DateOnly(2010, 5, 12),
                Slug = "Reda-Elmbarki"
            };
        }

        private Student RehydrateStudentFrom(StudentCommand command, Guid? id = null)
        {
            var student = Student.Register(
                firstName: command.FirstName,
                lastName: command.LastName,
                slug: command.Slug,
                genderId: command.GenderId,
                email: command.Email,
                phone: command.Phone,
                dateOfBirth: command.DateOfBirth,
                intakeId: command.IntakeId,
                isDirectRegistration: command.IsDirectRegistration,
                branchId: command.BranchId);

            if (id.HasValue)
            {
                var idProp = typeof(Domain.Common.BaseEntity).GetProperty("Id")!;
                idProp.SetValue(student, id.Value);
            }
            return student;
        }

        private StudentCommand CopyCommand(StudentCommand src) => new StudentCommand
        {
            FirstName = src.FirstName,
            LastName = src.LastName,
            Email = src.Email,
            Phone = src.Phone,
            DateOfBirth = src.DateOfBirth,
            GenderId = src.GenderId,
            LevelId = src.LevelId,
            IntakeId = src.IntakeId,
            IsDirectRegistration = src.IsDirectRegistration,
            BranchId = src.BranchId,
            Slug = src.Slug
        };

        private void SetupNoCollisions()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(It.IsAny<string?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
        }

        // ──────────────────────────── DEDUP GUARDS ────────────────────────────

        [Fact]
        public async Task CreateAsync_ThrowsDomainException_WhenPhoneAlreadyExists()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone, null)).ReturnsAsync(true);

            Func<Task> act = () => _sut.CreateAsync(_studentCommand);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("A student with this phone number already exists.");
            _studentQueryMock.Verify(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone, null), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
            _studentRepoMock.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsDomainException_WhenEmailAlreadyExists()
        {

            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone, null)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(_studentCommand.Email, null)).ReturnsAsync(true);

            Func<Task> act = () => _sut.CreateAsync(_studentCommand);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("A student with this email already exists.");
            _studentQueryMock.Verify(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone, null), Times.Once);
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(_studentCommand.Email, null), Times.Once);
            _studentRepoMock.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_SkipsEmailCheck_AndDoesNotThrow_WhenEmailIsNullOrWhitespace()
        {
            var cmd = CopyCommand(_studentCommand); 
            cmd.Email = "   ";
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(cmd.Phone, null))
                .ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(cmd.FirstName, cmd.LastName, cmd.DateOfBirth, null))
                .ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(It.IsAny<string>()))
                 .ReturnsAsync(false);
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .ReturnsAsync((Student s) => s);

            var result = await _sut.CreateAsync(cmd);

            result.Should().NotBeNull();
            _studentQueryMock.Verify(q => q.HasDuplicateByEmailAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ThrowsDomainException_WhenSameNameAndDobExists()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone, null)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(_studentCommand.Email, null)).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(
                _studentCommand.FirstName, _studentCommand.LastName, _studentCommand.DateOfBirth, null)).ReturnsAsync(true);

            Func<Task> act = () => _sut.CreateAsync(_studentCommand);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("A student with the same name and date of birth already exists.");
            _studentRepoMock.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ExecutesDedupChecks_InOrderPhoneEmailNameDob()
        {
            var order = new List<string>();
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false).Callback(() => order.Add("phone"));
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(It.IsAny<string?>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false).Callback(() => order.Add("email"));
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>()))
                .ReturnsAsync(false).Callback(() => order.Add("namedob"));
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(It.IsAny<string>()))
                .ReturnsAsync(false).Callback(() => order.Add("slug"));
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>())).ReturnsAsync((Student s) => s);

            await _sut.CreateAsync(_studentCommand);

            order.Should().ContainInOrder("phone", "email", "namedob", "slug");
        }

       
        // ──────────────────────────── ENTITY VALID STATE (Create) ────────────────────────────

        [Fact]
        public async Task CreateAsync_MappedEntity_ContainsEveryCommandAttribute_AsExpected()
        {
            SetupNoCollisions();
            Student? captured = null;
            var id = Guid.NewGuid();
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .Callback<Student>(s =>
                {
                    captured = s;
                    var idProp = typeof(Domain.Common.BaseEntity).GetProperty("Id")!;
                    idProp.SetValue(s, id);
                })
                .ReturnsAsync((Student s) => s);

            var result = await _sut.CreateAsync(_studentCommand);

            captured.Should().NotBeNull();
            captured!.FirstName.Should().Be(_studentCommand.FirstName);
            captured.LastName.Should().Be(_studentCommand.LastName);
            captured.Phone.Should().Be(_studentCommand.Phone);
            (captured.Email?.Value).Should().Be(_studentCommand.Email);
            captured.DateOfBirth.Should().Be(_studentCommand.DateOfBirth);
            captured.GenderId.Should().Be(_studentCommand.GenderId);
            captured.IntakeId.Should().Be(_studentCommand.IntakeId);
            captured.IsDirectRegistration.Should().BeFalse();
            captured.BranchId.Should().Be(_studentCommand.BranchId);
            captured.Slug.Should().Be(_studentCommand.Slug);
            captured.StudentResponsables.Should().NotBeNull().And.BeEmpty();
            captured.Enrollments.Should().NotBeNull().And.BeEmpty();
            result.Id.Should().Be(id);
            result.FirstName.Should().Be(captured.FirstName);
            result.LastName.Should().Be(captured.LastName);
            result.Phone.Should().Be(captured.Phone);
            result.Email.Should().Be(captured.Email?.Value);
            result.DateOfBirth.Should().Be(captured.DateOfBirth);
            result.GetType().GetProperty("GenderId").Should().BeNull("the response does not expose raw GenderId");
            result.IntakeId.Should().Be(captured.IntakeId);
            result.IsDirectRegistration.Should().BeFalse();
            result.BranchId.Should().Be(captured.BranchId);
            result.Slug.Should().Be(captured.Slug);
        }

        [Fact]
        public async Task CreateAsync_WithIsDirectRegistrationTrue_AndNoIntakeId_ProducesValidEntity()
        {
            var directCmd = CopyCommand(_studentCommand);
            directCmd.IntakeId = null;
            directCmd.IsDirectRegistration = true;
            SetupNoCollisions();
            Student? captured = null;
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .Callback<Student>(s => captured = s)
                .ReturnsAsync((Student s) => s);

            var result = await _sut.CreateAsync(directCmd);

            captured.Should().NotBeNull();
            captured!.IsDirectRegistration.Should().BeTrue();
            captured.IntakeId.Should().BeNull();
            result.IsDirectRegistration.Should().BeTrue();
            result.IntakeId.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_WhenEmailIsNull_EntityEmailRemainsNull_AndResponseIsEmptyString()
        {
            var cmd = CopyCommand(_studentCommand); cmd.Email = null;
            SetupNoCollisions();
            Student? captured = null;
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .Callback<Student>(s => captured = s)
                .ReturnsAsync((Student s) => s);

            var result = await _sut.CreateAsync(cmd);

            captured!.Email.Should().BeNull();
            result.Email.Should().Be(string.Empty);
        }

        // ──────────────────────────── REPO + MEDIATOR INTERACTIONS ────────────────────────────

        [Fact]
        public async Task CreateAsync_SavesEntityToRepository_AndReturnsSavedInstance()
        {
            SetupNoCollisions();
            var id = Guid.NewGuid();
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .ReturnsAsync((Student s) =>
                {
                    var idProp = typeof(Domain.Common.BaseEntity).GetProperty("Id")!;
                    idProp.SetValue(s, id);
                    return s;
                });

            var result = await _sut.CreateAsync(_studentCommand);

            result.Id.Should().Be(id);
            _studentRepoMock.Verify(r => r.AddAsync(It.Is<Student>(s =>
                s.FirstName == _studentCommand.FirstName &&
                s.LastName == _studentCommand.LastName)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_PublishesStudentCreatedDomainEvent_WithSameSavedId_AfterPersistence()
        {
            SetupNoCollisions();
            var id = Guid.NewGuid();
            StudentCreatedDomainEvent? publishedEvent = null;
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>()))
                .ReturnsAsync((Student s) =>
                {
                    var idProp = typeof(Domain.Common.BaseEntity).GetProperty("Id")!;
                    idProp.SetValue(s, id);
                    return s;
                });
            _mediatorMock.Setup(m => m.Publish(It.IsAny<StudentCreatedDomainEvent>(), It.IsAny<CancellationToken>()))
                .Callback<INotification, CancellationToken>((n, _) => publishedEvent = (StudentCreatedDomainEvent)n)
                .Returns(Task.CompletedTask);

            await _sut.CreateAsync(_studentCommand);

            publishedEvent.Should().NotBeNull();
            publishedEvent!.StudentId.Should().Be(id);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<StudentCreatedDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenDedupThrows_DoesNotSaveEntity_AndDoesNotPublishEvent()
        {
            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(_studentCommand.Phone, null)).ReturnsAsync(true);

            Func<Task> act = () => _sut.CreateAsync(_studentCommand);
            await act.Should().ThrowExactlyAsync<DomainException>();

            _studentRepoMock.Verify(r => r.AddAsync(It.IsAny<Student>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_SlugIsUniquePerGeneratedEntity_EvenForIdenticalNames()
        {
            var c1 = CopyCommand(_studentCommand);
            var c2 = CopyCommand(_studentCommand); c2.Phone = "+213-555-0200"; c2.Email = "other@example.com";

            _studentQueryMock.Setup(q => q.HasDuplicateByPhoneAsync(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByEmailAsync(It.IsAny<string?>(), It.IsAny<Guid?>())).ReturnsAsync(false);
            _studentQueryMock.Setup(q => q.HasDuplicateByNameDobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<Guid?>())).ReturnsAsync(false);

            var checkedSlugs = new List<string>();
            _studentQueryMock.Setup(q => q.IsExistsBySlugAsync(It.IsAny<string>()))
                .ReturnsAsync((string s) =>
                {
                    bool already = checkedSlugs.Contains(s);
                    checkedSlugs.Add(s);
                    return already;
                });
            _studentRepoMock.Setup(r => r.AddAsync(It.IsAny<Student>())).ReturnsAsync((Student s) => s);

            var r1 = await _sut.CreateAsync(c1);
            var r2 = await _sut.CreateAsync(c2);

            r1.Slug.Should().Be("Reda-Elmbarki");
            r2.Slug.Should().NotBeNull().And.StartWith("Reda-Elmbarki-");
            r2.Slug!.Substring("Reda-Elmbarki-".Length).Should().HaveLength(6);
        }

        // ──────────────────────────── GET BY ID ────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenQueryServiceReturnsData()
        {
            var id = Guid.NewGuid();
            var dto = new StudentResponseDto
            {
                Id = id,
                FirstName = "Amina",
                LastName = "Karimi",
                Phone = "0600000000",
                DateOfBirth = new DateOnly(2012, 2, 2)
            };
            _studentQueryMock.Setup(q => q.GetResponseByIdAsync(id)).ReturnsAsync(dto);

            var result = await _sut.GetByIdAsync(id);

            result.Should().BeSameAs(dto);
            _studentRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenQueryServiceReturnsNull()
        {
            var id = Guid.NewGuid();
            _studentQueryMock.Setup(q => q.GetResponseByIdAsync(id)).ReturnsAsync((StudentResponseDto?)null);

            Func<Task> act = () => _sut.GetByIdAsync(id);

            (await act.Should().ThrowExactlyAsync<NotFoundException>())
                .Which.Message.Should().Be($"No student found with id {id}");
        }

        // ──────────────────────────── UPDATE ────────────────────────────

        [Fact]
        public async Task UpdateAsync_ThrowsNotFoundException_WhenEntityMissing()
        {
            var id = Guid.NewGuid();
            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Student?)null);

            Func<Task> act = () => _sut.UpdateAsync(id, new UpdateStudentCommand());

            (await act.Should().ThrowExactlyAsync<NotFoundException>())
                .Which.Message.Should().Be($"No student found with id {id}");
            _studentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_AppliesEveryMutableField_FromDto_AndPersistsUpdatedEntity()
        {
            var id = Guid.NewGuid();
            var existing = RehydrateStudentFrom(_studentCommand, id);

            var dto = new UpdateStudentCommand
            {
                FirstName = "reda Edited",
                LastName = "Elmbarki Edited",
                Email = "edited@example.com",
                Phone = "+213-777-7777",
                DateOfBirth = new DateOnly(2009, 1, 9),
                GenderId = Guid.NewGuid(),
                IntakeId = null,
                IsDirectRegistration = true
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _studentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Student>()))
                .ReturnsAsync((Student s) => s);

            var result = await _sut.UpdateAsync(id, dto);

            // Entity state assertions
            existing.FirstName.Should().Be(dto.FirstName);
            existing.LastName.Should().Be(dto.LastName);
            (existing.Email?.Value).Should().Be(dto.Email);
            existing.Phone.Should().Be(dto.Phone);
            existing.DateOfBirth.Should().Be(dto.DateOfBirth);
            existing.GenderId.Should().Be(dto.GenderId);
            existing.IntakeId.Should().BeNull();
            existing.IsDirectRegistration.Should().BeTrue();
            existing.BranchId.Should().Be(_studentCommand.BranchId, "BranchId is not exposed on StudentRequestDto and must remain the original value");

            // Response assertions
            result.Id.Should().Be(id);
            result.FirstName.Should().Be(dto.FirstName);
            result.LastName.Should().Be(dto.LastName);
            result.Email.Should().Be(dto.Email);
            result.Phone.Should().Be(dto.Phone);
            result.DateOfBirth.Should().Be(dto.DateOfBirth);
            result.IntakeId.Should().BeNull();
            result.IsDirectRegistration.Should().BeTrue();

            _studentRepoMock.Verify(r => r.UpdateAsync(It.Is<Student>(s => s == existing)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DoesNotAlterIdOrSlug_OrOtherInternalFields()
        {
            var id = Guid.NewGuid();
            var existing = RehydrateStudentFrom(_studentCommand, id);
            var originalSlug = existing.Slug;
            var originalBranchId = existing.BranchId;
            var originalCreatedAt = existing.CreatedAt;
            var originalUpdatedAt = existing.UpdatedAt;

            var dto = new UpdateStudentCommand
            {
                FirstName = existing.FirstName + "!",
                LastName = existing.LastName,
                Phone = existing.Phone,
                DateOfBirth = existing.DateOfBirth,
                IntakeId = existing.IntakeId,
                IsDirectRegistration = existing.IsDirectRegistration,
                GenderId = existing.GenderId
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _studentRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Student>())).ReturnsAsync((Student s) => s);

            var result = await _sut.UpdateAsync(id, dto);

            existing.Id.Should().Be(id);
            result.Id.Should().Be(id);
            existing.Slug.Should().Be(originalSlug);
            result.Slug.Should().Be(originalSlug);
            existing.BranchId.Should().Be(originalBranchId);
            result.BranchId.Should().Be(originalBranchId);
            existing.CreatedAt.Should().Be(originalCreatedAt);
            existing.UpdatedAt.Should().Be(originalUpdatedAt, "StudentService does not set UpdatedAt; EF Core / repo typically does");
        }

        [Fact]
        public async Task UpdateAsync_ThrowsDomainException_WhenPhoneIsWhitespace()
        {
            var id = Guid.NewGuid();
            var existing = RehydrateStudentFrom(_studentCommand, id);
            var dto = new UpdateStudentCommand { FirstName = "A", LastName = "B", Phone = "   ", DateOfBirth = DateOnly.FromDateTime(DateTime.Today) };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            Func<Task> act = () => _sut.UpdateAsync(id, dto);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("Phone cannot be empty.");
            _studentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsDomainException_WhenSettingIntakeIdOnDirectRegistrationStudent()
        {
            var id = Guid.NewGuid();
            var directCmd = CopyCommand(_studentCommand);
            directCmd.IntakeId = null;
            directCmd.IsDirectRegistration = true;
            var existing = RehydrateStudentFrom(directCmd, id);
            var dto = new UpdateStudentCommand
            {
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                Phone = existing.Phone,
                DateOfBirth = existing.DateOfBirth,
                IntakeId = Guid.NewGuid(),
                IsDirectRegistration = true
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            Func<Task> act = () => _sut.UpdateAsync(id, dto);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("Cannot set IntakeId when IsDirectRegistration is true.");
            _studentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsDomainException_WhenUnsettingIntakeId_AndNotDirectRegistration()
        {
            var id = Guid.NewGuid();
            var existing = RehydrateStudentFrom(_studentCommand, id);
            var dto = new UpdateStudentCommand
            {
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                Phone = existing.Phone,
                DateOfBirth = existing.DateOfBirth,
                IntakeId = null,
                IsDirectRegistration = false
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            Func<Task> act = () => _sut.UpdateAsync(id, dto);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("Either IntakeId must be provided or IsDirectRegistration must be true.");
            _studentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsDomainException_WhenFlippingToDirectRegistration_ButIntakeIdStillSet()
        {
            var id = Guid.NewGuid();
            var existing = RehydrateStudentFrom(_studentCommand, id);
            var dto = new UpdateStudentCommand
            {
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                Phone = existing.Phone,
                DateOfBirth = existing.DateOfBirth,
                IntakeId = _studentCommand.IntakeId,
                IsDirectRegistration = true
            };

            _studentRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            Func<Task> act = () => _sut.UpdateAsync(id, dto);

            (await act.Should().ThrowExactlyAsync<DomainException>())
                .Which.Message.Should().Be("Cannot set IsDirectRegistration to true when IntakeId is provided.");
            _studentRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Student>()), Times.Never);
        }

        // ──────────────────────────── DELETE ────────────────────────────

        [Fact]
        public async Task DeleteAsync_DelegatesToRepositoryOnce()
        {
            var id = Guid.NewGuid();
            _studentRepoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            await _sut.DeleteAsync(id);

            _studentRepoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
