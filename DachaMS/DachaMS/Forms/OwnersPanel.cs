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
            txtSearch = new TextBox();
            txtSearch.Location = new Point(10, 12);
            txtSearch.Width = 250;
            txtSearch.Height = 28;

            btnSearch = new Button();
            btnSearch.Text = "Найти";
            btnSearch.Location = new Point(270, 10);
            btnSearch.Width = 80;
            btnSearch.Height = 30;

            btnAdd = new Button();
            btnAdd.Text = "Добавить владельца";
            btnAdd.Location = new Point(360, 10);
            btnAdd.Width = 160;
            btnAdd.Height = 30;

            flow = new FlowLayoutPanel();
            flow.Location = new Point(0, 50);
            flow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flow.AutoScroll = true;
            flow.WrapContents = true;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.Padding = new Padding(10);

            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
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
