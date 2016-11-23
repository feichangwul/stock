using System;
using System.Collections.Generic;
using System.Text;
using wojilu;
using wojilu.Web;
using wojilu.Web.Mvc;
using wojilu.Web.Mvc.Routes;

namespace StockRoom.BLL.Controller
{
    public class HistoryController : ControllerBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MainController));

        public HistoryController()
        {
            HideLayout(typeof(LayoutController));
        }

        public override void Layout()
        {

        }

        public virtual void Index()
        {
            List<StockDate> dataInDB = db.findAll<StockDate>();
            IBlock block = getBlock("list");
            foreach (var item in dataInDB)
            {
                block.Set("teacher.RoomId", item.RoomId);
                block.Set("teacher.CreatedDateTime", item.CreatedDate);
                block.Set("roomLink", getRoomLink(item.RoomId,item.CreatedDate));
                block.Next();
            }
        }

        public virtual void Show()
        {
            var result = RouteTool.RecognizePath(ctx.url.PathAndQueryWithouApp);
            string roomDate = result.getItem("roomDate").Replace(MvcConfig.Instance.UrlExt,"");
            string roomId = result.getItem("roomId");
            int id;
            int.TryParse(roomId, out id);

            DateTime date;
           // DateTime.TryParse(roomDate, out date);
            date = DateTime.ParseExact(roomDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            List<TeacherHis> dataInDB = db.find<TeacherHis>("roomId = :roomId and StrComp(:now,Left(StockDate,10)) = 0")
                    .set("roomId", id)
                    .set("now", date.ToString("yyyy/MM/dd"))
                    .list();

            IBlock block = getBlock("list");
            foreach (var item in dataInDB)
            {
                block.Set("teacher.AddTime", item.AddTime);
                string thumbPicURL = item.ThumbUrl;
                if (string.IsNullOrEmpty(thumbPicURL))
                {
                    block.Set("Content", item.MessageInfo);
                }
                else
                {
                    block.Set("Content", string.Format("<img src='{0}'  alt='pic' />", thumbPicURL));
                }
                block.Next();
            }
        }

        public String getRoomLink(int id, String date)
        {
            return "/history/" + id + "/" + date + MvcConfig.Instance.UrlExt;

        }
    }
}
