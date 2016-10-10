using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.IO;
using wojilu.Data;
using System.Data;
using wojilu;
using wojilu.ORM;
using System.Collections.Generic;
using StockRoom.BLL;
using System.Threading;

namespace UnitTestStockRoom
{

    [TestClass]
    public class TestSockOnline
    {
        [TestMethod]
        public void TestDbConnection()
        {
            int count = MappingClass.Instance.ClassList.Count;
            IDbConnection connection = DbContext.getConnection(typeof(Teacher));

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            else
            {
                connection.Close();
                connection.Open();
            }
        }

        [TestMethod]
        public void TestDbInsert()
        {
            //int count = MappingClass.Instance.ClassList.Count;
            //IDbConnection connection = DbContext.getConnection(typeof(Teacher));

            //if (connection.State == ConnectionState.Closed)
            //{
            //    connection.Open();
            //}
            //else
            //{
            //    connection.Close();
            //    connection.Open();
            //}
            Teacher t = new Teacher();
            t.MessageId = 111;
            t.MessageInfo = "AA";
            try
            {
                db.insert(t);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [TestMethod]
        public void TestGetThumbSrcUrl()
        {
            StockOnline stk = new StockOnline();
            //string teachPageJson = stk.GetTeacherPageJson();
            //int pageNo = stk.ParseTeacherPageNo(teachPageJson);

            //string teachJson = stk.GetTeacherJson(332, 0);
            //string teachJson = @"[{'__type':'BusinessFacade.ChatMessageBackup, BusinessFacade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null','_id':8191414,'MessageInfo':'\u003cfont style=\'font-family:微软雅黑;font-size:12;color:;\'\u003e\u003ca class=\'single_image\' href=\'http://photo26.hexun.com/p/2016/0809/578749/o_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg\'\u003e\u003cimg src=\'http://photo26.hexun.com/p/2016/0809/578749/t150_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg\'\u003e\u003c/a \u003e\u003c/font\u003e','Contenturl':'http://photo26.hexun.com/p/2016/0809/578749/o_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg','Picurl':'','FontColor':'red','SoundTime':'','UserID':4128648,'UserName':'徐小明','RoomID':332,'RoomName':' 徐小明文字互动课堂','AddTime':'2016-08-09 10:46:37','MessageType':2,'ReplyMessageID':8191295,'ReplyMessageInfo':'我刚刚睡了一觉，爬起来看到上证过了趋势线 仓位10了[facexiayan]\n','ReplyUserID':26590463,'ReplyUserName':'海天593','ReplyAddTime':'2016-08-09 10:44:14','IsTop':0,'MessageSize':'18px','MessageFont':'微软雅黑','MessageColor':'http://logo0.tool.hexun.com/88ff3e-40.jpg','IsDelete':0,'IP':'114.241.190.146','IsCheck':0,'Level':0,'Zan':0,'SourceText':null,'IsFirstTeacher':1}]";
            //stk.ParseTeacherJson(teachJson);
            //<div class="getmsginfo" style="font-family:微软雅黑;font-size:18px;color:red;line-height:24px;display:inline-block">早</div>
            //<font style='font-family:微软雅黑;font-size:12;color:;'><a class='single_image' 
            //href='http://photo26.hexun.com/p/2016/0809/578749/o_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg'>
            //<img src='http://photo26.hexun.com/p/2016/0809/578749/t150_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg'></a ></font>
            string testStr = @"<font style='font-family:微软雅黑;font-size:12;color:;'><a class='single_image' href='http://photo26.hexun.com/p/2016/0809/578749/o_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg'><img src='http://photo26.hexun.com/p/2016/0809/578749/t150_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg'></a ></font>";
            //string testStr = "<a class='single_image' href='http://photo26.hexun.com/p/2016/0809/578749/o_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg'>";
            string url = stk.GetImageUrl(testStr);
        }

        [TestMethod]
        public void TestGetThumbSrcUrlWithNoUrl()
        {
            StockOnline stk = new StockOnline();

            string testStr = @"<div class='getmsginfo' style='font-family:微软雅黑;font-size:18px;color:red;line-height:24px;display:inline-block'>早</div>";
            stk.GetImageUrl(testStr);
        }
        [TestMethod]
        public void TestValidRoom()
        {
            for (int i = 180; i < 1000; i++)
            {
                StockOnline stk = new StockOnline();
                string teachPageJson = stk.GetTeacherPageJson(i);
                int pageNo = stk.ParseTeacherPageNo(teachPageJson);
                if (pageNo > 0)
                {
                    stk.SaveFile(string.Format("{0} \r\n", pageNo), string.Format(@"C:\jinzhao\StockRoom\data\{0}_{1}.txt",i,pageNo), Encoding.GetEncoding("GB2312"));
                }
                Random rd = new Random();
                int sleepSecond = rd.Next(1, 2);
                Thread.Sleep(sleepSecond * 1000);
            }
        }

        [TestMethod]
        public void TestSaveFile()
        {
            int roomId = 333;
            StockOnline stk = new StockOnline();
            string teachPageJson = stk.GetTeacherPageJson(roomId);
            int pageNo = stk.ParseTeacherPageNo(teachPageJson);

            int pageIndex = 0;
            Encoding encoding = Encoding.GetEncoding("GB2312");
            string date = DateTime.Now.ToString("yyyyMMdd");
            string dir = string.Format("{0}\\{1}\\{2}", @"C:\jinzhao\StockRoom\data", roomId, date);

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
                    stk.SaveFile(teachJson, absolutePath, encoding);
                }
            }
        }

