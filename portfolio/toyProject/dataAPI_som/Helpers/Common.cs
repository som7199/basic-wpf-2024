﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataAPI_som.Helpers
{
    public class Common
    {
        public static readonly string CONNSTRING = "Data Source=localhost;" +
                                                    "Initial Catalog=EMS;" +
                                                    "Persist Security Info=True;" +
                                                    "User ID=ems_user;" +
                                                    "Encrypt=False;" +
                                                    "Password=mssql_p@ss";
    }
}
