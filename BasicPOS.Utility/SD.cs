using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicPOS.Utility
{
    public static class SD
    {
        public const string CartStatus_InProgress = "INPROGRESS";
        public const string CartStatus_Done = "DONE";
        public const string CartStatus_CANCELLED = "CANCELLED";

        public const string Role_Admin = "ADMIN";
        public const string Role_Customer = "CUSTOMER";

        public const string OrderStatus_Paid = "PAID";
        public const string OrderStatus_Done = "DONE";
        public const string OrderStatus_Cancelled = "CANCELLED";

    }
}
