//using FHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using FHubPanel.Models;

namespace FHub.Controllers
{
    public static class CommanClass
    {
        static FHubDBEntities db = new FHubDBEntities();
        [HttpPost]
        public static void ManageError(Exception ex)
        {
            ErrLog _ObjErr = new ErrLog();
            _ObjErr.ErrDate = System.DateTime.Now;
            _ObjErr.ErrCode = ex.Source;
            _ObjErr.ErrMethod = ex.StackTrace;
            _ObjErr.ErrDesc = ex.Message;
            //_ObjErr.ErrInternal = ex.InnerException == null? "" : ex.InnerException.ToString();
            db.ErrLogs.Add(_ObjErr);
            db.SaveChanges();
        }
    }
}