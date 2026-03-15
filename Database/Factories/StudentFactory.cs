using SchoolManagement.Models;

namespace SchoolManagement.Database.Factories ; 

public class StudentFactory : UserFactory<Student>
{
    protected override Student Create()
    {
        return new Student
        {
            DateOfBirth = faker.Date.Past(18, DateTime.Now.AddYears(-6)),  // between 6 and 18 years old
            Gender      = faker.PickRandom("Male", "Female"),
            NationalId  = faker.Random.AlphaNumeric(10).ToUpper(),
            PhotoUrl    = faker.Internet.Avatar(),
            GroupId     = faker.Random.Int(1, 10)
        } ; 
    }

}