using System;
namespace MVP_Web.Model
{
    public class Attempt
    {
        public string _id { get; set; }
        public string username { get; set; }
        public string examName { get; set; }
        public string examID { get; set; }
        public int score { get; set; }
        public int attempt { get; set; }
        public DateTime date { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }
}
