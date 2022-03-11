namespace CholoShajiIdentity.Models.CredentialModels
{
    public class ConfirmEmailCredential
    {
        public string UserName { get; set; }
        public string EmailConfirmationToken { get; set; }

    }
}
