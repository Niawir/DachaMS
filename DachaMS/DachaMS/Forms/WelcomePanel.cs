using System.Drawing;
using System.Windows.Forms;

namespace DachaMS
{
    public partial class WelcomePanel : BasePanel
    {
        public override string PanelTitle
        {
            get { return "Добро пожаловать"; }
        }

        public WelcomePanel()
        {
            InitializeComponent();
            BuildUI();
        }

        private void InitializeComponent()
        {
            this.BackColor = AppBranding.ContentBackgroundColor;
            this.SuspendLayout();
            this.ResumeLayout(false);
        }

        private void BuildUI()
        {
            var pnlCenter = new Panel();
            pnlCenter.Anchor = AnchorStyles.None;
            pnlCenter.Size = new Size(480, 320);

            var lblIcon = new Label();
            lblIcon.Text = AppBranding.AppEmoji;
            lblIcon.Font = new Font("Segoe UI Emoji", 64);
            lblIcon.AutoSize = true;
            lblIcon.Location = new Point(160, 20);

            var lblTitle = new Label();
            lblTitle.Text = "DachaMS";
            lblTitle.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(45, 45, 48);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(130, 120);

            var lblSub = new Label();
            lblSub.Text = "Система управления дачным массивом";
            lblSub.Font = new Font("Segoe UI", 12);
            lblSub.ForeColor = Color.FromArgb(100, 100, 100);
            lblSub.AutoSize = true;
            lblSub.Location = new Point(60, 180);

            var lblHint = new Label();
            lblHint.Text = "Выберите раздел в меню слева";
            lblHint.Font = new Font("Segoe UI", 10);
            lblHint.ForeColor = Color.FromArgb(150, 150, 150);
            lblHint.AutoSize = true;
            lblHint.Location = new Point(125, 230);

            pnlCenter.Controls.Add(lblIcon);
            pnlCenter.Controls.Add(lblTitle);
            pnlCenter.Controls.Add(lblSub);
            pnlCenter.Controls.Add(lblHint);

            this.Controls.Add(pnlCenter);

            // Center the panel when form resizes
            this.Resize += (s, e) =>
            {
                pnlCenter.Location = new Point(
                    (this.Width - pnlCenter.Width) / 2,
                    (this.Height - pnlCenter.Height) / 2);
            };

            // Initial centering
            pnlCenter.Location = new Point(200, 100);
        }
    }
}
