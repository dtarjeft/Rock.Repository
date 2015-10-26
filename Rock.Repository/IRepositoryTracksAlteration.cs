using System;

namespace Rock.Repository
{
    public interface IRepositoryTracksAlteration : IDataObject
    {
        int CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        string CreatedByUserName { get; set; }
        int UpdatedBy { get; set; }
        DateTime UpdatedDate { get; set; }
        string UpdatedByUserName { get; set; }
    }
}