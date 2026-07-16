namespace DachaMS.Models
{
    public static class Session
    {
        public static int UserId { get; set; }
        public static string FullName { get; set; }
        public static string Login { get; set; }
        public static string Role { get; set; }

        public static bool IsAdmin => Role == "admin";
        public static bool IsModerator => Role == "moderator";
        public static bool IsViewer => Role == "viewer";

        // Может добавлять / редактировать / удалять данные (admin + moderator)
        public static bool CanEdit => Role == "admin" || Role == "moderator";

        public static void Clear()
        {
            UserId = 0; FullName = ""; Login = ""; Role = "";
        }
    }
}
