using System;
using CholoShajiCore.CoreInterfaces;

namespace CholoShajiCore.Model
{
    [Serializable]
    public abstract class ARepositoryModel : IARepositoryModel
    {
        public string Id { get; set; }
        public ARepositoryModel()
        {
            Id = Guid.NewGuid().ToString("N");
        }
    }

    /*Jawad was there*/
}
