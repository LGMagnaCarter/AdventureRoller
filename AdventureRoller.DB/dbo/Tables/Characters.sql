CREATE TABLE [dbo].[Characters] (
    [CharacterId] UNIQUEIDENTIFIER NOT NULL,
    [DiscordId]   DECIMAL (20)     NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Edition]     NVARCHAR (50)    NOT NULL,
    [Active]      BIT              NOT NULL,
    [Level]       INT              NOT NULL,
    CONSTRAINT [PK_Character] PRIMARY KEY CLUSTERED ([CharacterId] ASC)
);

