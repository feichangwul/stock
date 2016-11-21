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
            List<StockDate> dataInDB = db.findAll<StockDate>();
            IBlock block = getBlock("list");
            foreach (var item in dataInDB)
            {
                block.Set("teacher.RoomId", item.RoomId);
                block.Set("teacher.CreatedDateTime", item.CreatedDate);
                block.Next();
            }
        }
    }
}
