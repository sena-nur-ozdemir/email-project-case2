namespace Case2EmailProject_.Dtos
{
    public class CreateMessageDto
    {   
        public string SenderMail { get; set; }
        public string ReceiverMail { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendingDate { get; set; }
        public int CategoryId { get; set; }
    }
}
