namespace Case2EmailProject_.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string SenderMail { get; set; }
        public string ReceiverMail { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendingDate { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeletedBySender { get; set; } = false;
        public bool IsDeletedByReceiver { get; set; } = false;
        public bool IsStarred { get; set; } = false;
        public bool IsDraft { get; set; } = false;
        public bool IsSpam { get; set; } = false;
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
