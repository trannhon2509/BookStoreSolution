namespace BookStore.BOL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Book>? Products { get; set; } = new List<Book>();
    }
}
