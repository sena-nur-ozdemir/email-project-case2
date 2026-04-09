namespace Case2EmailProject_.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
