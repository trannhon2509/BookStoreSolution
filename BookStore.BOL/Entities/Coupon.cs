namespace BookStore.BOL.Entities
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
