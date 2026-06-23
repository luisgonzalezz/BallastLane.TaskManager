USE BallastLaneTaskManager;
GO

DECLARE @DemoUserId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @Now DATETIME2(7) = SYSUTCDATETIME();
DECLARE @DemoPasswordHash NVARCHAR(512) = N'ctZQs2MfF64ii5QjAQOtGg==.+yd2b0SiPmazK9t8A3riv+6QOlgNy85/VAevlFHnZ9k=';

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @DemoUserId)
BEGIN
    INSERT INTO dbo.Users (Id, Email, PasswordHash, CreatedAt)
    VALUES
    (
        @DemoUserId,
        N'demo@ballastlane.com',
        @DemoPasswordHash,
        @Now
    );
END;
ELSE
BEGIN
    UPDATE dbo.Users
    SET
        Email = N'demo@ballastlane.com',
        PasswordHash = @DemoPasswordHash
    WHERE Id = @DemoUserId;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Tasks WHERE UserId = @DemoUserId)
BEGIN
    INSERT INTO dbo.Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAt, UpdatedAt)
    VALUES
    (
        '22222222-2222-2222-2222-222222222222',
        @DemoUserId,
        N'Prepare technical interview',
        N'Review Clean Architecture, ADO.NET, JWT, and testing approach.',
        1,
        DATEADD(DAY, 3, @Now),
        @Now,
        @Now
    ),
    (
        '33333333-3333-3333-3333-333333333333',
        @DemoUserId,
        N'Build API endpoints',
        N'Implement auth and task CRUD endpoints after repositories are ready.',
        2,
        DATEADD(DAY, 5, @Now),
        @Now,
        @Now
    );
END;
GO
