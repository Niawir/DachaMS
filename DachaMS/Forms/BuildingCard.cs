using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class BuildingCard : UserControl
    {
        private readonly int buildingId;

        public event Action DataChanged;

        public BuildingCard(
            int buildingId,
            string name,
            decimal priceBonus,
            int ratingBonus)
        {
            this.buildingId = buildingId;

            Width = 350;
            Height = 180;
            BorderStyle = BorderStyle.FixedSingle;

            Label lblName = new Label();
            lblName.Text = name;
            lblName.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblName.Location = new Point(10, 10);
            lblName.AutoSize = true;

            Label lblPrice = new Label();
            lblPrice.Text = "Надбавка: " + priceBonus.ToString("N0") + " ₽";
            lblPrice.Location = new Point(10, 50);
            lblPrice.AutoSize = true;

            Label lblRating = new Label();
            lblRating.Text = "Рейтинг: +" + ratingBonus;
            lblRating.Location = new Point(10, 80);
            lblRating.AutoSize = true;

            Controls.Add(lblName);
            Controls.Add(lblPrice);
            Controls.Add(lblRating);

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
            BuildingEditForm form = new BuildingEditForm(buildingId);
            if (form.ShowDialog() == DialogResult.OK)
                DataChanged?.Invoke();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Удалить постройку?",
                "Подтверждение",
                MessageBoxButtons.YesNo)
                != DialogResult.Yes)
                return;

            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM buildings WHERE building_id=@id",
                    conn);

                cmd.Parameters.AddWithValue("@id", buildingId);
                cmd.ExecuteNonQuery();
            }

            DataChanged?.Invoke();
        }
    }
}
