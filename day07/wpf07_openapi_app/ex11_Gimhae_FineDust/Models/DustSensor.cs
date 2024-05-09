using Microsoft.Xaml.Behaviors.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex11_Gimhae_FineDust.Models
{
    public class DustSensor
    {
        /*
         * "dev_id":"B4E62D460C9E","name":"미세먼지19","loc":"서상동 306-24 수로왕릉입구 광장 가로등주","coordx":"128.87892793651744","coordy":"35.23352393922928","ison":false,"pm10_after":115,"pm25_after":69,"state":0,"timestamp":"2024-04-24 14:57:04.084","company_id":"bcdbe35acf834d64bf4e7ed5fdf1cf94","company_name":"미세먼지 센서"
         */
        public int Id { get; set; } // 추가생성, DB에 넣을때 사용할 값
        public string Dev_id { get; set; } // 디바이스아이디
        public string Name { get; set; } // 디바이스이름
        public string Loc { get; set; } // 디바이스 위치주소
        public double Coordx { get; set; } // 경도
        public double Coordy { get; set; } // 위도
        public bool Ison { get; set; } // 디바이스 온 여부
        public int Pm10_after { get; set; } // PM 10mm 미세먼지 측정값
        public int Pm25_after { get; set; } // PM 2.5mm 초미세먼지 측정값
        public int State { get; set; } // 상태?
        public DateTime Timestamp { get; set; } // 측정일시
        public string Company_id { get; set; } // 회사아이디?
        public string Company_name { get; set; } // 회사명

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[Dustsensor]
                                                                   ([Dev_id]
                                                                   ,[Name]
                                                                   ,[Loc]
                                                                   ,[Coordx]
                                                                   ,[Coordy]
                                                                   ,[Ison]
                                                                   ,[Pm10_after]
                                                                   ,[Pm25_after]
                                                                   ,[State]
                                                                   ,[Timestamp]
                                                                   ,[Company_id]
                                                                   ,[Company_name])
                                                             VALUES
                                                                   (@Dev_id
                                                                   ,@Name
                                                                   ,@Loc
                                                                   ,@Coordx
                                                                   ,@Coordy
                                                                   ,@Ison
                                                                   ,@Pm10_after
                                                                   ,@Pm25_after
                                                                   ,@State
                                                                   ,@Timestamp
                                                                   ,@Company_id
                                                                   ,@Company_name)";

        public static readonly string SELECT_QUERY = @"SELECT [Id]
                                                              ,[Dev_id]
                                                              ,[Name]
                                                              ,[Loc]
                                                              ,[Coordx]
                                                              ,[Coordy]
                                                              ,[Ison]
                                                              ,[Pm10_after]
                                                              ,[Pm25_after]
                                                              ,[State]
                                                              ,[Timestamp]
                                                              ,[Company_id]
                                                              ,[Company_name]
                                                          FROM [dbo].[Dustsensor]
                                                         WHERE CONVERT(CHAR(10), [Timestamp], 23) = @Timestamp";

        public static readonly string GETDATE_QUERY = @"SELECT CONVERT(CHAR(10), Timestamp, 23) AS Save_Date
                                                          FROM [dbo].[DustSensor]
                                                         GROUP BY CONVERT(CHAR(10), Timestamp, 23)";
    }
}