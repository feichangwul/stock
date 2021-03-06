﻿using System;
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

            int i = (int)DateTime.Today.DayOfWeek;
            //只在工作日和15点之后去抓取数据
            if (i == 0 || i == 6) return;
            bool flag = DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 24;

            if (flag)
            {
                try
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    //如果已经包含有文件,说明已经运行过一次
                    if (Directory.GetFiles(dir).Length > 0)
                        return;
                    logger.Debug("--------------------------FKW OFFLINE Executing START--------------------------");

                    StockOnline stk = new StockOnline();
                    string teachPageJson = stk.GetTeacherPageJson(roomId);
                    int pageNo = stk.ParseTeacherPageNo(teachPageJson);

                    int pageIndex = 0;
                    Encoding encoding = Encoding.GetEncoding("GB2312");

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
                    //insert into date table
                    int rowCount = StockDate.count(string.Format("CreatedDate='{0}'", date));
                    if (rowCount <= 0)
                    {
                        var entity = new StockDate
                        {
                            RoomId = roomId,
                            CreatedDate = date
                        };
                        db.insert(entity);
                    }
                    // save to DB from files
                    foreach (var item in Directory.GetFiles(dir))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(item);

                        var arr = fileName.Split('_');
                        int count = arr.Length;
                        if (count > 0)
                        {
                            string index = arr[count - 1];
                            int no = -1;
                            int.TryParse(index, out no);
                            if (no != -1)
                            {
                                stk.InsertHistory(item, roomId,no);
                            }
                        }
                    }

                    logger.Debug("--------------------------FKW Executing END--------------------------"); 
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
