namespace SignalR.API.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }

        public Room Room { get; set; }
    }
}
