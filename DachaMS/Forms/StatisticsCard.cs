using System.Drawing;
using System.Windows.Forms;

namespace DachaMS
{
    public partial class StatisticsCard : UserControl
    {
        public StatisticsCard(
            string title,
            string value)
        {
            Width = 280;
            Height = 150;

            Margin = new Padding(10);

            BorderStyle =
                BorderStyle.FixedSingle;

            Label lblTitle =
                new Label();

            lblTitle.Text = title;

            lblTitle.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold);

            lblTitle.Location =
                new Point(10, 15);

            lblTitle.AutoSize = true;

            Label lblValue =
                new Label();

            lblValue.Text = value;

            lblValue.Font =
                new Font(
                    "Segoe UI",
                    12,
                    FontStyle.Regular);

            lblValue.Location =
                new Point(10, 60);

            lblValue.Size =
                new Size(
                    250,
                    70);

            Controls.Add(lblTitle);
            Controls.Add(lblValue);
        }
    }
}