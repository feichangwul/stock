using System;
using System.IO;
using System.Text;
using wojilu;
using wojilu.Web.Jobs;

namespace StockRoom.BLL.Jobs
{
    public class FKWRoomOfflineJob : IWebJobItem
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FKWRoomOfflineJob));

        public FKWRoomOfflineJob()
        {

        }

        public void Execute()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            int roomId = 333;
            string dir = string.Format("{0}\\data\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, roomId, date);
            //如果已经包含有文件,说明已经运行过一次
            if (Directory.GetFiles(dir).Length > 0)
                return;


            int i = (int)DateTime.Today.DayOfWeek;
            //只在工作日和15点之后去抓取数据
            bool flag = i != 0 && i != 6;
            flag = DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 23;

            if (flag)
            {
                try
                {
                    StockOnline stk = new StockOnline();
                    string teachPageJson = stk.GetTeacherPageJson(roomId);
                    int pageNo = stk.ParseTeacherPageNo(teachPageJson);

                    int pageIndex = 0;
                    Encoding encoding = Encoding.GetEncoding("GB2312");

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    for (; pageIndex < pageNo; pageIndex++)
                    {
                        string teachJson = stk.GetTeacherJson(roomId, pageIndex);
                        if (teachJson.Length > 100)
                        {
                            string absolutePath = string.Format("{0}\\{1}_{2}_{3}.txt", dir, date, roomId, pageIndex);
                            if (File.Exists(absolutePath))
                            {
                                file.Delete(absolutePath);
                            }
                            stk.SaveFile(teachJson, absolutePath, encoding);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    throw;
                }
            }
        }

        public void End()
        {

        }
    }
}
