using Bogus;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Domain.Academic.Entities;
using SchoolManagement.Domain.Core.Entities;
using SchoolManagement.Domain.Common.Entities;
using SchoolManagement.Infrastructure.Academic.Repositories;
using SchoolManagement.Infrastructure.Core.Repositories;
using SchoolManagement.Infrastructure.Common.Repositories;

namespace SchoolManagement.Infrastructure.Data.Factories;

public class SubjectFactory : Factory<Subject>
{
    private readonly SubjectRepository _repository;

    public SubjectFactory(AppDbContext context, SubjectRepository repository) : base(context)
    {
        _repository = repository;
    }

    protected override async Task<Subject> Make()
    {
        string name = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian");
        var branches = Context.Branches.Select(b => b.Id).ToList();

        return Subject.Create(
            name: name,
            slug: this.GenerateSlug(name),
            description: faker.Lorem.Sentence(),
            branchId: faker.PickRandom(branches)
        );
    }
}