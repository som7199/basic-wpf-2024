using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataAPI_som.Models
{
    public class Restaurant
    {
        //public int Count { get; set; }     // 데이터 잘 넘어오나 확인용
        public int Idx { get; set; }
        public string? Category {  get; set; }
        public string? Name { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }
        public string? Content { get; set; }
        public string? Holiday { get; set; }
        public string? Phone { get; set; }
        //public string MainMenu { get; set; }  // 결측치가 너무 많다..
        public string? Xposition { get; set; }
        public string? Yposition { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT [Idx]
                                                             ,[Category]
                                                             ,[Name]
                                                             ,[Area]
                                                             ,[Address]
                                                             ,[Content]
                                                             ,[Holiday]
                                                             ,[Phone]
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
                                                                   ,@Xposition
                                                                   ,@Yposition)";
    }
}
