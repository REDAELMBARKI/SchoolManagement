using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Interfaces.Repositories;
using SchoolManagement.Domain.Interfaces.Repositories.Common;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class SubjectRepository : Repository<Subject> , ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Subject>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Subject?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Subject?> UpdateAsync(int id, Subject entity)
        {
            throw new NotImplementedException();
        }

        Task<Subject> IRepository<Subject>.AddAsync(Subject entity)
        {
            return AddAsync(entity);
        }
    }
}
