using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class UsersInformation
    {
        public string mid;
        public int balance;
        public string user_name;
        public string permission;

        public UsersInformation()
        {

        }

        public UsersInformation(string mid,int balance,string user_name,string permission)
        {
            this.mid = mid;
            this.balance = balance;
            this.user_name = user_name;
            this.permission = permission;
        }
    }
}
