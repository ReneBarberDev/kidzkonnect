using System;

namespace kidzkonnect2._0.Models
{
    public class SearchResult
    {
        // Existing properties
        public int IdProfil { get; set; }
        public int IdUtilisateur { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsRequestPending { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public bool IsAlreadyFriend { get; set; }
    }

    public class RechercheViewModel
    {
        public List<SearchResult> Results { get; set; } = new List<SearchResult>();
    }
}
