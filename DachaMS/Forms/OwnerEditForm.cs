using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using System.Text.RegularExpressions;

namespace DachaMS
{
    public partial class OwnerEditForm : Form
    {
        private readonly int? ownerId;

        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtEmail;

        private Button btnSave;

        public OwnerEditForm(int? ownerId)
        {
            this.ownerId = ownerId;

            InitializeComponent();
            AppBranding.ApplySurfaceBackground(this);

            if (ownerId.HasValue)
                LoadOwner();
            btnSave.Click += BtnSave_Click;
        }

        private void InitializeComponent()
        {
            Width = 500;
            Height = 300;

            StartPosition =
                FormStartPosition.CenterParent;

            Label lblName = new Label();
            lblName.Text = "ФИО";
            lblName.Location = new Point(20, 20);

            txtName = new TextBox();
            txtName.Location = new Point(20, 45);
            txtName.Width = 420;

            Label lblPhone = new Label();
            lblPhone.Text = "Телефон";
            lblPhone.Location = new Point(20, 90);

            txtPhone = new TextBox();
            txtPhone.Location = new Point(20, 115);
            txtPhone.Width = 420;

            Label lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Location = new Point(20, 160);

            txtEmail = new TextBox();
            txtEmail.Location = new Point(20, 185);
            txtEmail.Width = 420;

            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(20, 225);
            btnSave.Width = 150;


            Controls.Add(lblName);
            Controls.Add(txtName);

            Controls.Add(lblPhone);
            Controls.Add(txtPhone);

            Controls.Add(lblEmail);
            Controls.Add(txtEmail);

            Controls.Add(btnSave);
        }

        private void LoadOwner()
        {
            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT * FROM owners WHERE owner_id=@id",
                        conn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    ownerId);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtName.Text =
                        reader["full_name"].ToString();

                    txtPhone.Text =
                        reader["phone"].ToString();

                    txtEmail.Text =
                        reader["email"].ToString();
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Введите ФИО владельца.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtName.Focus();
                return;
            }

            if (!Regex.IsMatch(name, @"^[А-Яа-яЁёA-Za-z\s\-]+$"))
            {
                MessageBox.Show(
                    "ФИО может содержать только буквы, пробелы и дефис.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtName.Focus();
                return;
            }

            if (name.Length < 5)
            {
                MessageBox.Show(
                    "ФИО должно содержать не менее 5 символов.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show(
                    "Введите номер телефона.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPhone.Focus();
                return;
            }

            if (!Regex.IsMatch(phone, @"^\+7-\d{3}-\d{3}-\d{4}$"))
            {
                MessageBox.Show(
                    "Телефон должен быть в формате +7-XXX-XXX-XXXX",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPhone.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                if (!Regex.IsMatch(
                    email,
                    @"^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
                {
                    MessageBox.Show(
                        "Email должен содержать только английские буквы и иметь корректный формат.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtEmail.Focus();
                    return;
                }
            }

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    SqlCommand cmd;

                    if (ownerId == null)
                    {
                        cmd = new SqlCommand(@"
                INSERT INTO owners
                (
                    full_name,
                    phone,
                    email
                )
                VALUES
                (
                    @name,
                    @phone,
                    @email
                )", conn);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"
                UPDATE owners
                SET
                    full_name = @name,
                    phone = @phone,
                    email = @email
                WHERE owner_id = @id", conn);

                        cmd.Parameters.AddWithValue("@id", ownerId);
                    }

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@phone", phone);

                    if (string.IsNullOrWhiteSpace(email))
                        cmd.Parameters.AddWithValue("@email", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@email", email);

                    cmd.ExecuteNonQuery();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка сохранения:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}