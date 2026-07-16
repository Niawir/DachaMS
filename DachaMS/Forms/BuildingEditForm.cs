using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;

namespace DachaMS
{
    public partial class BuildingEditForm : Form
    {
        private readonly int? buildingId;

        private TextBox txtName;

        private NumericUpDown numPriceBonus;
        private NumericUpDown numRatingBonus;

        private Button btnSave;

        public BuildingEditForm(int? buildingId)
        {
            this.buildingId = buildingId;

            InitializeComponent();
            AppBranding.ApplySurfaceBackground(this);

            if (buildingId.HasValue)
                LoadBuilding();
            btnSave.Click += BtnSave_Click;
        }

        private void InitializeComponent()
        {
            Width = 500;
            Height = 320;

            StartPosition =
                FormStartPosition.CenterParent;

            Label lblName = new Label();
            lblName.Text = "Название";
            lblName.Location = new Point(20, 20);

            txtName = new TextBox();
            txtName.Location = new Point(20, 45);
            txtName.Width = 420;

            Label lblPrice = new Label();
            lblPrice.Text = "Надбавка к стоимости";
            lblPrice.Location = new Point(20, 90);

            numPriceBonus = new NumericUpDown();
            numPriceBonus.Location = new Point(20, 115);
            numPriceBonus.Width = 200;
            numPriceBonus.Maximum = 100000000;

            Label lblRating = new Label();
            lblRating.Text = "Баллы рейтинга";
            lblRating.Location = new Point(20, 160);

            numRatingBonus = new NumericUpDown();
            numRatingBonus.Location = new Point(20, 185);
            numRatingBonus.Width = 200;
            numRatingBonus.Maximum = 100000;

            btnSave = new Button();
            btnSave.Text = "Сохранить";
            btnSave.Location = new Point(20, 235);
            btnSave.Width = 150;


            Controls.Add(lblName);
            Controls.Add(txtName);

            Controls.Add(lblPrice);
            Controls.Add(numPriceBonus);

            Controls.Add(lblRating);
            Controls.Add(numRatingBonus);

            Controls.Add(btnSave);
        }

        private void LoadBuilding()
        {
            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                SqlCommand cmd =
                    new SqlCommand(@"
                    SELECT
                        b.building_name,
                        bp.price_bonus,
                        br.rating_bonus
                    FROM buildings b
                    LEFT JOIN building_price_bonus bp
                        ON bp.building_id=b.building_id
                    LEFT JOIN building_rating_bonus br
                        ON br.building_id=b.building_id
                    WHERE b.building_id=@id",
                    conn);

                cmd.Parameters.AddWithValue(
                    "@id",
                    buildingId);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtName.Text =
                        reader["building_name"].ToString();

                    numPriceBonus.Value =
                        Convert.ToDecimal(
                            reader["price_bonus"]);

                    numRatingBonus.Value =
                        Convert.ToDecimal(
                            reader["rating_bonus"]);
                }
            }
        }

        private void BtnSave_Click(
    object sender,
    EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Введите название строения.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtName.Focus();
                return;
            }

            foreach (char c in name)
            {
                bool isRussianLetter =
                    (c >= 'А' && c <= 'я') ||
                    c == 'Ё' ||
                    c == 'ё';

                if (!isRussianLetter &&
                    !char.IsWhiteSpace(c) &&
                    c != '-')
                {
                    MessageBox.Show(
                        "Название строения может содержать только русские буквы, пробелы и дефис.",
                        "Ошибка ввода",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtName.Focus();
                    return;
                }
            }

            if (numPriceBonus.Value < 0)
            {
                MessageBox.Show(
                    "Надбавка к стоимости должна быть больше, или равна нулю.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                numPriceBonus.Focus();
                return;
            }

            if (numRatingBonus.Value < 0)
            {
                MessageBox.Show(
                    "Баллы рейтинга должны быть больше, или равны нулю.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                numRatingBonus.Focus();
                return;
            }

            using (SqlConnection conn =
                   Database.GetConnection())
            {
                conn.Open();

                if (buildingId == null)
                {
                    SqlCommand insert =
                        new SqlCommand(@"
                        INSERT INTO buildings
                        (
                            building_name
                        )
                        VALUES
                        (
                            @name
                        );

                        SELECT SCOPE_IDENTITY();",
                        conn);

                    insert.Parameters.AddWithValue(
                        "@name",
                        txtName.Text);

                    int newId =
                        Convert.ToInt32(
                            insert.ExecuteScalar());

                    SqlCommand price =
                        new SqlCommand(@"
                        INSERT INTO building_price_bonus
                        (
                            building_id,
                            price_bonus
                        )
                        VALUES
                        (
                            @id,
                            @price
                        )", conn);

                    price.Parameters.AddWithValue(
                        "@id",
                        newId);

                    price.Parameters.AddWithValue(
                        "@price",
                        numPriceBonus.Value);

                    price.ExecuteNonQuery();

                    SqlCommand rating =
                        new SqlCommand(@"
                        INSERT INTO building_rating_bonus
                        (
                            building_id,
                            rating_bonus
                        )
                        VALUES
                        (
                            @id,
                            @rating
                        )", conn);

                    rating.Parameters.AddWithValue(
                        "@id",
                        newId);

                    rating.Parameters.AddWithValue(
                        "@rating",
                        numRatingBonus.Value);

                    rating.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand update =
                        new SqlCommand(@"
                        UPDATE buildings
                        SET building_name=@name
                        WHERE building_id=@id",
                        conn);

                    update.Parameters.AddWithValue(
                        "@id",
                        buildingId);

                    update.Parameters.AddWithValue(
                        "@name",
                        txtName.Text);

                    update.ExecuteNonQuery();

                    SqlCommand price =
                        new SqlCommand(@"
                        UPDATE building_price_bonus
                        SET price_bonus=@price
                        WHERE building_id=@id",
                        conn);

                    price.Parameters.AddWithValue(
                        "@id",
                        buildingId);

                    price.Parameters.AddWithValue(
                        "@price",
                        numPriceBonus.Value);

                    price.ExecuteNonQuery();

                    SqlCommand rating =
                        new SqlCommand(@"
                        UPDATE building_rating_bonus
                        SET rating_bonus=@rating
                        WHERE building_id=@id",
                        conn);

                    rating.Parameters.AddWithValue(
                        "@id",
                        buildingId);

                    rating.Parameters.AddWithValue(
                        "@rating",
                        numRatingBonus.Value);

                    rating.ExecuteNonQuery();
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}