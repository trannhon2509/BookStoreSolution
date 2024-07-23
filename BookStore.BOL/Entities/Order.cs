using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BOL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public DateTime OrderDate { get; set; }
        public int? CouponId { get; set; } = null;

        public string? note { get; set; }
        public string Status { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        public ApplicationUser User { get; set; }

        public Coupon? Coupon { get; set; }
    }
}
