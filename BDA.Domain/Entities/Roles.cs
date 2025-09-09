//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;

//namespace BDA.Entities
//{
//    public class Roles : Entity<Guid>
//    {
//        public Roles()
//        { 
//            Id = Guid.NewGuid();
//            this.SerializedAccessRights = "";
//        }
 
//        public string Name { get; set; }
//        public string SerializedAccessRights { get; set; }
//        public bool NeedDivision { get; set; }
//        public bool NeedFunction { get; set; }
//        public bool NeedZone { get; set; }
//        public bool NeedUnit { get; set; }

//        private string GetAccessRightString(AccessRights accessRight)
//        {
//            return String.Concat("{", accessRight, "}");
//        }

//        public Roles AddAccessRights(params AccessRights[] accessRights)
//        {
//            foreach (var accessRight in accessRights)
//                this.AddAccessRight(accessRight);

//            return this;
//        }

//        //-------------------------------------------------------------------------------- 

//        public void AddAccessRight(AccessRights accessRight)
//        {
//            var str = GetAccessRightString(accessRight);
//            if (this.SerializedAccessRights.Contains(str))
//                return;

//            this.SerializedAccessRights += str;
//        }

//        public void RemoveAccessRight(AccessRights accessRight)
//        {
//            this.SerializedAccessRights = this.SerializedAccessRights.Replace(GetAccessRightString(accessRight), "");
//        }

//        public bool HaveAccessRight(AccessRights accessRight)
//        {
//            return this.SerializedAccessRights.Contains(GetAccessRightString(accessRight));
//        }

//        public IEnumerable<AccessRights> GetAccessRightList()
//        {
//            return this.SerializedAccessRights
//                    .Split(new[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries)
//                    .Select(x => Enum.Parse(typeof(AccessRights), x))
//                    .Cast<AccessRights>();
//        }
//    }

//    public class RoleNames
//    {
//        public static string BusinessAdmin = "Business Admin";
//        public static string TGBSBanking= "TGBS Banking";
//        public static string TGBSRecon = "TGBS Reconcilation";
//        public static string Executive = "Executive";
//        public static string Manager = "Manager";
//        public static string SeniorManager = "SeniorManager";
//        public static string HeadOfZone = "Head Of Zone";
//        public static string GeneralManager = "General Manager";
//        public static string SeniorGeneralManager = "Senior General Manager";
//    }

//    public enum AccessRights
//    {
//        SuperUser,
//        ManageSettings,
//        RequestWangCagaranLess10M,
//        VerifyWangCagaranLess10M,
//        ApproveWangCagaranLess10M,
//        RequestWangCagaranMore10M,
//        VerifyWangCagaranMore10M,
//        ApproveWangCagaranMore10M,
//        RequestWangHangus,
//        ApproveWangHangus,
//        AcceptBankDraf
//    }
//}
