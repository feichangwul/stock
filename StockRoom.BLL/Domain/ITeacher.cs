namespace StockRoom.BLL
{
    public interface ITeacher
    {
        int RoomId { get; set; }
        int PageNo { get; set; }
        int MessageId { get; set; }
        string MessageInfo { get; set; }
         string AddTime { get; set; }

         string MessageType { get; set; }
         string ReplyMessageInfo { get; set; }

         string ReplyAddTime { get; set; }

         string ReplyUserName { get; set; }

         string PicUrl { get; set; }

         string ContentUrl { get; set; }
         string LocalContentPath { get; set; }
         string ThumbUrl { get; set; }
         string LocalThumbPath { get; set; }
    }
}