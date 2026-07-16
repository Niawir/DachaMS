using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;

namespace DachaMS
{
    public partial class PriceRulesPanel : BasePanel
    {
        private NumericUpDown numBasePrice;
        private NumericUpDown numElectricityBonus;
        private NumericUpDown numAbandonedPenalty;

        private Button btnSave;

        public override string PanelTitle
        {
            get { return "Правила цен"; }
        }

        public PriceRulesPanel()
        {
            InitializeComponent();
            LoadConfig();
            btnSave.Click += BtnSave_Click;
        }

        private void InitializeComponent()
        {
            Label lblBasePrice = new Label();
            lblBasePrice.Text = "Цена за сотку";
            lblBasePrice.Location = new Point(20, 20);
            lblBasePrice.AutoSize = true;

            numBasePrice = new NumericUpDown();
            numBasePrice.Location = new Point(20, 45);
            numBasePrice.Width = 250;
            numBasePrice.Minimum = -100000000;
            numBasePrice.Maximum = 100000000;

            Label lblElectricity = new Label();
            lblElectricity.Text = "Надбавка за электричество";
            lblElectricity.Location = new Point(20, 95);
            lblElectricity.AutoSize = true;

            numElectricityBonus = new NumericUpDown();
            numElectricityBonus.Location = new Point(20, 120);
            numElectricityBonus.Width = 250;
            numElectricityBonus.Minimum = -100000000;
            numElectricityBonus.Maximum = 100000000;

            Label lblPenalty = new Label();
            lblPenalty.Text = "Штраф за заброшенность";
            lblPenalty.Location = new Point(20, 170);
            lblPenalty.AutoSize = true;

            numAbandonedPenalty = new NumericUpDown();
            numAbandonedPenalty.Location = new Point(20, 195);
            numAbandonedPenalty.Width = 250;
            numAbandonedPenalty.Minimum = -100000000;
            numAbandonedPenalty.Maximum = 100000000;

            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(20, 260);
            btnSave.Width = 180;
            btnSave.Height = 35;


            Controls.Add(lblBasePrice);
            Controls.Add(numBasePrice);

            Controls.Add(lblElectricity);
            Controls.Add(numElectricityBonus);

            Controls.Add(lblPenalty);
            Controls.Add(numAbandonedPenalty);

            Controls.Add(btnSave);
        }

        private void LoadConfig()
        {
            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM [DachaMS].[dbo].[config]", conn);


                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    numBasePrice.Value =
                        Convert.ToDecimal(
                            reader["base_price_per_sotka"]);

                    numElectricityBonus.Value =
                        Convert.ToDecimal(
                            reader["electricity_bonus"]);

                    numAbandonedPenalty.Value =
                        Convert.ToDecimal(
                            reader["abandoned_penalty"]);
                }
            }
        }

        private void BtnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {

                if (numBasePrice.Value < 0)
                {
                    MessageBox.Show(
                        "Цена за сотку должна быть больше, или равна нулю.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    numBasePrice.Focus();
                    return;
                }
                if (numElectricityBonus.Value < 0)
                {
                    MessageBox.Show(
                        "Надбавка за электричество должна быть больше, или равна нулю.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    numElectricityBonus.Focus();
                    return;
                }
                if (numAbandonedPenalty.Value < 0)
                {
                    MessageBox.Show(
                        "Штраф за заброшенность должен быть больше, или равен нулю.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    numAbandonedPenalty.Focus();
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
                            base_price_per_sotka = @base,
                            electricity_bonus = @electricity,
                            abandoned_penalty = @penalty",
                        conn);

                    cmd.Parameters.AddWithValue(
                        "@base",
                        numBasePrice.Value);

                    cmd.Parameters.AddWithValue(
                        "@electricity",
                        numElectricityBonus.Value);

                    cmd.Parameters.AddWithValue(
                        "@penalty",
                        numAbandonedPenalty.Value);

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