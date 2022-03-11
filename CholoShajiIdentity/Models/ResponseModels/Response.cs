using System.Collections.Generic;

namespace CholoShajiIdentity.Models.ResponseModels
{
    public class Response
    {
        public List<string> Messages  { get; set; }
        public bool IsSuccessful { get; set; }
        public Dictionary<string,string> AdditionalCredentialInformation { get; set; }

    }
}
