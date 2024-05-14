using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataAPI_som.Models
{
    public class Restaurant
    {
        public int Idx { get; set; }
        public string Category {  get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
        public string Address { get; set; }
        //public string BusinessHour { get; set; }
        public string Content { get; set; }
        public string Holiday { get; set; }
        public string Phone { get; set; }
        public string MainMenu { get; set; }
        public double Xposition { get; set; }
        public double Yposition { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT [Idx]
                                                             ,[Category]
                                                             ,[Name]
                                                             ,[Area]
                                                             ,[Address]
                                                             ,[Content]
                                                             ,[Holiday]
                                                             ,[Phone]
                                                             ,[MainMenu]
                                                             ,[Xposition]
                                                             ,[Yposition]
                                                        FROM [EMS].[dbo].[Restaurant]";
        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[Restaurant]
                                                                   ([Category]
                                                                   ,[Name]
                                                                   ,[Area]
                                                                   ,[Address]
                                                                   ,[Content]
                                                                   ,[Holiday]
                                                                   ,[Phone]
                                                                   ,[MainMenu]
                                                                   ,[Xposition]
                                                                   ,[Yposition])
                                                             VALUES
                                                                   (@Category
                                                                   ,@Name
                                                                   ,@Area
                                                                   ,@Address
                                                                   ,@Content
                                                                   ,@Holiday
                                                                   ,@Phone
                                                                   ,@MainMenu
                                                                   ,@Xposition
                                                                   ,@Yposition)";
    }
}
