using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACG.EA.AppCentre.Web
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder strBuilder = new StringBuilder();

            foreach (var msg in Context.AllErrors)
            {
                strBuilder.Append(msg + "\n");
            }
            this.Msg.Text = "A system error has occurred. Please try again, and if the error persists, contact the Help Centre.\n"
                + " Error Detail: \n" + strBuilder;
        }
    }
}