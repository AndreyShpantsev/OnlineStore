using System.Collections.Generic;

namespace DataAccessLogic.DatabaseModels
{
    public class SavedList
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<Antiques> Antiques { get; set; }
    }
}