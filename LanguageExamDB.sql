
-- LANGUAGE EXAM APP : the idea is of this database is to create a secure, automated Language Exam Registration System. The core idea is to replace manual paper-tracking that manages user authentication and categorizes exam levels
USE LanguageExamDB;
GO

--Roles Table to define if user is instructor or student
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

-- Users Table handles logging in
CREATE TABLE Users (
    -- IDENTITY(1,1) means the database starts at 1 and counts up automatically to make it easier for the c# code to run smoother
    UserID INT IDENTITY(1,1) PRIMARY KEY, 
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL, 
    RoleID INT,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- ExamLevels Table to categorize the exam levels
CREATE TABLE ExamLevels (
    LevelID INT IDENTITY(1,1) PRIMARY KEY, 
    LevelName VARCHAR(50) NOT NULL
);

-- ExamSessions Table for the actual test and the date
CREATE TABLE ExamSessions (
    SessionID INT IDENTITY(1,1) PRIMARY KEY,
    SessionName VARCHAR(100) NOT NULL,
    ExamDate DATETIME NOT NULL,
    TotalSeats INT CHECK (TotalSeats >= 0), -- Prevents negative numbers
    AvailableSeats INT CHECK (AvailableSeats >= 0), 
    LevelID INT,
    FOREIGN KEY (LevelID) REFERENCES ExamLevels(LevelID)
);

-- Registrations Table linking users to sessions
CREATE TABLE Registrations (
    RegistrationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    SessionID INT,
    RegistrationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (SessionID) REFERENCES ExamSessions(SessionID)
);
GO 

--  PROCEDURES and TRIGGERS

-- procedure to admit students to a session
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
    END
    ELSE
    BEGIN
        -- If no, throw an error so the C# app can catch it
        THROW 50000, 'Error: Session is full.', 1;
    END
END;
GO 
-- trigger for when a student is submitted to a session
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
    -- 'inserted' is a temporary table holding the new data
END;
GO 

--  SECURITY ROLES

-- Create sql server security roles
CREATE ROLE AdminRole;
CREATE ROLE StudentRole;

-- Admins can do anything to the exam schedule
GRANT ALL PRIVILEGES ON ExamSessions TO AdminRole;

-- Students can view the schedule but cannot change it
GRANT SELECT ON ExamSessions TO StudentRole;
DENY DELETE, UPDATE, INSERT ON ExamSessions TO StudentRole;
GO

-- Data for testing

-- Insert Roles
INSERT INTO Roles (RoleID, RoleName) VALUES (1, 'Student'), (2, 'Instructor');

-- Insert User no.1 (mock user)
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserID, FullName, Email, PasswordHash, RoleID) 
VALUES (1, 'Test Student', 'student@test.com', '1234', 1); 
SET IDENTITY_INSERT Users OFF;

-- Insert Exam Levels
SET IDENTITY_INSERT ExamLevels ON;
INSERT INTO ExamLevels (LevelID, LevelName) VALUES 
(1, 'B1 Intermediate'), 
(2, 'B2 Upper-Intermediate'), 
(3, 'C1 Advanced');
SET IDENTITY_INSERT ExamLevels OFF;

-- Insert Exam Sessions
SET IDENTITY_INSERT ExamSessions ON;
INSERT INTO ExamSessions (SessionID, SessionName, ExamDate, TotalSeats, AvailableSeats, LevelID)
VALUES 
(101, 'Saturday Morning Session', '2026-06-06 09:00:00', 15, 15, 1),
(102, 'Saturday Afternoon Session', '2026-06-06 13:00:00', 3, 3, 2),
(103, 'Sunday Morning Session', '2026-06-07 10:00:00', 0, 0, 3);
SET IDENTITY_INSERT ExamSessions OFF;
GO

-- reporting query

-- Query to see the report after data is inserted
SELECT  
    el.LevelName, -- Displays the name of the exam level
    COUNT(r.RegistrationID) AS TotalRegistrations
FROM ExamLevels el
-- 'join' is used to put back the normalized 3NF tables back together
JOIN ExamSessions es ON el.LevelID = es.LevelID -- Links the level category to the actual scheduled physical classes
JOIN Registrations r ON es.SessionID = r.SessionID -- Links the scheduled classes to the students who actually booked them
GROUP BY el.LevelName
HAVING COUNT(r.RegistrationID) > 0 -- Only show levels that have at least 1 student
--sorting the data logically
ORDER BY TotalRegistrations DESC; 