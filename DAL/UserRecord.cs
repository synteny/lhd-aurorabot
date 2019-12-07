using System;

namespace DAL
{
    public class UserRecord
    {
        public int ChatId { get; set; }
        public int DialogState { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? AreaId { get; set; }
        public int? Timezome { get; set; }
        public int? Weather { get; set; }
        public bool? Subscribed { get; set; }
    }
}