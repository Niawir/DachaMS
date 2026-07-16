using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class LoginForm : Form
    {
        private const int SidebarWidth = 320;
        private const int CardWidth = 380;
        private const int CardPadding = 32;

        private const int MinLoginLength = 4;
        private const int MinPasswordLength = 6;

        private static readonly Regex FullNameRegex =
            new Regex(@"^[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)?(\s[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)?){1,2}$");

        private static readonly Regex LoginRegex =
            new Regex(@"^[A-Za-z][A-Za-z0-9]*$");

        private static readonly Regex PasswordLetterRegex = new Regex(@"[A-Za-zА-Яа-яЁё]");
        private static readonly Regex PasswordDigitRegex = new Regex(@"\d");
        private static readonly Regex PasswordSpecialRegex = new Regex(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");

        private Panel sidebarPanel;
        private Panel rightPanel;
        private Panel cardShadowPanel;
        private Panel cardPanel;
        private Panel loginViewPanel;
        private Panel registerViewPanel;
        private Panel logoBadgePanel;

        private Label lblLoginUsername;
        private TextBox txtLoginUsername;
        private Label lblLoginPassword;
        private TextBox txtLoginPassword;
        private Button btnLogin;
        private Button btnGoRegister;
        private Button btnExitApp;
        private Label lblLoginError;

        private Label lblRegFullName;
        private TextBox txtRegFullName;
        private Label lblRegUsername;
        private TextBox txtRegUsername;
        private Label lblRegPassword;
        private TextBox txtRegPassword;
        private Label lblRegPassword2;
        private TextBox txtRegPassword2;
        private Button btnRegister;
        private Button btnGoLogin;
        private Label lblRegMessage;

        public LoginForm()
        {
            InitializeComponent();
            AppBranding.ApplyFormIcon(this);
            BuildSidebar();
            BuildRightArea();
            BuildLoginView();
            BuildRegisterView();
            ShowLoginView();
            this.Load += (s, e) => UpdateLayout();
            this.Resize += (s, e) => UpdateLayout();
        }

        private void InitializeComponent()
        {
            this.sidebarPanel = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.cardShadowPanel = new System.Windows.Forms.Panel();
            this.cardPanel = new System.Windows.Forms.Panel();
            this.loginViewPanel = new System.Windows.Forms.Panel();
            this.registerViewPanel = new System.Windows.Forms.Panel();
            this.rightPanel.SuspendLayout();
            this.cardPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidebarPanel
            // 
            this.sidebarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.sidebarPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebarPanel.Location = new System.Drawing.Point(0, 0);
            this.sidebarPanel.Name = "sidebarPanel";
            this.sidebarPanel.Size = new System.Drawing.Size(268, 682);
            this.sidebarPanel.TabIndex = 1;
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.White;
            this.rightPanel.Controls.Add(this.cardShadowPanel);
            this.rightPanel.Controls.Add(this.cardPanel);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(268, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(737, 682);
            this.rightPanel.TabIndex = 0;
            // 
            // cardShadowPanel
            // 
            this.cardShadowPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(215)))), ((int)(((byte)(220)))));
            this.cardShadowPanel.Location = new System.Drawing.Point(0, 0);
            this.cardShadowPanel.Name = "cardShadowPanel";
            this.cardShadowPanel.Size = new System.Drawing.Size(380, 480);
            this.cardShadowPanel.TabIndex = 0;
            // 
            // cardPanel
            // 
            this.cardPanel.BackColor = System.Drawing.Color.White;
            this.cardPanel.Controls.Add(this.loginViewPanel);
            this.cardPanel.Controls.Add(this.registerViewPanel);
            this.cardPanel.Location = new System.Drawing.Point(0, 0);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Size = new System.Drawing.Size(380, 480);
            this.cardPanel.TabIndex = 1;
            // 
            // loginViewPanel
            // 
            this.loginViewPanel.BackColor = System.Drawing.Color.White;
            this.loginViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loginViewPanel.Location = new System.Drawing.Point(0, 0);
            this.loginViewPanel.Name = "loginViewPanel";
            this.loginViewPanel.Size = new System.Drawing.Size(380, 480);
            this.loginViewPanel.TabIndex = 0;
            // 
            // registerViewPanel
            // 
            this.registerViewPanel.AutoScroll = true;
            this.registerViewPanel.BackColor = System.Drawing.Color.White;
            this.registerViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registerViewPanel.Location = new System.Drawing.Point(0, 0);
            this.registerViewPanel.Name = "registerViewPanel";
            this.registerViewPanel.Size = new System.Drawing.Size(380, 480);
            this.registerViewPanel.TabIndex = 1;
            this.registerViewPanel.Visible = false;
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(1005, 682);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DachaMS — вход";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.rightPanel.ResumeLayout(false);
            this.cardPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void BuildSidebar()
        {
            var lblTitle = new Label
            {
                Text = AppBranding.AppName.ToUpper(),
                Font = new Font("Segoe UI", 26f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(SidebarWidth - 48, 44),
                Location = new Point(32, 48)
            };

            var lblSubtitle = new Label
            {
                Text = AppBranding.AppSubtitle,
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(200, 200, 205),
                AutoSize = false,
                Size = new Size(SidebarWidth - 48, 48),
                Location = new Point(32, 96)
            };

            string[] features =
            {
                "учёт участков и построек",
                "рейтинг и ценообразование",
                "управление владельцами"
            };

            int featureY = 170;
            foreach (string feature in features)
            {
                var lblFeature = new Label
                {
                    Text = "•  " + feature,
                    Font = new Font("Segoe UI", 10.5f),
                    ForeColor = Color.FromArgb(220, 220, 225),
                    AutoSize = false,
                    Size = new Size(SidebarWidth - 48, 28),
                    Location = new Point(32, featureY)
                };
                sidebarPanel.Controls.Add(lblFeature);
                featureY += 30;
            }

            sidebarPanel.Controls.Add(lblTitle);
            sidebarPanel.Controls.Add(lblSubtitle);
        }

        private void BuildRightArea()
        {
            logoBadgePanel = new Panel
            {
                Size = new Size(72, 72),
                BackColor = Color.Transparent
            };
            logoBadgePanel.Paint += LogoBadgePanel_Paint;

            var lblLogoEmoji = new Label
            {
                Text = AppBranding.AppEmoji,
                Font = new Font("Segoe UI Emoji", 28f),
                AutoSize = false,
                Size = new Size(72, 72),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            logoBadgePanel.Controls.Add(lblLogoEmoji);
            rightPanel.Controls.Add(logoBadgePanel);
        }

        private void LogoBadgePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(AppBranding.SidebarColor, 2f))
            {
                e.Graphics.DrawEllipse(pen, 2, 2, logoBadgePanel.Width - 5, logoBadgePanel.Height - 5);
            }
        }

        private void UpdateLayout()
        {
            if (rightPanel.Width <= 0 || rightPanel.Height <= 0)
                return;

            int cardX = Math.Max(24, (rightPanel.Width - CardWidth) / 2);
            int cardY = Math.Max(60, (rightPanel.Height - cardPanel.Height) / 2);

            cardShadowPanel.Size = cardPanel.Size;
            cardShadowPanel.Location = new Point(cardX + 4, cardY + 4);
            cardPanel.Location = new Point(cardX, cardY);
            cardPanel.BringToFront();

            logoBadgePanel.Location = new Point(
                rightPanel.Width - logoBadgePanel.Width - 36,
                28);
        }

        private void BuildLoginView()
        {
            loginViewPanel.BackColor = Color.FromArgb(240, 242, 245);
            int y = CardPadding;
            int fieldWidth = CardWidth - CardPadding * 2;

            var lblTitle = CreateCardTitle("Авторизация", y);
            loginViewPanel.Controls.Add(lblTitle);
            y += 38;

            var lblDesc = CreateCardDescription(
                "Войдите для работы с базой данных дачного массива.",
                y,
                fieldWidth);
            loginViewPanel.Controls.Add(lblDesc);
            y += 44;

            lblLoginUsername = CreateFieldLabel("Логин", y);
            loginViewPanel.Controls.Add(lblLoginUsername);
            y += 22;

            txtLoginUsername = CreateTextBox(y, fieldWidth);
            loginViewPanel.Controls.Add(txtLoginUsername);
            y += 36;

            lblLoginPassword = CreateFieldLabel("Пароль", y);
            loginViewPanel.Controls.Add(lblLoginPassword);
            y += 22;

            txtLoginPassword = CreateTextBox(y, fieldWidth);
            txtLoginPassword.UseSystemPasswordChar = true;
            loginViewPanel.Controls.Add(txtLoginPassword);
            y += 44;

            btnLogin = CreatePrimaryButton("Войти", y, fieldWidth);
            btnLogin.Click += BtnLogin_Click;
            loginViewPanel.Controls.Add(btnLogin);
            y += 44;

            btnGoRegister = CreatePrimaryButton("Регистрация", y, fieldWidth);
            btnGoRegister.Click += (s, e) => ShowRegisterView();
            loginViewPanel.Controls.Add(btnGoRegister);
            y += 44;

            btnExitApp = new Button
            {
                Text = "Выйти из программы",
                Location = new Point(CardPadding, y),
                Size = new Size(fieldWidth, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = AppBranding.ErrorColor,
                Cursor = Cursors.Hand
            };
            btnExitApp.FlatAppearance.BorderColor = AppBranding.ErrorColor;
            btnExitApp.Click += (s, e) =>
            {
                if (MessageBox.Show(
                    "Вы уверены, что хотите выйти из программы?",
                    "Подтверждение выхода",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };
            loginViewPanel.Controls.Add(btnExitApp);
            y += 44;

            lblLoginError = new Label
            {
                Text = "",
                Location = new Point(CardPadding, y),
                Size = new Size(fieldWidth, 36),
                ForeColor = AppBranding.ErrorColor,
                AutoSize = false
            };
            loginViewPanel.Controls.Add(lblLoginError);

            txtLoginPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(null, null); };
            txtLoginUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtLoginPassword.Focus(); };
        }

        private void BuildRegisterView()
        {
            int y = CardPadding;
            int fieldWidth = CardWidth - CardPadding * 2;

            var lblTitle = CreateCardTitle("Регистрация", y);
            registerViewPanel.Controls.Add(lblTitle);
            y += 38;

            var lblDesc = CreateCardDescription(
                "Создайте учётную запись для доступа к системе.",
                y,
                fieldWidth);
            registerViewPanel.Controls.Add(lblDesc);
            y += 40;

            lblRegFullName = CreateFieldLabel("ФИО", y);
            registerViewPanel.Controls.Add(lblRegFullName);
            y += 22;

            txtRegFullName = CreateTextBox(y, fieldWidth);
            registerViewPanel.Controls.Add(txtRegFullName);
            y += 32;

            lblRegUsername = CreateFieldLabel("Логин", y);
            registerViewPanel.Controls.Add(lblRegUsername);
            y += 22;

            txtRegUsername = CreateTextBox(y, fieldWidth);
            registerViewPanel.Controls.Add(txtRegUsername);
            y += 32;

            lblRegPassword = CreateFieldLabel("Пароль", y);
            registerViewPanel.Controls.Add(lblRegPassword);
            y += 22;

            txtRegPassword = CreateTextBox(y, fieldWidth);
            txtRegPassword.UseSystemPasswordChar = true;
            registerViewPanel.Controls.Add(txtRegPassword);
            y += 32;

            lblRegPassword2 = CreateFieldLabel("Повторите пароль", y);
            registerViewPanel.Controls.Add(lblRegPassword2);
            y += 22;

            txtRegPassword2 = CreateTextBox(y, fieldWidth);
            txtRegPassword2.UseSystemPasswordChar = true;
            registerViewPanel.Controls.Add(txtRegPassword2);
            y += 36;

            btnRegister = CreatePrimaryButton("Зарегистрироваться", y, fieldWidth);
            btnRegister.Click += BtnRegister_Click;
            registerViewPanel.Controls.Add(btnRegister);
            y += 44;

            btnGoLogin = CreatePrimaryButton("Назад к входу", y, fieldWidth);
            btnGoLogin.Click += (s, e) => ShowLoginView();
            registerViewPanel.Controls.Add(btnGoLogin);
            y += 44;

            lblRegMessage = new Label
            {
                Text = "",
                Location = new Point(CardPadding, y),
                Size = new Size(fieldWidth, 40),
                AutoSize = false
            };
            registerViewPanel.Controls.Add(lblRegMessage);

            txtRegPassword2.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnRegister_Click(null, null); };
            txtRegPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtRegPassword2.Focus(); };
            txtRegUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtRegPassword.Focus(); };
            txtRegFullName.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtRegUsername.Focus(); };
        }

        private static Label CreateCardTitle(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = AppBranding.SidebarColor,
                AutoSize = false,
                Size = new Size(CardWidth - CardPadding * 2, 30),
                Location = new Point(CardPadding, y)
            };
        }

        private static Label CreateCardDescription(string text, int y, int width)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = AppBranding.MutedTextColor,
                AutoSize = false,
                Size = new Size(width, 36),
                Location = new Point(CardPadding, y)
            };
        }

        private static Label CreateFieldLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = AppBranding.SidebarColor,
                AutoSize = true,
                Location = new Point(CardPadding, y)
            };
        }

        private static TextBox CreateTextBox(int y, int width)
        {
            return new TextBox
            {
                Location = new Point(CardPadding, y),
                Width = width,
                Height = 28,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private static Button CreatePrimaryButton(string text, int y, int width)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(CardPadding, y),
                Width = width,
                Height = 36,
                BackColor = AppBranding.SidebarColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        public void ResetAndShow()
        {
            ShowLoginView();
            this.Show();
        }

        private void ShowLoginView()
        {
            loginViewPanel.Visible = true;
            registerViewPanel.Visible = false;
            lblLoginError.Text = "";
            txtLoginUsername.Text = "";
            txtLoginPassword.Text = "";
            cardPanel.Height = 420;
            UpdateLayout();
            txtLoginUsername.Focus();
        }

        private void ShowRegisterView()
        {
            loginViewPanel.Visible = false;
            registerViewPanel.Visible = true;
            SetRegisterMessage("", false);
            cardPanel.Height = 500;
            UpdateLayout();
            txtRegFullName.Focus();
        }

        private void SetRegisterMessage(string text, bool isError)
        {
            lblRegMessage.Text = text;
            lblRegMessage.ForeColor = isError
                ? AppBranding.ErrorColor
                : AppBranding.SuccessColor;
            lblRegMessage.BringToFront();
        }

        private static void ShowValidationError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static bool ValidateFullName(string fullName, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                error = "Поле «ФИО» не заполнено.";
                return false;
            }
            if (!FullNameRegex.IsMatch(fullName))
            {
                error = "ФИО должно быть указано на кириллице в формате «Фамилия Имя» " +
                        "или «Фамилия Имя Отчество», каждое слово — с заглавной буквы, " +
                        "без цифр, латинских символов и спецсимволов (кроме дефиса).";
                return false;
            }
            return true;
        }

        private static bool ValidateLogin(string login, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(login))
            {
                error = "Поле «Логин» не заполнено.";
                return false;
            }
            if (login.Length < MinLoginLength)
            {
                error = $"Логин должен содержать не менее {MinLoginLength} символов.";
                return false;
            }
            if (!LoginRegex.IsMatch(login))
            {
                error = "Логин может содержать только латинские буквы и цифры, " +
                        "без пробелов и спецсимволов, и должен начинаться с буквы.";
                return false;
            }
            return true;
        }

        private static bool ValidatePassword(string password, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(password))
            {
                error = "Поле «Пароль» не заполнено.";
                return false;
            }
            if (password.Contains(" "))
            {
                error = "Пароль не должен содержать пробелы.";
                return false;
            }
            if (password.Length < MinPasswordLength)
            {
                error = $"Пароль должен содержать не менее {MinPasswordLength} символов.";
                return false;
            }
            if (!PasswordLetterRegex.IsMatch(password))
            {
                error = "Пароль должен содержать хотя бы одну букву.";
                return false;
            }
            if (!PasswordDigitRegex.IsMatch(password))
            {
                error = "Пароль должен содержать хотя бы одну цифру.";
                return false;
            }
            if (!PasswordSpecialRegex.IsMatch(password))
            {
                error = "Пароль должен содержать хотя бы один спецсимвол (например: !@#$%^&*).";
                return false;
            }
            return true;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            lblLoginError.Text = "";
            string username = txtLoginUsername.Text.Trim();
            string password = txtLoginPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowValidationError("Введите логин и пароль.");
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT [user_id], [full_name], [role], [password_hash] FROM [DachaMS].[dbo].[users] WHERE [login] = @u";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                ShowValidationError("Неверный логин или пароль.");
                                return;
                            }

                            string storedPassword = reader["password_hash"].ToString().Trim();
                            if (storedPassword != password)
                            {
                                ShowValidationError("Неверный логин или пароль.");
                                return;
                            }

                            Session.UserId = (int)reader["user_id"];
                            Session.FullName = reader["full_name"].ToString();
                            Session.Role = reader["role"].ToString();
                            Session.Login = username;
                        }
                    }
                }

                var main = new MainForm(this);
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                ShowValidationError("Ошибка подключения к базе данных: " + ex.Message);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            SetRegisterMessage("", false);

            string fullName = txtRegFullName.Text.Trim();
            string username = txtRegUsername.Text.Trim();
            string password = txtRegPassword.Text;
            string password2 = txtRegPassword2.Text;

            string error;

            if (!ValidateFullName(fullName, out error))
            {
                ShowValidationError(error);
                txtRegFullName.Focus();
                return;
            }

            if (!ValidateLogin(username, out error))
            {
                ShowValidationError(error);
                txtRegUsername.Focus();
                return;
            }

            if (!ValidatePassword(password, out error))
            {
                ShowValidationError(error);
                txtRegPassword.Focus();
                return;
            }

            if (password != password2)
            {
                ShowValidationError("Пароли не совпадают.");
                txtRegPassword2.Focus();
                return;
            }

            btnRegister.Enabled = false;
            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM [DachaMS].[dbo].[users] WHERE [login] = @u";
                    using (var cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0)
                        {
                            ShowValidationError("Этот логин уже занят.");
                            txtRegUsername.Focus();
                            return;
                        }
                    }

                    string insertQuery = @"
                        INSERT INTO [DachaMS].[dbo].[users] (login, password_hash, full_name, role)
                        VALUES (@u, @p, @fn, 'viewer')";
                    using (var cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@fn", fullName);
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.ExecuteNonQuery();
                    }
                }

                SetRegisterMessage("Регистрация успешна! Войдите.", false);

                txtRegFullName.Clear();
                txtRegUsername.Clear();
                txtRegPassword.Clear();
                txtRegPassword2.Clear();

                ShowLoginView();
                txtLoginUsername.Text = username;
                txtLoginPassword.Focus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                ShowValidationError("Ошибка: " + ex.Message);
            }
            finally
            {
                btnRegister.Enabled = true;
            }
        }
    }
}