CREATE TABLE [dbo].[CharacterAttributes] (
    [CharacterId]    UNIQUEIDENTIFIER NOT NULL,
    [CharacterLevel] INT              NOT NULL,
    [Name]           NVARCHAR (50)    NOT NULL,
    [Value]          NVARCHAR (70)    NOT NULL,
    [Dice]           NVARCHAR (25)    NULL,
    CONSTRAINT [PK_CharacterAttributes] PRIMARY KEY CLUSTERED ([CharacterId] ASC, [Name] ASC),
    CONSTRAINT [FK__Character__Chara__619B8048] FOREIGN KEY ([CharacterId]) REFERENCES [dbo].[Characters] ([CharacterId])
);

