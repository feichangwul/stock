﻿using wojilu;
using wojilu.ORM;

namespace StockRoom.BLL
{
    public class Teacher : ObjectBase<Teacher>
    {
        [NotNull]
        public int RoomId { get; set; }
        [NotNull]
        public int PageNo { get; set; }
        [NotNull]
        public int MessageId { get; set; }
        [LongText]
        public string MessageInfo { get; set; }
        [NotNull]
        public string AddTime { get; set; }
        
        public string MessageType { get; set; }
        [LongText]
        public string ReplyMessageInfo { get; set; }
        
        public string ReplyAddTime { get; set; }
        
        public string ReplyUserName { get; set; }

        public string PicUrl { get; set; }

        public string ContentUrl { get; set; }
        public string LocalContentPath { get; set; }
        public string ThumbUrl { get; set; }
        public string LocalThumbPath { get; set; }
    }
}
