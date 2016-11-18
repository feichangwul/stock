using System;
using System.Collections.Generic;
using System.Text;
using wojilu;
using wojilu.Web;
using wojilu.Web.Mvc;

namespace StockRoom.BLL.Controller
{
    public class HistoryController : ControllerBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MainController));

        public virtual void Index()
        {
            string sql = "SELECT DISTINCT ID,CreatedDateTime,  ROOMID FROM Stock_History";
            List<TeacherHis> dataInDB = db.findBySql<TeacherHis>(sql);
            IBlock block = getBlock("list");
            foreach (var item in dataInDB)
            {
                block.Set("teacher.RoomId", item.RoomId);
                block.Set("teacher.CreatedDateTime", item.CreatedDateTime);
            }
            block.Next();
        }
    }
}
