using System;
using System.Collections.Generic;
using wojilu;
using wojilu.Web;
using wojilu.Web.Mvc;

namespace StockRoom.BLL.Controller
{
    public class MainController : ControllerBase
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MainController));

        public MainController()
        {
            //HidePermission(typeof(SecurityController));
        }
        public virtual void Index()
        {
            //只在工作日
            int i = (int)DateTime.Today.DayOfWeek;
            //if weekend
            if (i == 0 || i == 6)
            {
                //view history page
                redirect(new HistoryController().Index);
            }
            else
            {
                redirect(Show, 332);
            }
        }


        public virtual void Show(Int64 id)
        {
            //只在工作日
            int i = (int)DateTime.Today.DayOfWeek;
            //if weekend
            if (i == 0 || i == 6)
            {
                //view history page
                redirect(new HistoryController().Index);
            }
            else
            {
                var dataInDB = db.find<Teacher333>("roomId = :roomId and StrComp(:now,Left(addtime,10)) = 0")
                  .set("roomId", id)
                  .set("now", DateTime.Now.ToString("yyyy-MM-dd"))
                  .list();

                IBlock block = getBlock("list");
                foreach (var item in dataInDB)
                {
                    //block.Set("teacher.RoomId", item.RoomId);
                    block.Set("teacher.AddTime", item.AddTime);
                    string replyMsg = item.ReplyMessageInfo;
                    string thumbPicURL = item.ThumbUrl;
                    if (string.IsNullOrEmpty(thumbPicURL))
                    {
                        string content = item.MessageInfo;
                        if (!string.IsNullOrEmpty(replyMsg))
                        {
                            content = string.Format("<font size=\"2\" color=\"red\">{0}</font>{1}{2}", replyMsg, "</br>", content);
                        }
                        block.Set("Content", content);
                    }
                    else
                    {
                        block.Set("Content", string.Format("<img src='{0}'  alt='pic' />", thumbPicURL));
                    }
                    block.Next();
                }
            }

        }

        public virtual void JsonResult()
        {

            List<Teacher333> dataInDB = Teacher333.findAll();

            // 使用 echoJson 方法返回 json 字符串，客户端jquery可以直接使用
            echoJson(dataInDB);
        }
    }
}
