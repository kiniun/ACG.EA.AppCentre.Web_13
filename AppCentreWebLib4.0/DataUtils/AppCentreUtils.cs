using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACG.EA.AppCentre.DataUtils
{
    public class BLL_Base
    {
        protected readonly string ApplicationName;
        public BLL_Base(string appName)
        {
            ApplicationName = appName;
        }

        public BLL_Base()
        {

        }
    }

    public class AppCentreUtils: BLL_Base
    {
        //optional application name contructor
        //public AppCentreUtils(string appName)
        //    : base(appName)
        //{

        //}
        public AppCentreUtils()
            : base()
        {

        }


    }
}
