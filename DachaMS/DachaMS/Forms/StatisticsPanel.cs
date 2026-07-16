using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using DachaMS.DB;

namespace DachaMS
{
    public partial class StatisticsPanel : BasePanel
    {
        private FlowLayoutPanel flow;
        private Button btnRefresh;

        public override string PanelTitle
        {
            get { return "Статистика по массиву"; }
        }

        public StatisticsPanel()
        {
            InitializeComponent();
            ResizeFlow();
            LoadStatistics();
            btnRefresh.Click += (s, e) => LoadStatistics();
        }

        private void InitializeComponent()
        {
            btnRefresh = new Button();
            btnRefresh.Text = "Обновить";
            btnRefresh.Location = new Point(10, 10);
            btnRefresh.Width = 150;
            btnRefresh.Height = 30;

            flow = new FlowLayoutPanel();
            flow.Location = new Point(0, 50);
            flow.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                        | AnchorStyles.Left | AnchorStyles.Right;
            flow.AutoScroll = false;
            flow.WrapContents = true;
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.Padding = new Padding(10);

            Controls.Add(btnRefresh);
            Controls.Add(flow);

            this.Resize += (s, e) =>
            {
                ResizeFlow();
                UpdateFlowScroll();
            };
            flow.Resize += (s, e) => UpdateFlowScroll();
        }

        private void ResizeFlow()
        {
            flow.Size = new Size(Width, Height - 50);
        }

        private void UpdateFlowScroll()
        {
            BasePanel.UpdateFlowScroll(flow);
        }

        private void LoadStatistics()
        {
            flow.Controls.Clear();

            try
            {
                using (SqlConnection conn = Database.GetConnection())
                {
                    conn.Open();

                    // ── Общие счётчики ──────────────────────────────────────
                    AddCard("Участков всего",
                        ExecuteScalar(conn, "SELECT COUNT(*) FROM plots"));

                    AddCard("Владельцев",
                        ExecuteScalar(conn, "SELECT COUNT(*) FROM owners"));

                    AddCard("Построек",
                        ExecuteScalar(conn, "SELECT COUNT(*) FROM buildings"));

                    // ── Состояние участков ──────────────────────────────────
                    AddCard("Заброшенных участков",
                        ExecuteScalar(conn,
                            "SELECT COUNT(*) FROM plots WHERE condition = N'заброшен'"));

                    AddCard("Облагороженных участков",
                        ExecuteScalar(conn,
                            "SELECT COUNT(*) FROM plots WHERE condition = N'облагорожен'"));

                    AddCard("Участков без владельца",
                        ExecuteScalar(conn,
                            "SELECT COUNT(*) FROM plots WHERE owner_id IS NULL"));

                    AddCard("Участков с электричеством",
                        ExecuteScalar(conn,
                            "SELECT COUNT(*) FROM plots WHERE electricity = 1"));

                    // ── Площадь ─────────────────────────────────────────────
                    object avgArea = ExecuteScalar(conn,
                        "SELECT AVG(CAST(area_sotki AS FLOAT)) FROM plots");
                    AddCard("Средняя площадь",
                        avgArea == null || avgArea == DBNull.Value
                            ? "–"
                            : Math.Round(Convert.ToDouble(avgArea), 1).ToString("N1") + " сот.");

                    object maxArea = ExecuteScalar(conn,
                        "SELECT MAX(area_sotki) FROM plots");
                    AddCard("Максимальная площадь",
                        maxArea == null || maxArea == DBNull.Value
                            ? "–"
                            : maxArea.ToString() + " сот.");

                    object minArea = ExecuteScalar(conn,
                        "SELECT MIN(area_sotki) FROM plots");
                    AddCard("Минимальная площадь",
                        minArea == null || minArea == DBNull.Value
                            ? "–"
                            : minArea.ToString() + " сот.");

                    // ── Стоимость ────────────────────────────────────────────
                    object totalPrice = ExecuteScalar(conn,
                        "SELECT SUM(dbo.calculate_price(plot_id)) FROM plots");
                    AddCard("Суммарная стоимость",
                        totalPrice == null || totalPrice == DBNull.Value
                            ? "–"
                            : Convert.ToDecimal(totalPrice).ToString("N0") + " ₽");

                    AddCard("Средняя стоимость",
                        Convert.ToDecimal(
                            ExecuteScalar(conn,
                                "SELECT AVG(dbo.calculate_price(plot_id)) FROM plots"))
                            .ToString("N0") + " ₽");

                    AddCard("Максимальная стоимость",
                        Convert.ToDecimal(
                            ExecuteScalar(conn,
                                "SELECT MAX(dbo.calculate_price(plot_id)) FROM plots"))
                            .ToString("N0") + " ₽");

                    AddCard("Минимальная стоимость",
                        Convert.ToDecimal(
                            ExecuteScalar(conn,
                                "SELECT MIN(dbo.calculate_price(plot_id)) FROM plots"))
                            .ToString("N0") + " ₽");

                    // ── Рейтинг ──────────────────────────────────────────────
                    object avgRating = ExecuteScalar(conn,
                        "SELECT AVG(CAST(dbo.calculate_rating(plot_id) AS FLOAT)) FROM plots");
                    AddCard("Средний рейтинг",
                        avgRating == null || avgRating == DBNull.Value
                            ? "–"
                            : Math.Round(Convert.ToDouble(avgRating), 1).ToString("N1"));

                    // ── Лучший и худший участки ──────────────────────────────
                    LoadBestPlot(conn, best: true);
                    LoadBestPlot(conn, best: false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки статистики:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            flow.PerformLayout();
            UpdateFlowScroll();
        }

        private object ExecuteScalar(SqlConnection conn, string sql)
        {
            return new SqlCommand(sql, conn).ExecuteScalar();
        }

        private void LoadBestPlot(SqlConnection conn, bool best)
        {
            string order = best ? "DESC" : "ASC";
            string label = best ? "Лучший участок" : "Худший участок";

            SqlCommand cmd = new SqlCommand($@"
                SELECT TOP 1
                    street_num,
                    plot_num,
                    dbo.calculate_rating(plot_id) AS rating
                FROM plots
                ORDER BY dbo.calculate_rating(plot_id) {order}",
                conn);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                AddCard(label,
                    "Улица " + reader["street_num"] +
                    ", участок " + reader["plot_num"] +
                    Environment.NewLine +
                    "Рейтинг: " + reader["rating"]);
            }

            reader.Close();
        }

        private void AddCard(string title, string value)
        {
            flow.Controls.Add(new StatisticsCard(title, value));
        }

        private void AddCard(string title, object value)
        {
            AddCard(title,
                value == null || value == DBNull.Value
                    ? "0"
                    : value.ToString());
        }
    }
}
