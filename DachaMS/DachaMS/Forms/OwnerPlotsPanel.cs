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
            Label lblOwner = new Label();
            lblOwner.Text = "Владелец";
            lblOwner.Location = new Point(20, 20);

            cmbOwners = new ComboBox();
            cmbOwners.Location = new Point(20, 45);
            cmbOwners.Width = 400;
            cmbOwners.DropDownStyle = ComboBoxStyle.DropDownList;

            Label lblOwnerPlots = new Label();
            lblOwnerPlots.Text = "Участки владельца";
            lblOwnerPlots.Location = new Point(20, 90);

            lstOwnerPlots = new ListBox();
            lstOwnerPlots.Location = new Point(20, 120);
            lstOwnerPlots.Size = new Size(400, 450);

            Label lblFreePlots = new Label();
            lblFreePlots.Text = "Свободные участки";
            lblFreePlots.Location = new Point(650, 90);

            lstFreePlots = new ListBox();
            lstFreePlots.Location = new Point(650, 120);
            lstFreePlots.Size = new Size(400, 450);

            btnAssign = new Button();
            btnAssign.Text = "<<< Назначить";
            btnAssign.Location = new Point(460, 220);
            btnAssign.Width = 150;

            btnUnassign = new Button();
            btnUnassign.Text = "Снять >>>";
            btnUnassign.Location = new Point(460, 280);
            btnUnassign.Width = 150;

            btnRefresh = new Button();
            btnRefresh.Text = "Обновить";
            btnRefresh.Location = new Point(460, 340);
            btnRefresh.Width = 150;

            Controls.Add(lblOwner);
            Controls.Add(cmbOwners);
            Controls.Add(lblOwnerPlots);
            Controls.Add(lstOwnerPlots);
            Controls.Add(lblFreePlots);
            Controls.Add(lstFreePlots);
            Controls.Add(btnAssign);
            Controls.Add(btnUnassign);
            Controls.Add(btnRefresh);
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
