using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class OwnerPlotsPanel : BasePanel
    {
        private ComboBox cmbOwners;

        private ListBox lstOwnerPlots;
        private ListBox lstFreePlots;

        private Button btnAssign;
        private Button btnUnassign;
        private Button btnRefresh;

        public override string PanelTitle
        {
            get { return "Назначение участков"; }
        }

        public OwnerPlotsPanel()
        {
            InitializeComponent();

            LoadOwners();
            LoadPlots();
            cmbOwners.SelectedIndexChanged += (s, e) => LoadPlots();
            btnAssign.Click += BtnAssign_Click;
            btnUnassign.Click += BtnUnassign_Click;
            btnRefresh.Click += (s, e) => { LoadOwners(); LoadPlots(); };

            // Viewer не может назначать и снимать участки
            if (!Session.CanEdit)
            {
                btnAssign.Enabled = false;
                btnUnassign.Enabled = false;
            }
        }

        private void InitializeComponent()
        {
            this.lblOwner = new System.Windows.Forms.Label();
            this.cmbOwners = new System.Windows.Forms.ComboBox();
            this.lblOwnerPlots = new System.Windows.Forms.Label();
            this.lstOwnerPlots = new System.Windows.Forms.ListBox();
            this.lblFreePlots = new System.Windows.Forms.Label();
            this.lstFreePlots = new System.Windows.Forms.ListBox();
            this.btnAssign = new System.Windows.Forms.Button();
            this.btnUnassign = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblOwner
            // 
            this.lblOwner.Location = new System.Drawing.Point(20, 20);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(100, 23);
            this.lblOwner.TabIndex = 0;
            this.lblOwner.Text = "Владелец";
            // 
            // cmbOwners
            // 
            this.cmbOwners.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOwners.Location = new System.Drawing.Point(20, 45);
            this.cmbOwners.Name = "cmbOwners";
            this.cmbOwners.Size = new System.Drawing.Size(400, 21);
            this.cmbOwners.TabIndex = 1;
            // 
            // lblOwnerPlots
            // 
            this.lblOwnerPlots.Location = new System.Drawing.Point(20, 90);
            this.lblOwnerPlots.Name = "lblOwnerPlots";
            this.lblOwnerPlots.Size = new System.Drawing.Size(100, 23);
            this.lblOwnerPlots.TabIndex = 2;
            this.lblOwnerPlots.Text = "Участки владельца";
            // 
            // lstOwnerPlots
            // 
            this.lstOwnerPlots.Location = new System.Drawing.Point(20, 120);
            this.lstOwnerPlots.Name = "lstOwnerPlots";
            this.lstOwnerPlots.Size = new System.Drawing.Size(400, 446);
            this.lstOwnerPlots.TabIndex = 3;
            // 
            // lblFreePlots
            // 
            this.lblFreePlots.Location = new System.Drawing.Point(650, 90);
            this.lblFreePlots.Name = "lblFreePlots";
            this.lblFreePlots.Size = new System.Drawing.Size(100, 23);
            this.lblFreePlots.TabIndex = 4;
            this.lblFreePlots.Text = "Свободные участки";
            // 
            // lstFreePlots
            // 
            this.lstFreePlots.Location = new System.Drawing.Point(650, 120);
            this.lstFreePlots.Name = "lstFreePlots";
            this.lstFreePlots.Size = new System.Drawing.Size(400, 446);
            this.lstFreePlots.TabIndex = 5;
            // 
            // btnAssign
            // 
            this.btnAssign.Location = new System.Drawing.Point(460, 220);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(150, 23);
            this.btnAssign.TabIndex = 6;
            this.btnAssign.Text = "<<< Назначить";
            // 
            // btnUnassign
            // 
            this.btnUnassign.Location = new System.Drawing.Point(460, 280);
            this.btnUnassign.Name = "btnUnassign";
            this.btnUnassign.Size = new System.Drawing.Size(150, 23);
            this.btnUnassign.TabIndex = 7;
            this.btnUnassign.Text = "Снять >>>";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(460, 340);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(150, 23);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Обновить";
            // 
            // OwnerPlotsPanel
            // 
            this.Controls.Add(this.lblOwner);
            this.Controls.Add(this.cmbOwners);
            this.Controls.Add(this.lblOwnerPlots);
            this.Controls.Add(this.lstOwnerPlots);
            this.Controls.Add(this.lblFreePlots);
            this.Controls.Add(this.lstFreePlots);
            this.Controls.Add(this.btnAssign);
            this.Controls.Add(this.btnUnassign);
            this.Controls.Add(this.btnRefresh);
            this.Name = "OwnerPlotsPanel";
            this.Size = new System.Drawing.Size(1089, 600);
            this.ResumeLayout(false);

        }

        private void LoadOwners()
        {
            cmbOwners.Items.Clear();

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    @"SELECT owner_id, full_name
                      FROM owners
                      ORDER BY full_name",
                    conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    cmbOwners.Items.Add(new ComboItem(
                        Convert.ToInt32(reader["owner_id"]),
                        reader["full_name"].ToString()));
                }
            }

            if (cmbOwners.Items.Count > 0 &&
                cmbOwners.SelectedIndex < 0)
            {
                cmbOwners.SelectedIndex = 0;
            }
        }

        private void LoadPlots()
        {
            lstOwnerPlots.Items.Clear();
            lstFreePlots.Items.Clear();

            if (cmbOwners.SelectedItem == null)
                return;

            int ownerId = ((ComboItem)cmbOwners.SelectedItem).Id;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand ownerPlots = new SqlCommand(@"
                    SELECT plot_id, street_num, plot_num
                    FROM plots
                    WHERE owner_id=@owner
                    ORDER BY street_num, plot_num",
                    conn);

                ownerPlots.Parameters.AddWithValue("@owner", ownerId);

                SqlDataReader reader = ownerPlots.ExecuteReader();

                while (reader.Read())
                {
                    lstOwnerPlots.Items.Add(new ComboItem(
                        Convert.ToInt32(reader["plot_id"]),
                        "Улица " + reader["street_num"] +
                        ", участок " + reader["plot_num"]));
                }

                reader.Close();

                SqlCommand freePlots = new SqlCommand(@"
                    SELECT plot_id, street_num, plot_num
                    FROM plots
                    WHERE owner_id IS NULL
                    ORDER BY street_num, plot_num",
                    conn);

                reader = freePlots.ExecuteReader();

                while (reader.Read())
                {
                    lstFreePlots.Items.Add(new ComboItem(
                        Convert.ToInt32(reader["plot_id"]),
                        "Улица " + reader["street_num"] +
                        ", участок " + reader["plot_num"]));
                }
            }
        }

        private void BtnAssign_Click(object sender, EventArgs e)
        {
            if (cmbOwners.SelectedItem == null) return;
            if (lstFreePlots.SelectedItem == null) return;

            int ownerId = ((ComboItem)cmbOwners.SelectedItem).Id;
            int plotId = ((ComboItem)lstFreePlots.SelectedItem).Id;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    UPDATE plots
                    SET owner_id=@owner
                    WHERE plot_id=@plot",
                    conn);

                cmd.Parameters.AddWithValue("@owner", ownerId);
                cmd.Parameters.AddWithValue("@plot", plotId);
                cmd.ExecuteNonQuery();
            }

            LoadPlots();
        }

        private void BtnUnassign_Click(object sender, EventArgs e)
        {
            if (lstOwnerPlots.SelectedItem == null) return;

            int plotId = ((ComboItem)lstOwnerPlots.SelectedItem).Id;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    UPDATE plots
                    SET owner_id = NULL
                    WHERE plot_id=@plot",
                    conn);

                cmd.Parameters.AddWithValue("@plot", plotId);
                cmd.ExecuteNonQuery();
            }

            LoadPlots();
        }
    }
}
