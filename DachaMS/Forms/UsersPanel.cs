using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;

namespace DachaMS
{
    public partial class UsersPanel : BasePanel
    {
        private FlowLayoutPanel flow;
        private Button btnRefresh;

        public override string PanelTitle
        {
            get { return "Пользователи"; }
        }

        public UsersPanel()
        {
            InitializeComponent();
            ResizeFlow();
            LoadUsers();
            btnRefresh.Click += (s, e) => LoadUsers();
        }

        private void InitializeComponent()
        {
            btnRefresh = new Button();
            btnRefresh.Text = "Обновить";
            btnRefresh.Location = new Point(10, 10);
            btnRefresh.Width = 120;
            btnRefresh.Height = 30;

            flow = new FlowLayoutPanel();
            flow.Location = new Point(0, 50);
            flow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flow.AutoScroll = true;
            flow.WrapContents = true;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.Padding = new Padding(10);

            Controls.Add(btnRefresh);
            Controls.Add(flow);

            //this.Resize += (s, e) => ResizeFlow();
        }

        private void ResizeFlow()
        {
            flow.Size = new Size(this.Width, this.Height - 50);
        }

        private void LoadUsers()
        {
            flow.Controls.Clear();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT user_id, full_name, login, role
                    FROM users
                    ORDER BY full_name", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    UserCard card = new UserCard(
                        Convert.ToInt32(reader["user_id"]),
                        reader["full_name"].ToString(),
                        reader["login"].ToString(),
                        reader["role"].ToString());

                    card.DataChanged += LoadUsers;
                    flow.Controls.Add(card);
                }
            }
        }
    }
}
