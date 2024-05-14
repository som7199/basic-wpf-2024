CREATE TABLE [dbo].[Restaurant](
   [Idx] [int] IDENTITY(1,1) NOT NULL,
   [Category] [nvarchar](10) NULL,
   [Name] [nvarchar](50) NULL,
   [Area] [nvarchar](50) NULL,
   [Address] [nvarchar](100) NULL,
   [Content] [nvarchar](200) NULL,
   [Holiday] [nvarchar](50) NULL,
   [Phone] [nvarchar](30) NULL,
   [Menuprice] [nvarchar](30) NULL,
   [Xposition] [float] NULL,
   [Yposition] [float] NULL,

 CONSTRAINT [PK_Restaurant] PRIMARY KEY
 (    [Idx] ASC ) ON [PRIMARY]
) ON [PRIMARY]
GO