using SchoolManagement.Models;

namespace SchoolManagement.Database.Factories ; 

public class StudentFactory : UserFactory<Student>
{
    protected override Student Create()
    {
        return new Student
        {
            GroupId     = faker.Random.Int(1, 10) , 
        } ; 
    }

}