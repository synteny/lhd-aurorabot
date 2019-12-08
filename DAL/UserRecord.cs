using System;

namespace DAL
{
    public class UserRecord
    {
        public int chat_id { get; set; }
        public int dialog_state { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? area_id { get; set; }
        public int? timezome { get; set; }
        public int? weather { get; set; }
        public bool? subscribed { get; set; }
    }
}