using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class UserCard : UserControl
    {
        private readonly int userId;

        public event Action DataChanged;

        public UserCard(
            int userId,
            string fullName,
            string login,
            string role)
        {
            this.userId = userId;

            Width = 350;
            Height = 150;
            BorderStyle = BorderStyle.FixedSingle;

            Label lblName = new Label();
            lblName.Text = fullName;
            lblName.Location = new Point(10, 10);
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            Label lblLogin = new Label();
            lblLogin.Text = "Логин: " + login;
            lblLogin.Location = new Point(10, 45);
            lblLogin.AutoSize = true;

            // Пометка «Вы» для текущего пользователя
            if (userId == Session.UserId)
            {
                Label lblSelf = new Label();
                lblSelf.Text = "(вы)";
                lblSelf.ForeColor = Color.Gray;
                lblSelf.Location = new Point(10, 68);
                lblSelf.AutoSize = true;
                Controls.Add(lblSelf);
            }

            ComboBox cmbRole = new ComboBox();
            cmbRole.Location = new Point(10, 90);
            cmbRole.Width = 150;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Items.Add("viewer");
            cmbRole.Items.Add("moderator");
            cmbRole.Items.Add("admin");
            cmbRole.SelectedItem = role;

            // Нельзя менять роль собственного аккаунта
            if (userId == Session.UserId)
                cmbRole.Enabled = false;

            Button btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(175, 87);
            btnSave.Width = 120;
            btnSave.Enabled = userId != Session.UserId;

            btnSave.Click += (s, e) =>
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE users
                        SET role=@role
                        WHERE user_id=@id",
                        conn);

                    cmd.Parameters.AddWithValue("@role", cmbRole.Text);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }

                DataChanged?.Invoke();
            };

            Controls.Add(lblName);
            Controls.Add(lblLogin);
            Controls.Add(cmbRole);
            Controls.Add(btnSave);
        }
    }
}
