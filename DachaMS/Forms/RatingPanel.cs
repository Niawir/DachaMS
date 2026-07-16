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
            this.btnRefresh = new System.Windows.Forms.Button();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(10, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 30);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Обновить";
            // 
            // flow
            // 
            this.flow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flow.Location = new System.Drawing.Point(0, 50);
            this.flow.Name = "flow";
            this.flow.Padding = new System.Windows.Forms.Padding(10);
            this.flow.Size = new System.Drawing.Size(235, 127);
            this.flow.TabIndex = 1;
            // 
            // RatingPanel
            // 
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.flow);
            this.Name = "RatingPanel";
            this.Size = new System.Drawing.Size(185, 177);
            this.ResumeLayout(false);

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
