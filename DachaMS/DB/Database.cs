using System.Data.SqlClient;

namespace DachaMS.DB
{
    public static class Database
    {
        // Измените Server= под своё имя (например DESKTOP-XXX\SQLEXPRESS)
        private static readonly string ConnectionString =
            "Server=DESKTOP-2M5PL2B\\SQLEXPRESS;Database=DachaMS;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}