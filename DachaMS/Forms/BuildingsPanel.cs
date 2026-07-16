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
            this.btnAdd = new System.Windows.Forms.Button();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(10, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(180, 30);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Добавить постройку";
            // 
            // flow
            // 
            this.flow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flow.AutoScroll = true;
            this.flow.Location = new System.Drawing.Point(0, 50);
            this.flow.Name = "flow";
            this.flow.Padding = new System.Windows.Forms.Padding(10);
            this.flow.Size = new System.Drawing.Size(302, 148);
            this.flow.TabIndex = 1;
            // 
            // BuildingsPanel
            // 
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.flow);
            this.Name = "BuildingsPanel";
            this.Size = new System.Drawing.Size(252, 198);
            this.ResumeLayout(false);

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
