using System;
using System.Collections.Generic;
using wojilu;
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
            List<Teacher> dataInDB = Teacher.findAll();

            //List<Teacher> dataInDB = db.find<Teacher>("StrComp(:now,Left(addtime,10)) = 0")
            //        .set("now", DateTime.Now.ToString("yyyy-MM-dd")).list();

            bindList("list", "teacher", dataInDB);
        }

        public virtual void Show(Int64 id)
        {
            List<Teacher> dataInDB = db.find<Teacher>("roomId = :roomId")
                    .set("roomId", id).list();

            bindList("list", "teacher", dataInDB);
        }

        public virtual void JsonResult()
        {

            List<Teacher> dataInDB = Teacher.findAll();

            // 使用 echoJson 方法返回 json 字符串，客户端jquery可以直接使用
            echoJson(dataInDB);
        }
    }
}
