using System;
using wojilu.ORM;

namespace StockRoom.BLL
{
    [Table("Stock_History")]
    public class TeacherHis : Teacher
    {
        [NotNull]
        public DateTime CreatedDateTime { get; set; }       
    }
}
