namespace BookStore.Api.Dtos
{
    public class CouponDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
