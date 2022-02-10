using System;

namespace Estimmo.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime SentOn { get; set; }
        public bool IsArchived { get; set; }
    }
}
