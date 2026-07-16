using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class OwnerCard : UserControl
    {
        private int ownerId;

        public event Action DataChanged;

        public OwnerCard(
            int ownerId,
            string fullName,
            string phone,
            string email)
        {
            this.ownerId = ownerId;

            Width = 350;
            Height = 170;
            BorderStyle = BorderStyle.FixedSingle;

            Label lblName = new Label();
            lblName.Text = fullName;
            lblName.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblName.Location = new Point(10, 10);
            lblName.AutoSize = true;

            Label lblPhone = new Label();
            lblPhone.Text = "Телефон: " + phone;
            lblPhone.Location = new Point(10, 50);
            lblPhone.AutoSize = true;

            Label lblEmail = new Label();
            lblEmail.Text = "Email: " + email;
            lblEmail.Location = new Point(10, 75);
            lblEmail.AutoSize = true;

            Controls.Add(lblName);
            Controls.Add(lblPhone);
            Controls.Add(lblEmail);

            // Кнопки видны только модератору и администратору
            if (Session.CanEdit)
            {
                Button btnEdit = new Button();
                btnEdit.Text = "Изменить";
                btnEdit.Location = new Point(220, 20);
                btnEdit.Width = 100;
                btnEdit.Click += BtnEdit_Click;

                Button btnDelete = new Button();
                btnDelete.Text = "Удалить";
                btnDelete.Location = new Point(220, 60);
                btnDelete.Width = 100;
                btnDelete.Click += BtnDelete_Click;

                Controls.Add(btnEdit);
                Controls.Add(btnDelete);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            OwnerEditForm form = new OwnerEditForm(ownerId);
            if (form.ShowDialog() == DialogResult.OK)
                DataChanged?.Invoke();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Удалить владельца?",
                "Подтверждение",
                MessageBoxButtons.YesNo)
                != DialogResult.Yes)
                return;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM owners WHERE owner_id=@id",
                    conn);

                cmd.Parameters.AddWithValue("@id", ownerId);
                cmd.ExecuteNonQuery();
            }

            DataChanged?.Invoke();
        }
    }
}
