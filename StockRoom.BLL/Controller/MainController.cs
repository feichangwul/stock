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
            //List<Teacher> dataInDB = Teacher.findBySql("SELECT TOP 1 * FROM TEACHER ORDER BY id desc");

            //bindList("list", "teacher", dataInDB);
            Int64 id = 332;
            List<Teacher> dataInDB = db.find<Teacher>("roomId = :roomId and StrComp(:now,Left(addtime,10)) = 0")
                .set("roomId", id)
                .set("now", DateTime.Now.ToString("yyyy-MM-dd"))
                    .list();

            //List<Teacher> dataInDB = Teacher.findAll();

            IBlock block = getBlock("list");
            foreach (var item in dataInDB)
            {
                //block.Set("teacher.RoomId", item.RoomId);
                block.Set("teacher.AddTime", item.AddTime);
                string thumbPicURL = item.ThumbUrl;
                if (string.IsNullOrEmpty(thumbPicURL))
                {
                    block.Set("Content", item.MessageInfo);
                }
                else
                {
                    block.Set("Content", string.Format("<img src='{0}'  alt='pic' />",thumbPicURL));
                }
                block.Next();
            }
        }

        public virtual void Show(Int64 id)
        {
            List<Teacher> dataInDB = db.find<Teacher>("roomId = :roomId and StrComp(:now,Left(addtime,10)) = 0")
                    .set("roomId", id)
                    .set("now", DateTime.Now.ToString("yyyy-MM-dd"))
                    .list();

            IBlock block = getBlock("list");
            foreach (var item in dataInDB)
            {
                //block.Set("teacher.RoomId", item.RoomId);
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

        public virtual void JsonResult()
        {

            List<Teacher> dataInDB = Teacher.findAll();

            // 使用 echoJson 方法返回 json 字符串，客户端jquery可以直接使用
            echoJson(dataInDB);
        }
    }
}
