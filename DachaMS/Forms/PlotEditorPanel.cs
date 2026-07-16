using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.Models;
using DachaMS.DB;

namespace DachaMS
{
    public partial class PlotEditorPanel : UserControl
    {
        private int? currentPlotId;

        public event Action Saved;

        private NumericUpDown numStreet;
        private NumericUpDown numPlot;
        private NumericUpDown numArea;

        private ComboBox cmbOwner;
        private ComboBox cmbCondition;

        private CheckBox chkElectricity;

        private CheckedListBox clbBuildings;

        private Label lblPrice;
        private Label lblRating;

        private Button btnSave;
        private Label lblTitle;
        private Label lblStreet;
        private Label lblPlot;
        private Label lblArea;
        private Label lblOwner;
        private Label lblCondition;
        private Label lblBuildings;
        private Button btnDelete;

        public PlotEditorPanel()
        {
            InitializeComponent();
            AppBranding.ApplyContentBackground(this);

            LoadOwners();
            LoadBuildings();
            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;

            if (Session.Role != "admin")
            {
                btnSave.Enabled = false;
                btnDelete.Enabled = false;
                cmbOwner.Enabled = false;
                cmbCondition.Enabled = false;
                numStreet.Enabled = false;
                numPlot.Enabled = false;
                numArea.Enabled = false;
                chkElectricity.Enabled = false;
                clbBuildings.Enabled = false;
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblStreet = new System.Windows.Forms.Label();
            this.numStreet = new System.Windows.Forms.NumericUpDown();
            this.lblPlot = new System.Windows.Forms.Label();
            this.numPlot = new System.Windows.Forms.NumericUpDown();
            this.lblArea = new System.Windows.Forms.Label();
            this.numArea = new System.Windows.Forms.NumericUpDown();
            this.lblOwner = new System.Windows.Forms.Label();
            this.cmbOwner = new System.Windows.Forms.ComboBox();
            this.chkElectricity = new System.Windows.Forms.CheckBox();
            this.lblCondition = new System.Windows.Forms.Label();
            this.cmbCondition = new System.Windows.Forms.ComboBox();
            this.lblBuildings = new System.Windows.Forms.Label();
            this.clbBuildings = new System.Windows.Forms.CheckedListBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblRating = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numStreet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numArea)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(149, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Редактор участка";
            // 
            // lblStreet
            // 
            this.lblStreet.Location = new System.Drawing.Point(20, 70);
            this.lblStreet.Name = "lblStreet";
            this.lblStreet.Size = new System.Drawing.Size(100, 23);
            this.lblStreet.TabIndex = 1;
            this.lblStreet.Text = "Улица";
            // 
            // numStreet
            // 
            this.numStreet.Location = new System.Drawing.Point(20, 95);
            this.numStreet.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numStreet.Name = "numStreet";
            this.numStreet.Size = new System.Drawing.Size(200, 20);
            this.numStreet.TabIndex = 2;
            // 
            // lblPlot
            // 
            this.lblPlot.Location = new System.Drawing.Point(250, 70);
            this.lblPlot.Name = "lblPlot";
            this.lblPlot.Size = new System.Drawing.Size(100, 23);
            this.lblPlot.TabIndex = 3;
            this.lblPlot.Text = "Участок";
            // 
            // numPlot
            // 
            this.numPlot.Location = new System.Drawing.Point(250, 95);
            this.numPlot.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numPlot.Name = "numPlot";
            this.numPlot.Size = new System.Drawing.Size(200, 20);
            this.numPlot.TabIndex = 4;
            // 
            // lblArea
            // 
            this.lblArea.Location = new System.Drawing.Point(20, 140);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(100, 23);
            this.lblArea.TabIndex = 5;
            this.lblArea.Text = "Площадь (сотки)";
            // 
            // numArea
            // 
            this.numArea.DecimalPlaces = 2;
            this.numArea.Location = new System.Drawing.Point(20, 165);
            this.numArea.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numArea.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numArea.Name = "numArea";
            this.numArea.Size = new System.Drawing.Size(200, 20);
            this.numArea.TabIndex = 6;
            // 
            // lblOwner
            // 
            this.lblOwner.Location = new System.Drawing.Point(20, 210);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(100, 23);
            this.lblOwner.TabIndex = 7;
            this.lblOwner.Text = "Владелец";
            // 
            // cmbOwner
            // 
            this.cmbOwner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOwner.Location = new System.Drawing.Point(20, 235);
            this.cmbOwner.Name = "cmbOwner";
            this.cmbOwner.Size = new System.Drawing.Size(430, 21);
            this.cmbOwner.TabIndex = 8;
            // 
            // chkElectricity
            // 
            this.chkElectricity.Location = new System.Drawing.Point(20, 280);
            this.chkElectricity.Name = "chkElectricity";
            this.chkElectricity.Size = new System.Drawing.Size(104, 24);
            this.chkElectricity.TabIndex = 9;
            this.chkElectricity.Text = "Есть электричество";
            // 
            // lblCondition
            // 
            this.lblCondition.Location = new System.Drawing.Point(20, 320);
            this.lblCondition.Name = "lblCondition";
            this.lblCondition.Size = new System.Drawing.Size(100, 23);
            this.lblCondition.TabIndex = 10;
            this.lblCondition.Text = "Состояние";
            // 
            // cmbCondition
            // 
            this.cmbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCondition.Items.AddRange(new object[] {
            "облагорожен",
            "заброшен"});
            this.cmbCondition.Location = new System.Drawing.Point(20, 345);
            this.cmbCondition.Name = "cmbCondition";
            this.cmbCondition.Size = new System.Drawing.Size(250, 21);
            this.cmbCondition.TabIndex = 11;
            // 
            // lblBuildings
            // 
            this.lblBuildings.Location = new System.Drawing.Point(20, 390);
            this.lblBuildings.Name = "lblBuildings";
            this.lblBuildings.Size = new System.Drawing.Size(100, 23);
            this.lblBuildings.TabIndex = 12;
            this.lblBuildings.Text = "Постройки";
            // 
            // clbBuildings
            // 
            this.clbBuildings.Location = new System.Drawing.Point(20, 415);
            this.clbBuildings.Name = "clbBuildings";
            this.clbBuildings.Size = new System.Drawing.Size(430, 139);
            this.clbBuildings.TabIndex = 13;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(20, 590);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(0, 19);
            this.lblPrice.TabIndex = 14;
            // 
            // lblRating
            // 
            this.lblRating.AutoSize = true;
            this.lblRating.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRating.Location = new System.Drawing.Point(20, 620);
            this.lblRating.Name = "lblRating";
            this.lblRating.Size = new System.Drawing.Size(0, 19);
            this.lblRating.TabIndex = 15;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(20, 670);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(180, 23);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Сохранить";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(220, 670);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(180, 23);
            this.btnDelete.TabIndex = 17;
            this.btnDelete.Text = "Удалить";
            // 
            // PlotEditorPanel
            // 
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblStreet);
            this.Controls.Add(this.numStreet);
            this.Controls.Add(this.lblPlot);
            this.Controls.Add(this.numPlot);
            this.Controls.Add(this.lblArea);
            this.Controls.Add(this.numArea);
            this.Controls.Add(this.lblOwner);
            this.Controls.Add(this.cmbOwner);
            this.Controls.Add(this.chkElectricity);
            this.Controls.Add(this.lblCondition);
            this.Controls.Add(this.cmbCondition);
            this.Controls.Add(this.lblBuildings);
            this.Controls.Add(this.clbBuildings);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblRating);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnDelete);
            this.Name = "PlotEditorPanel";
            this.Size = new System.Drawing.Size(593, 701);
            ((System.ComponentModel.ISupportInitialize)(this.numStreet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPlot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numArea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void LoadOwners()
        {
            cmbOwner.Items.Clear();
            cmbOwner.Items.Add(new ComboItem(0, "Не назначен"));

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    // Заставляем SQL Server принудительно переключиться на нужную базу
                    // с помощью команды "USE DachaMS;" прямо перед основным запросом!
                    string sql = @"
                USE [DachaMS];
                SELECT [owner_id], [full_name] 
                FROM [dbo].[owners] 
                ORDER BY [full_name];";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader myReader = cmd.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                cmbOwner.Items.Add(new ComboItem(
                                    Convert.ToInt32(myReader["owner_id"]),
                                    myReader["full_name"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в LoadOwners: " + ex.Message + "\n" + ex.StackTrace);
            }

            cmbOwner.SelectedIndex = 0;
        }



        private void LoadBuildings()
        {
            clbBuildings.Items.Clear();

            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        @"SELECT
                            building_id,
                            building_name
                          FROM [DachaMS].[dbo].[buildings]
                          ORDER BY building_name",
                        conn);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    clbBuildings.Items.Add(
                        new ComboItem(
                            Convert.ToInt32(
                                reader["building_id"]),
                            reader["building_name"]
                                .ToString()));
                }
            }
        }

        public void LoadPlot(int? plotId)
        {
            currentPlotId = plotId;

            foreach (int i in clbBuildings.CheckedIndices)
            {
            }

            for (int i = 0; i < clbBuildings.Items.Count; i++)
                clbBuildings.SetItemChecked(i, false);

            if (plotId == null)
            {
                numStreet.Value = 1;
                numPlot.Value = 1;
                numArea.Value = 0;

                chkElectricity.Checked = false;

                cmbOwner.SelectedIndex = 0;
                cmbCondition.Text = "заброшен";

                lblPrice.Text = "Стоимость: -";
                lblRating.Text = "Рейтинг: -";

                btnDelete.Enabled = false;

                return;
            }

            btnDelete.Enabled = true;

            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
            SELECT *
            FROM plots
            WHERE plot_id=@id",
                    conn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    plotId);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    numStreet.Value =
                        Convert.ToDecimal(
                            reader["street_num"]);

                    numPlot.Value =
                        Convert.ToDecimal(
                            reader["plot_num"]);

                    numArea.Value =
                        Convert.ToDecimal(
                            reader["area_sotki"]);

                    chkElectricity.Checked =
                        Convert.ToBoolean(
                            reader["electricity"]);

                    string condition =
                        reader["condition"]
                        .ToString();

                    cmbCondition.Text =
                        condition;

                    if (reader["owner_id"] != DBNull.Value)
                    {
                        int ownerId =
                            Convert.ToInt32(
                                reader["owner_id"]);

                        for (int i = 0;
                             i < cmbOwner.Items.Count;
                             i++)
                        {
                            ComboItem item =
                                (ComboItem)
                                cmbOwner.Items[i];

                            if (item.Id == ownerId)
                            {
                                cmbOwner.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        cmbOwner.SelectedIndex = 0;
                    }
                }

                reader.Close();

                LoadBuildingsForPlot(
                    conn,
                    plotId.Value);

                LoadCalculatedValues(
                    conn,
                    plotId.Value);
            }
        }
        private void LoadBuildingsForPlot(
        SqlConnection conn,
        int plotId)
        {
            SqlCommand cmd =
                new SqlCommand(@"
        SELECT building_id
        FROM plot_buildings
        WHERE plot_id=@id",
                conn);

            cmd.Parameters.AddWithValue(
                "@id",
                plotId);

            SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                int buildingId =
                    Convert.ToInt32(
                        reader["building_id"]);

                for (int i = 0;
                     i < clbBuildings.Items.Count;
                     i++)
                {
                    ComboItem item =
                        (ComboItem)
                        clbBuildings.Items[i];

                    if (item.Id == buildingId)
                    {
                        clbBuildings.SetItemChecked(
                            i,
                            true);

                        break;
                    }
                }
            }

            reader.Close();
        }
        private void LoadCalculatedValues(SqlConnection conn, int plotId)
        {
            // Добавляем USE [DachaMS] и оборачиваем функции в квадратные скобки
            string sql = @"
        USE [DachaMS];
        SELECT 
            [dbo].[calculate_price](@id), 
            [dbo].[calculate_rating](@id);";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", plotId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Проверяем на DBNull, чтобы программа случайно не упала, если данных нет
                        lblPrice.Text = "Стоимость: " + (reader[0] != DBNull.Value ? Convert.ToDecimal(reader[0]).ToString("N0") : "0") + " ₽";
                        lblRating.Text = "Рейтинг: " + (reader[1] != DBNull.Value ? reader[1].ToString() : "0");
                    }
                }
            }
        }

        private void BtnSave_Click(
        object sender,
        EventArgs e)
        {
            if (numStreet.Value <= 0)
            {
                MessageBox.Show(
                    "Номер улицы должен быть больше 0.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (numPlot.Value <= 0)
            {
                MessageBox.Show(
                    "Номер участка должен быть больше 0.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (numArea.Value <= 0)
            {
                MessageBox.Show(
                    "Площадь участка должна быть больше 0.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlTransaction tr =
                    conn.BeginTransaction();

                try
                {
                    SqlCommand duplicateCheck;

                    if (currentPlotId == null)
                    {
                        duplicateCheck = new SqlCommand(@"
        SELECT COUNT(*)
        FROM plots
        WHERE street_num = @street
          AND plot_num = @plot",
                            conn,
                            tr);
                    }
                    else
                    {
                        duplicateCheck = new SqlCommand(@"
        SELECT COUNT(*)
        FROM plots
        WHERE street_num = @street
          AND plot_num = @plot
          AND plot_id <> @id",
                            conn,
                            tr);

                        duplicateCheck.Parameters.AddWithValue(
                            "@id",
                            currentPlotId.Value);
                    }

                    duplicateCheck.Parameters.AddWithValue(
                        "@street",
                        (int)numStreet.Value);

                    duplicateCheck.Parameters.AddWithValue(
                        "@plot",
                        (int)numPlot.Value);

                    int duplicates =
                        Convert.ToInt32(
                            duplicateCheck.ExecuteScalar());

                    if (duplicates > 0)
                    {
                        MessageBox.Show(
                            "Участок с таким номером улицы и номером участка уже существует.",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        tr.Rollback();
                        return;
                    }

                    int plotId;

                    if (currentPlotId == null)
                    {
                        SqlCommand insert =
                            new SqlCommand(@"
                    INSERT INTO plots
                    (
                        street_num,
                        plot_num,
                        area_sotki,
                        electricity,
                        condition,
                        owner_id
                    )
                    VALUES
                    (
                        @street,
                        @plot,
                        @area,
                        @electricity,
                        @condition,
                        @owner
                    );

                    SELECT SCOPE_IDENTITY();",
                            conn,
                            tr);

                        FillPlotParameters(insert);

                        plotId =
                            Convert.ToInt32(
                                insert.ExecuteScalar());

                        currentPlotId =
                            plotId;
                    }
                    else
                    {
                        plotId =
                            currentPlotId.Value;

                        SqlCommand update =
                            new SqlCommand(@"
                    UPDATE plots
                    SET
                        street_num=@street,
                        plot_num=@plot,
                        area_sotki=@area,
                        electricity=@electricity,
                        condition=@condition,
                        owner_id=@owner
                    WHERE plot_id=@id",
                            conn,
                            tr);

                        FillPlotParameters(update);

                        update.Parameters.AddWithValue(
                            "@id",
                            plotId);

                        update.ExecuteNonQuery();

                        SqlCommand deleteLinks =
                            new SqlCommand(@"
                    DELETE FROM plot_buildings
                    WHERE plot_id=@id",
                            conn,
                            tr);

                        deleteLinks.Parameters.AddWithValue(
                            "@id",
                            plotId);

                        deleteLinks.ExecuteNonQuery();
                    }

                    SaveBuildings(
                        conn,
                        tr,
                        plotId);

                    tr.Commit();

                    LoadCalculatedValues(
                        conn,
                        plotId);

                    Saved?.Invoke();

                    MessageBox.Show(
                        "Сохранено.");
                }
                catch (Exception ex)
                {
                    tr.Rollback();

                    MessageBox.Show(
                        ex.Message);
                }
            }
        }
        private void FillPlotParameters(
        SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue(
                "@street",
                (int)numStreet.Value);

            cmd.Parameters.AddWithValue(
                "@plot",
                (int)numPlot.Value);

            cmd.Parameters.AddWithValue(
                "@area",
                numArea.Value);

            cmd.Parameters.AddWithValue(
                "@electricity",
                chkElectricity.Checked);

            cmd.Parameters.AddWithValue(
                "@condition",
                cmbCondition.Text);

            ComboItem owner =
                (ComboItem)
                cmbOwner.SelectedItem;

            if (owner.Id == 0)
            {
                cmd.Parameters.AddWithValue(
                    "@owner",
                    DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue(
                    "@owner",
                    owner.Id);
            }
        }
        private void SaveBuildings(
        SqlConnection conn,
        SqlTransaction tr,
        int plotId)
        {
            foreach (object obj
                in clbBuildings.CheckedItems)
            {
                ComboItem item =
                    (ComboItem)obj;

                SqlCommand cmd =
                    new SqlCommand(@"
            INSERT INTO plot_buildings
            (
                plot_id,
                building_id
            )
            VALUES
            (
                @plot,
                @building
            )",
                    conn,
                    tr);

                cmd.Parameters.AddWithValue(
                    "@plot",
                    plotId);

                cmd.Parameters.AddWithValue(
                    "@building",
                    item.Id);

                cmd.ExecuteNonQuery();
            }
        }
        private void BtnDelete_Click(
        object sender,
        EventArgs e)
        {
            if (currentPlotId == null)
                return;

            if (MessageBox.Show(
                "Удалить участок?",
                "Подтверждение",
                MessageBoxButtons.YesNo)
                != DialogResult.Yes)
                return;

            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
            DELETE FROM plots
            WHERE plot_id=@id",
                    conn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    currentPlotId);

                cmd.ExecuteNonQuery();
            }

            LoadPlot(null);

            Saved?.Invoke();
        }
    }
}