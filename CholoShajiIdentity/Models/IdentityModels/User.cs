using Microsoft.AspNetCore.Identity;

namespace CholoShajiIdentity.Models.IdentityModels
{
    public class User : IdentityUser
    {
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
