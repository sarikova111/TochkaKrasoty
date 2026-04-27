namespace TochkaKrasoty.Models
{
    public class ServiceItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationMinutes { get; set; }

        public int MasterId { get; set; }
        public Master? Master { get; set; }
    }
}