using System;
using System.ComponentModel;
using System.Text;

namespace ACG.EA.AppCentre.Lib.Models
{
    public class User_Permission
    {
        public User_Permission()
        { }

        [System.ComponentModel.DataAnnotations.Key]
        public int user_permission_Id { get; set; }
        public int user_Id { get; set; }
        public string permission_Id { get; set; }
        public string application_Id { get; set; }
        public Nullable<int> target_value_Id { get; set; }
        public bool grant { get; set; }
    }

    public class User
    {
        public User()
        { }

        public int? User_Id { get; set; }
        public string User_Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Create_Date { get; set; }

        public string Last_Modified { get; set; }
        public string Modified_By { get; set; }
    }

    public class UserPermissionProfile
    {
        public UserPermissionProfile()
        { }

        public int user_Id { get; set; }
        public string User_Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string permission_Id { get; set; }
        public string application_Id { get; set; }
        public string target_value { get; set; }
        public bool grant { get; set; }
    }

    [TypeConverter(typeof(PatchModel))]
    public class PatchPoint
    {
        public string id { get; set; }
        public string fieldToPatch { get; set; }

        public static bool TryParse(string s, out PatchPoint result)
        {
            result = null;

            var parts = s.Split(',');
            if (parts.Length != 2)
            {
                return false;
            }

            //string _id, _fieldToPatch;
            if (parts[0] != null && parts[1] != null)
            {
                result = new PatchPoint() { fieldToPatch = parts[1], id = parts[0] };
                return true;
            }
            return false;
        }
    }

    class PatchModel : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                PatchPoint point;
                if (PatchPoint.TryParse((string)value, out point))
                {
                    return point;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
