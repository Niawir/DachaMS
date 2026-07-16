using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;

namespace DachaMS
{
    public partial class RatingRulesPanel : BasePanel
    {
        private NumericUpDown numElectricityScore;
        private NumericUpDown numConditionScore;

        private Button btnSave;

        public override string PanelTitle
        {
            get { return "Правила рейтинга"; }
        }

        public RatingRulesPanel()
        {
            InitializeComponent();
            LoadConfig();
            btnSave.Click += BtnSave_Click;
        }

        private void InitializeComponent()
        {
            Label lblElectricity = new Label();
            lblElectricity.Text = "Баллы за электричество";
            lblElectricity.Location = new Point(20, 20);
            lblElectricity.AutoSize = true;

            numElectricityScore = new NumericUpDown();
            numElectricityScore.Location = new Point(20, 45);
            numElectricityScore.Width = 250;
            numElectricityScore.Minimum = -1000;
            numElectricityScore.Maximum = 1000;

            Label lblCondition = new Label();
            lblCondition.Text = "Баллы за облагороженность";
            lblCondition.Location = new Point(20, 95);
            lblCondition.AutoSize = true;

            numConditionScore = new NumericUpDown();
            numConditionScore.Location = new Point(20, 120);
            numConditionScore.Width = 250;
            numConditionScore.Minimum = -1000;
            numConditionScore.Maximum = 1000;

            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(20, 190);
            btnSave.Width = 180;
            btnSave.Height = 35;

            Controls.Add(lblElectricity);
            Controls.Add(numElectricityScore);

            Controls.Add(lblCondition);
            Controls.Add(numConditionScore);

            Controls.Add(btnSave);
        }

        private void LoadConfig()
        {
            using (SqlConnection conn = Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand(
                        "SELECT TOP 1 * FROM config",
                        conn);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    numElectricityScore.Value =
                        Convert.ToDecimal(
                            reader["electricity_score"]);

                    numConditionScore.Value =
                        Convert.ToDecimal(
                            reader["condition_score"]);
                }
            }
        }

        private void BtnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (numElectricityScore.Value < 0)
                {
                    MessageBox.Show(
                        "Баллы за электричество должны быть больше, или равны нулю.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    numElectricityScore.Focus();
                    return;
                }
                if (numConditionScore.Value < 0)
                {
                    MessageBox.Show(
                        "Баллы за облагороженность должны быть больше, или равны нулю.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    numConditionScore.Focus();
                    return;
                }

                using (SqlConnection conn =
                       Database.GetConnection())
                {
                    conn.Open();

                    SqlCommand cmd =
                        new SqlCommand(@"
                        UPDATE config
                        SET
                            electricity_score=@electricity,
                            condition_score=@condition",
                        conn);

                    cmd.Parameters.AddWithValue(
                        "@electricity",
                        (int)numElectricityScore.Value);

                    cmd.Parameters.AddWithValue(
                        "@condition",
                        (int)numConditionScore.Value);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Изменения сохранены.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}