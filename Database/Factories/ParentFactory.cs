using SchoolManagement.Models;

namespace SchoolManagement.Database.Factories ; 

public class ParentFactory : UserFactory<Parent>
{
    protected override Parent Create()
    {
        return new Parent
        {
            Relationship = faker.PickRandom("Father", "Mother", "Guardian", "Uncle", "Aunt") ,
            Role = Role.Parent

        } ; 
    }

}