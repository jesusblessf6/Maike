using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class CompanyVM
    {
        public int Id { get; set; }
        private string name;
        [Required(AllowEmptyStrings = false, ErrorMessage = "公司简称不能为空！")]
        public string Name
        {
            get
            {
                if (name != null)
                    return name.Trim();
                else
                    return "";
            }
            set
            {
                name = value;
            }
        }
        private string fullName;
        [Required(AllowEmptyStrings = false, ErrorMessage = "公司全称不能为空！")]
        public string FullName
        {
            get
            {
                if (fullName != null)
                    return fullName.Trim();
                else
                    return "";
            }
            set
            {
                fullName = value;
            }
        }
        private string address;
        public string Address
        {
            get
            {
                if (address != null)
                    return address.Trim();
                else
                    return "";
            }
            set
            {
                address = value;
            }
        }
        private string zip;
        public string Zip
        {
            get {
                if (zip != null)
                    return zip.Trim();
                else
                    return "";
            }
            set
            {
                zip = value;
            }
        }
        private string comment;
        public string Comment
        {
            get
            {
                if (comment != null)
                    return comment.Trim();
                else
                    return "";
            }
            set
            {
                comment = value;
            }
        }
        public int Type { get; set; }
    }
}
