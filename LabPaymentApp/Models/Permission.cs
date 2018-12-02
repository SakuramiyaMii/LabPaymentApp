using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 権限定義クラス
/// 使用する際Permissions.permissionListを参照する
/// </summary>

namespace LabPaymentApp.Models
{
    public class Permission
    {
        public int _permissionId { get; set; }
        public string _permissionName { get; set; }
    }
    public class Permissions
    {
        public static List<Permission> permissionList = new List<Permission>
            {
            new Permission {_permissionId = 0, _permissionName = "未定義"},
            new Permission {_permissionId = 1, _permissionName = "利用者"},
            new Permission {_permissionId = 2, _permissionName = "仕入者"},
            new Permission {_permissionId = 3, _permissionName = "管理者"}
            };
    }
}
