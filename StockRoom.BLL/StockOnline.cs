using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using wojilu;

namespace StockRoom.BLL
{
    public class StockOnline
    {
        public const string BASEADDRESS = "http://zhibo.hexun.com/";
        private static readonly ILog logger = LogManager.GetLogger(typeof(StockOnline));
        public void FetchOnlineData(int roomId)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            string imgDir = string.Format("{0}\\imgs\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, roomId, date);

            //
            Dictionary<int, Teacher> teacherDic = new Dictionary<int, Teacher>();

            string teachPageJson = GetTeacherPageJson(roomId);
            int pageNo = ParseTeacherPageNo(teachPageJson) - 1;
            logger.Debug(string.Format("The current Page No : {0} ",pageNo));
            string teachJson = "";
            List<Teacher> teacherList = null;

            int currentPageIndex = -1;
            //获取数据库最大的pageNo
            string condition = string.Format(" roomId = {0} and StrComp('{1}',Left(addtime,10)) = 0 ", roomId, DateTime.Now.ToString("yyyy-MM-dd"));
            Teacher latestTeacher = Teacher.findOne(condition);
            if (latestTeacher != null)
            {
                currentPageIndex = latestTeacher.PageNo;
            }
            //如果还是同一页，就只增加新记录
            if (pageNo == currentPageIndex)
            {
                teachJson = GetTeacherJson(roomId, pageNo);
                teacherList = ParseTeacherJson(teachJson);
                logger.Debug(string.Format("The total count of page is : {0} ", teacherList.Count));

                //获取数据库中已经保存的记录
                //只取当天，当前最新页面的数据
                List<Teacher> dataInDB = db.find<Teacher>("roomId = :roomId and StrComp(:now,Left(addtime,10)) = 0 and pageno= :pageNo")
                    .set("roomId", roomId)
                    .set("now", DateTime.Now.ToString("yyyy-MM-dd"))
                    .set("pageNo", currentPageIndex).list();
                logger.Debug(string.Format("The total count in DB is : {0} ", dataInDB.Count));

                foreach (var teach in teacherList)
                {
                    if (dataInDB.Find(x => x.MessageId == teach.MessageId) == null)
                    {
                        //insert into db
                        teach.PageNo = pageNo;

                        DownloadTeacherImages(teach, imgDir);
                        db.insert(teach);
                    }
                }
            }
            else//新的一页，直接添加所有记录
            {
                int pageNextIndex = 0;
                if (currentPageIndex >= 0) pageNextIndex = currentPageIndex + 1;

                //为保险起见，删除可能存在的多余记录
                condition = string.Format(" roomId = {0} and StrComp('{1}',Left(addtime,10)) = 0 and pageno >= {2}", roomId, DateTime.Now.ToString("yyyy-MM-dd"), pageNextIndex);
                db.deleteBatch<Teacher>(condition);
                Random rd = new Random();
                for (; pageNextIndex <= pageNo; pageNextIndex++)
                {
                    teachJson = GetTeacherJson(roomId, pageNextIndex);
                    teacherList = ParseTeacherJson(teachJson);
                    logger.Debug(string.Format("The total count of page-{0} is : {1} ", pageNextIndex, teacherList.Count));

                    foreach (var teach in teacherList)
                    {
                        //insert into db
                        teach.PageNo = pageNextIndex;
                        DownloadTeacherImages(teach, imgDir);
                        db.insert(teach);
                    }
                    int sleepSecond = rd.Next(2, 8);
                    Thread.Sleep(sleepSecond * 1000);
                }
            }
        }
        public bool InsertHistory(string filePath,int roomId, int pageNo)
        {
            Encoding encoding = Encoding.GetEncoding("GB2312");
            //Read Data from files and parse it into list
            var teacherList = ParseJsonFromFile(filePath, encoding);
            //delete before insert
            string condition = string.Format(" roomId = {0} and StrComp('{1}',Left(addtime,10)) = 0 and pageno = {2}", roomId, DateTime.Now.ToString("yyyy-MM-dd"), pageNo);
            db.deleteBatch<TeacherHis>(condition);
           
            foreach (var teach in teacherList)
            {
                //insert into db
                teach.PageNo = pageNo;
                teach.StockDate = DateTime.Now.ToString("yyyy/MM/dd");
                try
                {
                    db.insert(teach);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    return false;
                }
                
            }
            return true;
        }
        private String OpenRead(string baseAddress, string resourceUrl)
        {
            string result = string.Empty;
            WebClient wc = new WebClient();
            wc.BaseAddress = baseAddress;   //设置根目录
            wc.Encoding = Encoding.UTF8;                    //设置按照何种编码访问，如果不加此行，获取到的字符串中文将是乱码

            //----------------------以下为OpenRead()以流的方式读取----------------------
            Encoding encoding = Encoding.GetEncoding("GB2312");
            wc.Headers.Add("Accept", "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*");
            wc.Headers.Add("Accept-Language", "zh-cn");
            wc.Headers.Add("UA-CPU", "x86");
            //wc.Headers.Add("Accept-Encoding","gzip, deflate");    //因为我们的程序无法进行gzip解码所以如果这样请求获得的资源可能无法解码。当然我们可以给程序加入gzip处理的模块 那是题外话了。
            wc.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
            //Headers   用于添加添加请求的头信息
            //获取访问流
            try
            {
                using (Stream objStream = wc.OpenRead(resourceUrl))
                {
                    System.IO.StreamReader _read = new System.IO.StreamReader(objStream, encoding);    //新建一个读取流，用指定的编码读取，此处是utf-8
                    result = _read.ReadToEnd();   //输出读取到的字符串
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return result;
        }

        public string GetTeacherPageJson(int roomId)
        {
            string resourceUrl = string.Format("/AjaxPage/AjaxHandler.ashx?MethodName=getmsgpagecount&RoomID={0}&pageindex=0&msgid=0", roomId);
            return OpenRead(BASEADDRESS, resourceUrl);
        }
        /// <summary>
        /// {"allPageCount":"337","teacherPageCount":"337","allCount":"6728","pagemsgcount":"20"}
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public int ParseTeacherPageNo(string jsonString)
        {
            int result = -1;
            try
            {
                JToken json = JValue.Parse(jsonString);
                result = json["teacherPageCount"].ToObject<int>();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return result;
        }
        public string GetTeacherJson(int roomId, int pageIndex)
        {
            string resourceUrl = string.Format("/AjaxPage/AjaxHandler.ashx?MethodName=FristDisplayByMegType&RoomID={0}&msgtype=1&pageindex={1}&_=1468834794608", roomId, pageIndex);
            return OpenRead(BASEADDRESS, resourceUrl);
        }
        /// <summary>
        /// 从文件获取json数据，并解析
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public List<TeacherHis> ParseJsonFromFile(String absolutePath, Encoding encoding)
        {
            var alTeachers = new List<TeacherHis>();
            if (File.Exists(absolutePath))
            {
                string json = ReadFile(absolutePath, encoding);
                if (!string.IsNullOrEmpty(json))
                {
                    ParseTeacherJson(json).ForEach(x =>
                        alTeachers.Add(new TeacherHis
                        {
                            RoomId = x.RoomId,
                            MessageInfo = x.MessageInfo,
                            AddTime = x.AddTime,
                            ReplyMessageInfo = x.ReplyMessageInfo,
                            ReplyAddTime = x.ReplyAddTime,
                            PicUrl = x.PicUrl,
                            ThumbUrl = x.ThumbUrl,
                            LocalThumbPath = x.LocalThumbPath
                        }));
                }
            }
            return alTeachers;
        }

        public List<Teacher> ParseTeacherJson(string json)
        {
            List<Teacher> alTeachers = new List<Teacher>();
            try
            {
                JArray jsonObj = JArray.Parse(json);

                foreach (JObject jObject in jsonObj)
                {
                    string message = jObject["MessageInfo"].ToString();

                    string replyMessageInfo = "";
                    string replyAddTime = "";
                    string thumbUrl = "";
                    string hrefUrl = string.Empty;
                    //如果是包含在<div>中，可以忽略reply的message
                    if (!message.StartsWith("<div"))
                    {
                        replyMessageInfo = jObject["ReplyMessageInfo"].ToString();
                        replyAddTime = jObject["ReplyAddTime"].ToString();
                    }
                    else
                    {
                        thumbUrl = GetImageUrl(message);
                        hrefUrl = GetHrefUrl(message);
                        message = GetMessageContent(message);
                        //如果信息是图片
                        if (!string.IsNullOrEmpty(thumbUrl))
                        {
                            message = "";
                        }
                    }
                    var teach = new Teacher
                    {
                        RoomId = (int)jObject["RoomID"],
                        PageNo = -999,
                        MessageInfo = message,
                        MessageId = (int)jObject["_id"],
                        AddTime = jObject["AddTime"].ToString(),
                        ReplyMessageInfo = replyMessageInfo,
                        ReplyAddTime = replyAddTime,
                        ReplyUserName = jObject["ReplyUserName"].ToString(),
                        ContentUrl = jObject["Contenturl"].ToString(),
                        PicUrl = hrefUrl,
                        ThumbUrl = thumbUrl
                    };

                    alTeachers.Add(teach);
                }
                return alTeachers;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw;
            }

        }

        public void DownloadTeacherImages(Teacher teach, string basePath)
        {
            string str = teach.PicUrl;
            if (!string.IsNullOrEmpty(str))
            {
                int index = str.LastIndexOf("/");
                if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                string imgPath = string.Format("{0}\\{1}", basePath, str.Substring(index + 1));
                teach.LocalContentPath = imgPath;
                DownloadImage(str, imgPath);
                
            }
            //download the thumb picture
            str = teach.ThumbUrl;
            if (!string.IsNullOrEmpty(str))
            {
                int index = str.LastIndexOf("/");
                if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
                string imgPath = string.Format("{0}\\{1}", basePath, str.Substring(index + 1));
                teach.LocalThumbPath = imgPath;
                DownloadImage(str, imgPath);
            }
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetMessageContent(string value)
        {
            string result = string.Empty;
            string pattern = @"<.+?>(.+?)<.+?>";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(value);
            result = match.Groups[1].Value;
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string GetImageUrl(string value)
        {
            string result = string.Empty;

            string pattern = "img src=\"(http://.+\\.jpg)\">";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(value);
            result = match.Groups[1].Value;
            return result;
        }

        public string GetHrefUrl(string value)
        {
            string result = string.Empty;

            string pattern = "href=\"(http://.+\\.jpg)(\"><img)";
            Regex reg = new Regex(pattern);
            Match match = reg.Match(value);
            result = match.Groups[1].Value;
            return result;
        }

        public void DownloadImage(string url, string path)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;

            req.ContentType = "image/png";
            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

            System.IO.Stream stream = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                using (FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buff = new byte[512];
                    int c = 0; //实际读取的字节数
                    while ((c = stream.Read(buff, 0, buff.Length)) > 0)
                    {
                        writer.Write(buff, 0, c);
                    }
                }
            }
            finally
            {
                // 释放资源
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        public void DownloadImageWithWebClient(string url, string path)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, path);
            }
        }
        /// <summary>
        /// 将字符串写入某个文件中(需要指定文件编码方式)
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="fileContent">需要写入文件的字符串</param>
        /// <param name="encoding">编码方式</param>
        public void SaveFile(string content, string absolutePath, Encoding encoding)
        {
            using (StreamWriter writer = new StreamWriter(absolutePath, false, encoding))
            {
                writer.Write(content);
            }
        }

        /// <summary>
        /// 以某种编码方式，读取文件的内容
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>文件的内容</returns>
        public String ReadFile(String absolutePath, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(absolutePath, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
