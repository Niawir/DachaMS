using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class OwnersPanel : BasePanel
    {
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnAdd;
        private FlowLayoutPanel flow;

        public override string PanelTitle
        {
            get { return "Владельцы"; }
        }

        public OwnersPanel()
        {
            InitializeComponent();
            ResizeFlow();
            LoadOwners();
            btnSearch.Click += (s, e) => LoadOwners();
            btnAdd.Click += BtnAdd_Click;

            if (!Session.CanEdit)
                btnAdd.Visible = false;
        }

        private void InitializeComponent()
        {
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(10, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(270, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 30);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Найти";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(360, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(160, 30);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Добавить владельца";
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
            this.flow.Size = new System.Drawing.Size(587, 217);
            this.flow.TabIndex = 3;
            // 
            // OwnersPanel
            // 
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.flow);
            this.Name = "OwnersPanel";
            this.Size = new System.Drawing.Size(537, 267);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ResizeFlow()
        {
            flow.Size = new Size(this.Width, this.Height - 50);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new OwnerEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
                LoadOwners();
        }

        private void LoadOwners()
        {
            flow.Controls.Clear();

            string search = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(search))
            {
                foreach (char c in search)
                {
                    bool isRussianLetter =
                        (c >= 'А' && c <= 'я') ||
                        c == 'Ё' ||
                        c == 'ё';

                    if (!isRussianLetter &&
                        !char.IsWhiteSpace(c) &&
                        c != '-')
                    {
                        MessageBox.Show(
                            "Поиск владельцев может содержать только русские буквы, пробелы и дефис.",
                            "Ошибка ввода",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtSearch.Focus();
                        return;
                    }
                }
            }

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT owner_id, full_name, phone, email
        FROM owners
        WHERE full_name LIKE @search
        ORDER BY full_name", conn);

                cmd.Parameters.AddWithValue(
                    "@search",
                    "%" + search + "%");

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    OwnerCard card = new OwnerCard(
                        Convert.ToInt32(reader["owner_id"]),
                        reader["full_name"].ToString(),
                        reader["phone"].ToString(),
                        reader["email"].ToString());

                    card.DataChanged += LoadOwners;
                    flow.Controls.Add(card);
                }
            }

            if (flow.Controls.Count == 0)
            {
                MessageBox.Show(
                    "По вашему запросу ничего не найдено.",
                    "Поиск",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
