USE [STGenetics]
GO

/****** Object:  Table [dbo].[Animal]    Script Date: 12/05/2023 20:10:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Animal](
	[AnimalId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Breed] [int] NOT NULL,
	[BirthDate] [datetime] NOT NULL,
	[Sex] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Animals] PRIMARY KEY CLUSTERED 
(
	[AnimalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Animal]  WITH CHECK ADD  CONSTRAINT [FK_Animal_Breed] FOREIGN KEY([Breed])
REFERENCES [dbo].[Breed] ([BreedId])
GO

ALTER TABLE [dbo].[Animal] CHECK CONSTRAINT [FK_Animal_Breed]
GO

ALTER TABLE [dbo].[Animal]  WITH CHECK ADD  CONSTRAINT [FK_Animals_Sex] FOREIGN KEY([Sex])
REFERENCES [dbo].[Sex] ([SexId])
GO

ALTER TABLE [dbo].[Animal] CHECK CONSTRAINT [FK_Animals_Sex]
GO

ALTER TABLE [dbo].[Animal]  WITH CHECK ADD  CONSTRAINT [FK_Animals_Status] FOREIGN KEY([Status])
REFERENCES [dbo].[Status] ([StatusId])
GO

ALTER TABLE [dbo].[Animal] CHECK CONSTRAINT [FK_Animals_Status]
GO


