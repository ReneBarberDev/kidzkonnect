namespace kidzkonnect2._0.Models
{
    public class FriendsSuggestionsViewModel
    {
        public int IdProfil { get; set; }
        public int IdUtilisateur { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> CommonInterests { get; set; }
        public List<string> CommonChallenges { get; set; }
        public bool IsRequestPending { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set; }
        public int? AvatarFk { get; set; }
        public virtual Avatar? AvatarFkNavigation { get; set; }
        // Constructeur
        public FriendsSuggestionsViewModel()
        {
            CommonInterests = new List<string>();
            CommonChallenges = new List<string>();

        }
    }
}
