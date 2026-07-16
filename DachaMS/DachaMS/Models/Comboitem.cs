namespace DachaMS.Models
{
    public class ComboItem
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public ComboItem(
            int id,
            string text)
        {
            Id = id;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}