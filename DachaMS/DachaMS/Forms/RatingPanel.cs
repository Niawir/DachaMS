using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;

namespace DachaMS
{
    public partial class RatingPanel : BasePanel
    {
        private FlowLayoutPanel flow;
        private Button btnRefresh;

        public override string PanelTitle
        {
            get { return "Рейтинг участков"; }
        }

        public RatingPanel()
        {
            InitializeComponent();
            ResizeFlow();
            LoadRating();
            btnRefresh.Click += (s, e) => LoadRating();
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
            flow.Dock = DockStyle.None;
            flow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flow.AutoScroll = false;
            flow.WrapContents = true;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.Padding = new Padding(10);

            Controls.Add(btnRefresh);
            Controls.Add(flow);

            this.Resize += (s, e) =>
            {
                ResizeFlow();
                UpdateFlowScroll();
            };
            flow.Resize += (s, e) => UpdateFlowScroll();
        }

        private void ResizeFlow()
        {
            flow.Size = new Size(Width, Height - 50);
        }

        private void UpdateFlowScroll()
        {
            BasePanel.UpdateFlowScroll(flow);
        }

        private void LoadRating()
        {
            flow.Controls.Clear();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
    SELECT 
        p.plot_id, 
        p.street_num, 
        p.plot_num, 
        p.area_sotki, 
        o.full_name, 
        [dbo].[calculate_price](p.plot_id) AS price, 
        [dbo].[calculate_rating](p.plot_id) AS rating 
    FROM [plots] p 
    LEFT JOIN [owners] o ON o.owner_id = p.owner_id 
    ORDER BY rating DESC, price DESC", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                int place = 1;

                while (reader.Read())
                {
                    RatingCard card = new RatingCard(
                        place,
                        Convert.ToInt32(reader["plot_id"]),
                        reader["street_num"].ToString(),
                        reader["plot_num"].ToString(),
                        reader["area_sotki"].ToString(),
                        reader["full_name"].ToString(),
                        Convert.ToDecimal(reader["price"]),
                        Convert.ToInt32(reader["rating"]));

                    flow.Controls.Add(card);
                    place++;
                }
            }

            flow.PerformLayout();
            UpdateFlowScroll();
        }
    }
}
