using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class MainForm : Form
    {
        private Panel menuPanel;
        private Panel headerPanel;
        private Panel contentPanel;

        private Label lblUser;
        private Label lblTitle;
        private Label lblAppEmoji;

        private Button btnBack;
        private Button btnLogout;
        private LoginForm _loginForm;

        private Stack<UserControl> history =
            new Stack<UserControl>();

        private UserControl currentPanel;

        public MainForm(LoginForm loginForm)
        {
            _loginForm = loginForm;
            InitializeComponent();
            AppBranding.ApplyFormIcon(this);
            AppBranding.ApplySurfaceBackground(this);

            OpenPanel(new WelcomePanel());
            btnBack.Click += BtnBack_Click;
            lblUser.Text = Session.FullName + " (" + Session.Role + ")";
            BuildMenu();
            BuildLogoutButton();
        }

        private void InitializeComponent()
        {
            this.menuPanel = new System.Windows.Forms.Panel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblAppEmoji = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // menuPanel
            //
            this.menuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.menuPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.menuPanel.Location = new System.Drawing.Point(0, 0);
            this.menuPanel.Name = "menuPanel";
            this.menuPanel.Size = new System.Drawing.Size(220, 711);
            this.menuPanel.TabIndex = 2;
            this.menuPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.menuPanel_Paint);
            //
            // headerPanel
            //
            this.headerPanel.BackColor = AppBranding.SurfaceBackgroundColor;
            this.headerPanel.Controls.Add(this.btnBack);
            this.headerPanel.Controls.Add(this.lblAppEmoji);
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblUser);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(220, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(976, 60);
            this.headerPanel.TabIndex = 1;
            //
            // btnBack
            //
            this.btnBack.Location = new System.Drawing.Point(10, 12);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 35);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "← Назад";
            //
            // lblAppEmoji
            //
            this.lblAppEmoji.AutoSize = true;
            this.lblAppEmoji.Font = new System.Drawing.Font("Segoe UI Emoji", 22F);
            this.lblAppEmoji.Location = new System.Drawing.Point(118, 10);
            this.lblAppEmoji.Name = "lblAppEmoji";
            this.lblAppEmoji.Size = new System.Drawing.Size(58, 40);
            this.lblAppEmoji.TabIndex = 3;
            this.lblAppEmoji.Text = "🏡";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(172, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 25);
            this.lblTitle.TabIndex = 1;
            //
            // lblUser
            //
            this.lblUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(1676, 20);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(0, 13);
            this.lblUser.TabIndex = 2;
            //
            // contentPanel
            //
            this.contentPanel.BackColor = AppBranding.SurfaceBackgroundColor;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(220, 60);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(976, 651);
            this.contentPanel.TabIndex = 0;
            //
            // MainForm
            //
            this.ClientSize = new System.Drawing.Size(1196, 711);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.menuPanel);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DachaMS";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private void BuildMenu()
        {
            var lblAppName = new Label();
            lblAppName.Text = "Меню";
            lblAppName.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            lblAppName.ForeColor = Color.White;
            lblAppName.AutoSize = false;
            lblAppName.Size = new Size(200, 50);
            lblAppName.Location = new Point(10, 10);
            lblAppName.TextAlign = ContentAlignment.MiddleCenter;
            menuPanel.Controls.Add(lblAppName);

            AddMenuSeparator(58);

            int top = 75;

            // ─── Доступно всем ───────────────────────────────────────
            AddMenuButton("Участки", top,
                (s, e) => OpenPanel(new PlotsPanel()));
            top += 50;

            AddMenuButton("Рейтинг", top,
                (s, e) => OpenPanel(new RatingPanel()));
            top += 50;

            // ─── Модератор + Администратор ───────────────────────────
            if (Session.CanEdit)
            {
                AddMenuSeparator(top);
                top += 12;

                AddMenuButton("Статистика", top,
                    (s, e) => OpenPanel(new StatisticsPanel()));
                top += 50;

                AddMenuButton("Владельцы", top,
                    (s, e) => OpenPanel(new OwnersPanel()));
                top += 50;

                AddMenuButton("Постройки", top,
                    (s, e) => OpenPanel(new BuildingsPanel()));
                top += 50;

                AddMenuButton("Назначение участков", top,
                    (s, e) => OpenPanel(new OwnerPlotsPanel()));
                top += 50;
            }

            // ─── Только Администратор ────────────────────────────────
            if (Session.IsAdmin)
            {
                AddMenuSeparator(top);
                top += 12;

                AddMenuButton("Правила цен", top,
                    (s, e) => OpenPanel(new PriceRulesPanel()));
                top += 50;

                AddMenuButton("Правила рейтинга", top,
                    (s, e) => OpenPanel(new RatingRulesPanel()));
                top += 50;

                AddMenuButton("Пользователи", top,
                    (s, e) => OpenPanel(new UsersPanel()));
                top += 50;
            }
        }

        private void BuildLogoutButton()
        {
            btnLogout = new Button();
            btnLogout.Text = "← Выйти из аккаунта";
            btnLogout.Width = 200;
            btnLogout.Height = 40;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.ForeColor = Color.FromArgb(255, 100, 90);
            btnLogout.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 85);
            btnLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLogout.Location = new Point(10, menuPanel.Height - 55);
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show(
                    "Вы уверены, что хотите выйти из аккаунта?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Session.Clear();
                    _loginForm.ResetAndShow();
                    this.Close();
                }
            };
            menuPanel.Controls.Add(btnLogout);
        }

        private void AddMenuButton(string text, int top, EventHandler click)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Width = 200;
            btn.Height = 40;
            btn.Location = new Point(10, top);
            btn.FlatStyle = FlatStyle.Flat;
            btn.ForeColor = Color.White;
            btn.Click += click;
            menuPanel.Controls.Add(btn);
        }

        private void AddMenuSeparator(int top)
        {
            var sep = new Panel();
            sep.BackColor = Color.FromArgb(80, 80, 85);
            sep.Location = new Point(10, top);
            sep.Size = new Size(200, 1);
            menuPanel.Controls.Add(sep);
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            if (history.Count == 0)
                return;

            UserControl panel = history.Pop();
            contentPanel.Controls.Clear();
            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
            currentPanel = panel;
            UpdateTitle();
        }

        public void OpenPanel(UserControl panel)
        {
            if (currentPanel != null)
                history.Push(currentPanel);

            contentPanel.Controls.Clear();
            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
            currentPanel = panel;
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (currentPanel is BasePanel)
                lblTitle.Text = ((BasePanel)currentPanel).PanelTitle;
            else
                lblTitle.Text = "";
        }

        private void menuPanel_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
