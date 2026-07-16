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
            this.lblName = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cmbCondition = new System.Windows.Forms.ComboBox();
            this.cmbSort = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.split = new System.Windows.Forms.SplitContainer();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.editorScroll = new System.Windows.Forms.Panel();
            this.editor = new DachaMS.PlotEditorPanel();
            ((System.ComponentModel.ISupportInitialize)(this.split)).BeginInit();
            this.split.Panel1.SuspendLayout();
            this.split.Panel2.SuspendLayout();
            this.split.SuspendLayout();
            this.editorScroll.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(10, 14);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(32, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Имя:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(50, 10);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // cmbCondition
            // 
            this.cmbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCondition.Items.AddRange(new object[] {
            "Все",
            "облагорожен",
            "заброшен"});
            this.cmbCondition.Location = new System.Drawing.Point(240, 10);
            this.cmbCondition.Name = "cmbCondition";
            this.cmbCondition.Size = new System.Drawing.Size(150, 21);
            this.cmbCondition.TabIndex = 2;
            // 
            // cmbSort
            // 
            this.cmbSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSort.Items.AddRange(new object[] {
            "Цена ↑",
            "Цена ↓"});
            this.cmbSort.Location = new System.Drawing.Point(400, 10);
            this.cmbSort.Name = "cmbSort";
            this.cmbSort.Size = new System.Drawing.Size(150, 21);
            this.cmbSort.TabIndex = 3;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(560, 9);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 30);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Поиск";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(650, 9);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(130, 30);
            this.btnNew.TabIndex = 5;
            this.btnNew.Text = "Новый участок";
            // 
            // split
            // 
            this.split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.split.Location = new System.Drawing.Point(0, 50);
            this.split.Name = "split";
            // 
            // split.Panel1
            // 
            this.split.Panel1.Controls.Add(this.flow);
            // 
            // split.Panel2
            // 
            this.split.Panel2.Controls.Add(this.editorScroll);
            this.split.Size = new System.Drawing.Size(1677, 733);
            this.split.SplitterDistance = 1352;
            this.split.TabIndex = 6;
            // 
            // flow
            // 
            this.flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flow.Location = new System.Drawing.Point(0, 0);
            this.flow.Name = "flow";
            this.flow.Padding = new System.Windows.Forms.Padding(5);
            this.flow.Size = new System.Drawing.Size(1352, 733);
            this.flow.TabIndex = 0;
            // 
            // editorScroll
            // 
            this.editorScroll.AutoScroll = true;
            this.editorScroll.Controls.Add(this.editor);
            this.editorScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorScroll.Location = new System.Drawing.Point(0, 0);
            this.editorScroll.Name = "editorScroll";
            this.editorScroll.Size = new System.Drawing.Size(321, 733);
            this.editorScroll.TabIndex = 0;
            // 
            // editor
            // 
            this.editor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editor.BackColor = System.Drawing.Color.White;
            this.editor.Location = new System.Drawing.Point(2, 0);
            this.editor.Name = "editor";
            this.editor.Size = new System.Drawing.Size(605, 701);
            this.editor.TabIndex = 0;
            // 
            // PlotsPanel
            // 
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.cmbCondition);
            this.Controls.Add(this.cmbSort);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.split);
            this.Name = "PlotsPanel";
            this.Size = new System.Drawing.Size(1677, 783);
            this.split.Panel1.ResumeLayout(false);
            this.split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.split)).EndInit();
            this.split.ResumeLayout(false);
            this.editorScroll.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