        [TestMethod]
        public void TestFetchOnlineData()
        {
            int roomId = 332;
            StockOnline stk = new StockOnline();
            stk.FetchOnlineData(roomId);
        }

        [TestMethod]
        public void TestParseJsonFromFile()
        {
            int roomId = 332;
            StockOnline stk = new StockOnline();
            Encoding encoding = Encoding.GetEncoding("GB2312");
            string date = DateTime.Now.ToString("yyyyMMdd");
            string dir = string.Format("{0}\\{1}\\{2}", @"C:\jinzhao\StockRoom\data", roomId, date);
            List<TeacherHis> alTeacherHis = new List<TeacherHis>();
            List<Teacher> alTeachers = new List<Teacher>();
            foreach (var file in Directory.GetFiles(dir))
            {
                alTeachers = stk.ParseJsonFromFile(file, encoding);
                foreach (var item in alTeachers)
                {
                    TeacherHis his = new TeacherHis
                    {
                        RoomId = item.RoomId,
                        MessageInfo = item.MessageInfo,
                        MessageId = item.MessageId,
                        AddTime = item.AddTime,
                        ReplyMessageInfo = item.ReplyMessageInfo,
                        ReplyAddTime = item.ReplyAddTime,
                        ReplyUserName = item.ReplyUserName,
                        ContentUrl = item.ContentUrl,
                        ThumbUrl = item.ThumbUrl,
                        CreatedDateTime = DateTime.Now
                    };
                    string imgDir = string.Format("{0}\\{1}\\{2}", @"C:\jinzhao\StockRoom\imgs", roomId, date);
                    stk.DownloadTeacherImages(item, imgDir);
                    db.insert(his);
                }
            }

        }
         [TestMethod]
        public void TestDownloadImage()
        {
            StockOnline stk = new StockOnline();
            string str = @"http://photo26.hexun.com/p/2016/0809/578749/o_vip_5A30CCFCF1AC23B9837A6377F9FEBC70.jpg";
            int index = str.LastIndexOf("/");
            string imgPath = string.Format("./imgs/{0}",str.Substring(index + 1));
            stk.DownloadImage(str, imgPath);
        }

        [TestMethod]
        public void TestGetMessageContent()
        {
            string str = @"<div class='getmsginfo' style='font-family:微软雅黑;font-size:18px;color:red;line-height:24px;display:inline-block'>早</div>";

            StockOnline stk = new StockOnline();
            stk.GetMessageContent(str);
        }
    }
}
