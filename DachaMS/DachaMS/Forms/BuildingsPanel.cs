using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class BuildingsPanel : BasePanel
    {
        private Button btnAdd;
        private FlowLayoutPanel flow;

        public override string PanelTitle
        {
            get { return "Постройки"; }
        }

        public BuildingsPanel()
        {
            InitializeComponent();
            ResizeFlow();
            LoadBuildings();
            btnAdd.Click += BtnAdd_Click;

            if (!Session.CanEdit)
                btnAdd.Visible = false;
        }

        private void InitializeComponent()
        {
            btnAdd = new Button();
            btnAdd.Text = "Добавить постройку";
            btnAdd.Location = new Point(10, 10);
            btnAdd.Width = 180;
            btnAdd.Height = 30;

            flow = new FlowLayoutPanel();
            flow.Location = new Point(0, 50);
            flow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flow.AutoScroll = true;
            flow.WrapContents = true;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.Padding = new Padding(10);

            Controls.Add(btnAdd);
            Controls.Add(flow);

            this.Resize += (s, e) => ResizeFlow();
        }

        private void ResizeFlow()
        {
            flow.Size = new Size(this.Width, this.Height - 50);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new BuildingEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
                LoadBuildings();
        }

        private void LoadBuildings()
        {
            flow.Controls.Clear();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT
                        b.building_id,
                        b.building_name,
                        ISNULL(bp.price_bonus,0) price_bonus,
                        ISNULL(br.rating_bonus,0) rating_bonus
                    FROM buildings b
                    LEFT JOIN building_price_bonus bp ON bp.building_id=b.building_id
                    LEFT JOIN building_rating_bonus br ON br.building_id=b.building_id
                    ORDER BY b.building_name", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    BuildingCard card = new BuildingCard(
                        Convert.ToInt32(reader["building_id"]),
                        reader["building_name"].ToString(),
                        Convert.ToDecimal(reader["price_bonus"]),
                        Convert.ToInt32(reader["rating_bonus"]));

                    card.DataChanged += LoadBuildings;
                    flow.Controls.Add(card);
                }
            }
        }
    }
}
