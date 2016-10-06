using System;
using System.Collections.Generic;
using System.Text;
using wojilu;
using wojilu.Web.Mvc;

namespace StockRoom.BLL.Controller
{

    public class LayoutController : ControllerBase {

        private static readonly ILog logger = LogManager.GetLogger( typeof( LayoutController ) );

        public LayoutController() {
        }

        public override void Layout() {

            logger.Info( "开始加载布局文件" );

            

        }


    }
}
