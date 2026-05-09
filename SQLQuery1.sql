use LanguageExamDB;
GO

-- Roles Table to define if u user in instructor or student
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

--  Users Table handles logging in
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY, -- IDENTITY(1,1) means the database starts at 1 and counts up automatically, makes it automatic so it's easier to apply with c#
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL, 
    RoleID INT,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

--  ExamLevels Table to  categorize the exam levels
CREATE TABLE ExamLevels (
    LevelID INT IDENTITY(1,1) PRIMARY KEY, 
    LevelName VARCHAR(50) NOT NULL
);

--  ExamSessions Table for the actual test and the date
CREATE TABLE ExamSessions (
    SessionID INT IDENTITY(1,1) PRIMARY KEY,
    SessionName VARCHAR(100) NOT NULL,
    ExamDate DATETIME NOT NULL,
    TotalSeats INT CHECK (TotalSeats >= 0), -- Prevents negative numbers
    AvailableSeats INT CHECK (AvailableSeats >= 0), 
    LevelID INT,
    FOREIGN KEY (LevelID) REFERENCES ExamLevels(LevelID)
);

-- 5. Registrations Table linking users to sessions
CREATE TABLE Registrations (
    RegistrationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    SessionID INT,
    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (SessionID) REFERENCES ExamSessions(SessionID)
);
GO 
CREATE PROCEDURE RegisterForExam
    @p_UserID INT,
    @p_SessionID INT
AS
BEGIN
    -- Check if the classroom actually has seats left
    IF (SELECT AvailableSeats FROM ExamSessions WHERE SessionID = @p_SessionID) > 0
    BEGIN
        -- If yes then add them to the registration table
        INSERT INTO Registrations (UserID, SessionID)
        VALUES (@p_UserID, @p_SessionID);
        PRINT 'Registration Successful!';
    END
    ELSE
    BEGIN
        -- If no then stop the registration
        PRINT 'Error: Session is full.';
    END
END;
GO 
CREATE TRIGGER trg_UpdateSeats
ON Registrations
AFTER INSERT
AS
BEGIN
    -- The moment a student is inserted into Registrations, subtract 1 seat
    UPDATE ExamSessions
    SET AvailableSeats = AvailableSeats - 1
    FROM ExamSessions
    INNER JOIN inserted i ON ExamSessions.SessionID = i.SessionID;
    -- inserted is a temporary table holding the new data
END;

-- Create the actual SQL Server security roles
CREATE ROLE AdminRole;
CREATE ROLE StudentRole;

-- Admins can do anything to the exam schedule
GRANT ALL PRIVILEGES ON ExamSessions TO AdminRole;

-- Students can view the schedule, but cannot alter it
GRANT SELECT ON ExamSessions TO StudentRole;
DENY DELETE, UPDATE, INSERT ON ExamSessions TO StudentRole;

-- to see your report after data is inserted
SELECT  
    el.LevelName, 
    COUNT(r.RegistrationID) AS TotalRegistrations
FROM ExamLevels el
JOIN ExamSessions es ON el.LevelID = es.LevelID
JOIN Registrations r ON es.SessionID = r.SessionID
GROUP BY el.LevelName
HAVING COUNT(r.RegistrationID) > 0 -- Only show levels that have at least 1 student
ORDER BY TotalRegistrations DESC;