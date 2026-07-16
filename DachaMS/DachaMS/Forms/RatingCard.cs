using System.Drawing;
using System.Windows.Forms;

namespace DachaMS
{
    public partial class RatingCard : UserControl
    {
        public RatingCard(
            int place,
            int plotId,
            string street,
            string plot,
            string area,
            string owner,
            decimal price,
            int rating)
        {
            Width = 380;
            Height = 175;
            Margin = new Padding(8);

            BorderStyle = BorderStyle.FixedSingle;

            Label lblPlace = new Label();
            lblPlace.Text = "#" + place;
            lblPlace.Font =
                new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Bold);

            lblPlace.Location =
                new Point(10, 10);

            lblPlace.AutoSize = true;

            Label lblPlot = new Label();
            lblPlot.Text =
                "Улица " + street +
                ", участок " + plot;

            lblPlot.Location =
                new Point(10, 50);

            lblPlot.AutoSize = true;

            Label lblArea = new Label();
            lblArea.Text =
                "Площадь: " + area + " сот.";

            lblArea.Location =
                new Point(10, 75);

            lblArea.AutoSize = true;

            Label lblOwner = new Label();
            lblOwner.Text =
                "Владелец: " +
                (string.IsNullOrEmpty(owner)
                ? "не назначен"
                : owner);

            lblOwner.Location =
                new Point(10, 100);

            lblOwner.AutoSize = true;

            Label lblPrice = new Label();
            lblPrice.Text =
                "Стоимость: " +
                price.ToString("N0") +
                " ₽";

            lblPrice.Location =
                new Point(190, 50);

            lblPrice.AutoSize = true;

            Label lblRating = new Label();
            lblRating.Text =
                "Рейтинг: " + rating;

            lblRating.Font =
                new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold);

            lblRating.Location =
                new Point(190, 80);

            lblRating.AutoSize = true;

            Controls.Add(lblPlace);
            Controls.Add(lblPlot);
            Controls.Add(lblArea);
            Controls.Add(lblOwner);
            Controls.Add(lblPrice);
            Controls.Add(lblRating);
        }
    }
}