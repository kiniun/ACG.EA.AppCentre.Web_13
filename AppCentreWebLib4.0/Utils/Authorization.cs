using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACG.EA.AppCentre.Lib.DAL;

namespace ACG.EA.AppCentre.Utils
{
    public class Authorization
    {
        protected const string ApplicationName = "AppCentre";
        public static List<string> GetUserAccessProfile(string userId)
        {
            var prof = new List<string>();

            using(var db = new AppCentreEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;
                var query = from perms in db.VW_USER_PERMISSION.Where(p => p.USER_NAME == userId && p.APPLICATION_ID == ApplicationName)
                            orderby perms.PERMISSION_ID
                            select perms.PERMISSION_ID;
                // query.ToList<PERMISSION>();
            }
            return prof;
        }
    }
}
