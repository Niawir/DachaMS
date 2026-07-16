using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;
using DachaMS.Models;

namespace DachaMS
{
    public partial class PlotsPanel : BasePanel
    {
        private Label lblName;
        private TextBox txtSearch;
        private ComboBox cmbCondition;
        private ComboBox cmbSort;
        private Button btnSearch;
        private Button btnNew;
        private SplitContainer split;
        private FlowLayoutPanel flow;
        private PlotEditorPanel editor;

        public override string PanelTitle
        {
            get { return "Участки"; }
        }

        public PlotsPanel()
        {
            InitializeComponent();
            ResizeSplit();
            LoadPlots();
            btnSearch.Click += (s, e) => LoadPlots();
            btnNew.Click += BtnNew_Click;
            editor.Saved += () => LoadPlots(false);
            cmbSort.SelectedIndexChanged += (s, e) => LoadPlots();
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    LoadPlots();
            };

            // Viewer: скрываем кнопку добавления и правую панель редактора
            if (!Session.CanEdit)
            {
                btnNew.Visible = false;
                split.Panel2Collapsed = true;
                split.IsSplitterFixed = true;
            }
        }

        private void InitializeComponent()
        {
            lblName = new Label();
            lblName.Text = "Имя:";
            lblName.Location = new Point(10, 14);
            lblName.AutoSize = true;

            txtSearch = new TextBox();
            txtSearch.Location = new Point(50, 10);
            txtSearch.Width = 180;
            txtSearch.Height = 28;

            cmbCondition = new ComboBox();
            cmbCondition.Location = new Point(240, 10);
            cmbCondition.Width = 150;
            cmbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCondition.Items.Add("Все");
            cmbCondition.Items.Add("облагорожен");
            cmbCondition.Items.Add("заброшен");
            cmbCondition.SelectedIndex = 0;

            cmbSort = new ComboBox();
            cmbSort.Location = new Point(400, 10);
            cmbSort.Width = 150;
            cmbSort.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSort.Items.Add("Цена ↑");
            cmbSort.Items.Add("Цена ↓");
            cmbSort.SelectedIndex = 0;

            btnSearch = new Button();
            btnSearch.Text = "Поиск";
            btnSearch.Location = new Point(560, 9);
            btnSearch.Width = 80;
            btnSearch.Height = 30;

            btnNew = new Button();
            btnNew.Text = "Новый участок";
            btnNew.Location = new Point(650, 9);
            btnNew.Width = 130;
            btnNew.Height = 30;

            split = new SplitContainer();
            split.Location = new Point(0, 50);
            split.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            split.SplitterDistance = 700;

            flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.AutoScroll = false;
            flow.WrapContents = true;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.Padding = new Padding(5);

            editor = new PlotEditorPanel();
            editor.Dock = DockStyle.None;
            editor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var editorScroll = new Panel();
            editorScroll.Dock = DockStyle.Fill;
            editorScroll.AutoScroll = true;
            editorScroll.Controls.Add(editor);
            editorScroll.Resize += (s, e) =>
            {
                editor.Width = editorScroll.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            };

            split.Panel1.Controls.Add(flow);
            split.Panel2.Controls.Add(editorScroll);

            Controls.Add(lblName);
            Controls.Add(txtSearch);
            Controls.Add(cmbCondition);
            Controls.Add(cmbSort);
            Controls.Add(btnSearch);
            Controls.Add(btnNew);
            Controls.Add(split);

            this.Resize += (s, e) =>
            {
                ResizeSplit();
                UpdateFlowScroll();
            };
            flow.Resize += (s, e) => UpdateFlowScroll();
        }

        private void ResizeSplit()
        {
            split.Size = new Size(Width, Height - 50);
        }

        private void UpdateFlowScroll()
        {
            BasePanel.UpdateFlowScroll(flow);
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            editor.LoadPlot(null);
        }

        private void LoadPlots(bool showNotFoundMessage = false)
        {
            flow.Controls.Clear();

            string name = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(name))
            {
                foreach (char c in name)
                {
                    bool isCyrillic = (c >= 'А' && c <= 'я') || c == 'Ё' || c == 'ё';
                    if (!isCyrillic && c != '-')
                    {
                        MessageBox.Show(
                            "Поиск по владельцу может содержать только кириллицу и дефис.",
                            "Ошибка ввода",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        txtSearch.Focus();
                        return;
                    }
                }
            }

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    string sql = @"
            USE [DachaMS];

            SELECT
                p.plot_id,
                p.street_num,
                p.plot_num,
                p.area_sotki,
                p.electricity,
                p.condition,
                o.full_name,
                [dbo].[calculate_price](p.plot_id) AS price,
                [dbo].[calculate_rating](p.plot_id) AS rating
            FROM [dbo].[plots] p
            LEFT JOIN [dbo].[owners] o
                ON o.owner_id = p.owner_id
            WHERE 1 = 1";

                    if (cmbCondition.Text != "Все")
                        sql += " AND p.condition = @condition";

                    if (!string.IsNullOrEmpty(name))
                        sql += " AND o.full_name LIKE @name";

                    string order =
                        cmbSort.SelectedIndex == 1
                        ? " DESC"
                        : " ASC";

                    sql += " ORDER BY [dbo].[calculate_price](p.plot_id)" + order;

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (cmbCondition.Text != "Все")
                            cmd.Parameters.AddWithValue("@condition", cmbCondition.Text);

                        if (!string.IsNullOrEmpty(name))
                            cmd.Parameters.AddWithValue("@name", "%" + name + "%");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlotCard card = new PlotCard(
                                    Convert.ToInt32(reader["plot_id"]),
                                    reader);

                                card.EditRequested += OpenPlot;
                                card.DeleteRequested += DeletePlot;
                                flow.Controls.Add(card);
                            }
                        }
                    }
                }

                flow.PerformLayout();
                UpdateFlowScroll();

                if (showNotFoundMessage && flow.Controls.Count == 0)
                {
                    MessageBox.Show(
                        "По вашему запросу ничего не найдено.",
                        "Поиск",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки участков:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OpenPlot(int plotId)
        {
            if (!Session.CanEdit)
                return;

            editor.LoadPlot(plotId);
        }

        private void DeletePlot(int plotId)
        {
            if (!Session.CanEdit)
                return;

            if (MessageBox.Show(
                "Удалить участок?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM [DachaMS].[dbo].[plot_buildings] WHERE plot_id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", plotId);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM [DachaMS].[dbo].[plots] WHERE plot_id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", plotId);
                        cmd.ExecuteNonQuery();
                    }
                }

                editor.LoadPlot(null);
                LoadPlots();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось удалить участок: " + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
