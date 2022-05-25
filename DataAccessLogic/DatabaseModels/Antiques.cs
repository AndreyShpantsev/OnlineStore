using System;
using System.Collections.Generic;

namespace DataAccessLogic.DatabaseModels
{
    public class Antiques
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoSrc { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public Note Note { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<SavedList> SavedLists { get; set; }
    }
}