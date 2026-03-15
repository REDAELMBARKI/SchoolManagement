using Bogus;
using SchoolManagement.Models;

namespace SchoolManagement.Database.Factories ; 
public class LanguageFactory : Factory<Language>
{ 

     protected override Language Make(){
         return new Language
         {
            Name = faker.PickRandom("English", "French", "Spanish", "Arabic", "German", "Italian") 
         } ;

     }
}