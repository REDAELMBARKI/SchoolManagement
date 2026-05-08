using SchoolManagement.Backend.Dtos.Requests;
using SchoolManagement.Backend.Dtos.Responses;
using SchoolManagement.Backend.Exceptions;
using SchoolManagement.Backend.Mappers;
using SchoolManagement.Backend.Models;
using SchoolManagement.Backend.Repositories;

namespace SchoolManagement.Backend.Services;

public class StudentService
{
    private readonly StudentRepository _repository;
    private readonly StudentMapper _mapper;

    public StudentService(StudentRepository repository, StudentMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await _repository.GetAllAsync();
        return students.Select(s => _mapper.ToStudentDto(s)).ToList();
    }

    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await _repository.FindByIdAsync(id);
        return _mapper.ToStudentDto(student);
    }

    public async Task<StudentResponseDto> CreateAsync(StudentRequestDto dto)
    {
        var student = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            GenderId = dto.GenderId,
            IntakeId = dto.IntakeId
        };

        return await _repository.AddAsync(student);
    }

    public async Task<StudentResponseDto> UpdateAsync(int id, StudentRequestDto dto)
    {
        var existing = await _repository.FindByIdAsync(id);

        var student = new Student
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            GenderId = dto.GenderId,
            IntakeId = dto.IntakeId
        };

        await _repository.UpdateAsync(id, student);
        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}