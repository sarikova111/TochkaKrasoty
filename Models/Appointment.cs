using System;

namespace TochkaKrasoty.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public string ClientName { get; set; } = "";

        // ВАЖНО: вот это поле вызывает ошибку сейчас
        public string Phone { get; set; } = "";

        public int MasterId { get; set; }
        public Master? Master { get; set; }

        public int ServiceItemId { get; set; }
        public ServiceItem? Service { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string? Review { get; set; }
    }
}