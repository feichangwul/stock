using System;
using System.IO;
using System.Text;
using wojilu;
using wojilu.Web.Jobs;

namespace StockRoom.BLL.Jobs
{
    public class FKWRoomOnlineJob : IWebJobItem
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FKWRoomOnlineJob));

        public FKWRoomOnlineJob()
        {

        }

        public void Execute()
        {
            int i = (int)DateTime.Today.DayOfWeek;
            //只在工作日
            bool flag = i != 0 && i != 6;
            if (!flag) return;
            //9:30以后开始抓取数据
            //12点到1点休息
            flag = DateTime.Now.Hour != 12 && ((DateTime.Now.Hour > 9) || (DateTime.Now.Hour == 9 && DateTime.Now.Minute >= 30)) && DateTime.Now.Hour < 15;
            
            if (flag)
            {
                logger.Debug(string.Format("------------Data: {0}", DateTime.Now.Hour));
                int roomId = 333;
                logger.Debug("------------START to fetch data : RoomID 333----------");
                StockOnline stk = new StockOnline();
                stk.FetchOnlineData(roomId);
                logger.Debug("------------END to fetch : RoomID 333----------");
            }
        }

        public void End()
        {
            
        }
    }
}
