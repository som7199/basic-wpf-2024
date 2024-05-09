CREATE TABLE [dbo].[Dustsensor](
   [Id] [int] IDENTITY(1,1) NOT NULL,
   [Dev_id] [varchar](20) NULL,
   [Name] [varchar](20) NULL,
   [Loc] [nvarchar](100) NULL,
   [Coordx] [float] NULL,
   [Coordy] [float] NULL,
   [Ison] [bit] NULL,
   [Pm10_after] [int] NULL,
   [Pm25_after] [int] NULL,
   [State] [int] NULL,
   [Timestamp] [datetime] NULL,
   [Company_id] [varchar](50) NULL,
   [Company_name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Dustsensor] PRIMARY KEY
 (    [Id] ASC ) ON [PRIMARY]
) ON [PRIMARY]
GO