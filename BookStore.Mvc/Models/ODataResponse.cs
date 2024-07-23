namespace BookStore.Mvc.Models
{
    public class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }

}
