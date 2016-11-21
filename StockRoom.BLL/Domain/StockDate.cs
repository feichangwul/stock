using System;
using wojilu;
using wojilu.ORM;

namespace StockRoom.BLL
{
    [Table("Stock_Date")]
    public class StockDate : ObjectBase<StockDate>
    {
        [NotNull]
        public int RoomId { get; set; }
        [NotNull]
        public string CreatedDate { get; set; }       
    }
}
