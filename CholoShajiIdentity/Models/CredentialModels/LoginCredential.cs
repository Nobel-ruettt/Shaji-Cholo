namespace CholoShajiIdentity.Models.CredentialModels
{
    public class LoginCredential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public bool RememberMe { get; set; }

    }
}
