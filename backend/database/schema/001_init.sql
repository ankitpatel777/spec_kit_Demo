IF OBJECT_ID(N'dbo.Albums', N'U') IS NULL
CREATE TABLE dbo.Albums (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Albums PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    CoverPhotoId UNIQUEIDENTIFIER NULL,
    CoverImagePath NVARCHAR(1024) NULL,
    StartDate DATETIME2 NULL,
    EndDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    IsDateGroup BIT NOT NULL CONSTRAINT DF_Albums_IsDateGroup DEFAULT 0
);

IF OBJECT_ID(N'dbo.Photos', N'U') IS NULL
CREATE TABLE dbo.Photos (
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Photos PRIMARY KEY,
    FileName NVARCHAR(255) NOT NULL,
    OriginalPath NVARCHAR(1024) NOT NULL,
    CaptureDate DATETIME2 NULL,
    ImportedAtUtc DATETIME2 NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    UpdatedAtUtc DATETIME2 NOT NULL,
    FileModifiedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Photos_FileModifiedAtUtc DEFAULT SYSUTCDATETIME(),
    Hash NVARCHAR(128) NULL,
    ContentType NVARCHAR(128) NULL,
    FileSizeBytes BIGINT NOT NULL
);

IF COL_LENGTH(N'dbo.Photos', N'FileModifiedAtUtc') IS NULL
ALTER TABLE dbo.Photos ADD FileModifiedAtUtc DATETIME2 NOT NULL
    CONSTRAINT DF_Photos_FileModifiedAtUtc DEFAULT SYSUTCDATETIME();

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Albums_CoverPhoto')
ALTER TABLE dbo.Albums ADD CONSTRAINT FK_Albums_CoverPhoto
    FOREIGN KEY (CoverPhotoId) REFERENCES dbo.Photos(Id) ON DELETE SET NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Photos_OriginalPath' AND object_id = OBJECT_ID(N'dbo.Photos'))
CREATE UNIQUE INDEX IX_Photos_OriginalPath ON dbo.Photos (OriginalPath);

IF OBJECT_ID(N'dbo.AlbumPhotos', N'U') IS NULL
CREATE TABLE dbo.AlbumPhotos (
    AlbumId UNIQUEIDENTIFIER NOT NULL,
    PhotoId UNIQUEIDENTIFIER NOT NULL,
    SortOrder INT NOT NULL,
    AssignDateUtc DATETIME2 NULL,
    CONSTRAINT PK_AlbumPhotos PRIMARY KEY (AlbumId, PhotoId),
    CONSTRAINT FK_AlbumPhotos_Albums FOREIGN KEY (AlbumId) REFERENCES dbo.Albums(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AlbumPhotos_Photos FOREIGN KEY (PhotoId) REFERENCES dbo.Photos(Id) ON DELETE CASCADE,
    CONSTRAINT CK_AlbumPhotos_SortOrder CHECK (SortOrder >= 0)
);
