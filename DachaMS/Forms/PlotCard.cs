using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class PlotCard : UserControl
    {
        private readonly int plotId;

        public event Action<int> EditRequested;
        public event Action<int> DeleteRequested;

        public PlotCard(int plotId, SqlDataReader row)
        {
            this.plotId = plotId;

            Width = 330;
            Height = 190;
            Margin = new Padding(5);
            BorderStyle = BorderStyle.FixedSingle;

            Label lblTitle = new Label();
            lblTitle.Text =
                "Улица " + row["street_num"] +
                ", участок " + row["plot_num"];
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.Location = new Point(10, 10);
            lblTitle.AutoSize = true;

            Label lblOwner = new Label();
            lblOwner.Text =
                "Владелец: " +
                (row["full_name"] == DBNull.Value
                    ? "не назначен"
                    : row["full_name"].ToString());
            lblOwner.Location = new Point(10, 40);
            lblOwner.AutoSize = true;

            Label lblArea = new Label();
            lblArea.Text = "Площадь: " + row["area_sotki"] + " сот.";
            lblArea.Location = new Point(10, 65);
            lblArea.AutoSize = true;

            Label lblPrice = new Label();
            lblPrice.Text =
                "Стоимость: " +
                Convert.ToDecimal(row["price"]).ToString("N0") + " ₽";
            lblPrice.Location = new Point(10, 90);
            lblPrice.AutoSize = true;

            Label lblRating = new Label();
            lblRating.Text = "Рейтинг: " + row["rating"];
            lblRating.Location = new Point(10, 115);
            lblRating.AutoSize = true;

            Controls.Add(lblTitle);
            Controls.Add(lblOwner);
            Controls.Add(lblArea);
            Controls.Add(lblPrice);
            Controls.Add(lblRating);

            // Кнопки видны модератору и администратору
            if (Session.CanEdit)
            {
                Button btnEdit = new Button();
                btnEdit.Text = "Изменить";
                btnEdit.Location = new Point(190, 60);
                btnEdit.Width = 110;
                btnEdit.Click += (s, e) => EditRequested?.Invoke(plotId);

                Button btnDelete = new Button();
                btnDelete.Text = "Удалить";
                btnDelete.Location = new Point(190, 95);
                btnDelete.Width = 110;
                btnDelete.Click += (s, e) => DeleteRequested?.Invoke(plotId);

                Controls.Add(btnEdit);
                Controls.Add(btnDelete);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "PlotCard";
            this.Load += new System.EventHandler(this.PlotCard_Load);
            this.ResumeLayout(false);
        }

        private void PlotCard_Load(object sender, EventArgs e)
        {
        }
    }
}
