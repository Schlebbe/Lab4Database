--Skolan vill kunna ta fram en översikt över all personal där det framgår namn och vilka befattningar de har samt hur många år de har arbetat på skolan. Administratören vill också ha möjlighet att spara ner ny personal. (SQL i SSMS)
SELECT 
	StaffID, 
	FirstName, 
	LastName, 
	Role, 
	DATEDIFF(YEAR, HireDate, GETDATE()) AS YearsWorked
FROM Staff

INSERT INTO Staff (FirstName, LastName, Role, HireDate) VALUES
('Minerva', 'McGonagall', 'Teacher', CAST(GETDATE() AS DATE))

--Vi vill spara ner studenter och se vilken klass de går i. Vi vill kunna spara ner betyg för en student i varje kurs de läst och vi vill kunna se vilken lärare som satt betyget. Betyg ska också ha ett datum då de satts. (SQL i SSMS)
INSERT INTO Student (FirstName, LastName, PersonalNumber, ClassID) VALUES
('Harry', 'Potter', '1980-07-31', 1)

SELECT 
	s.FirstName,
	s.LastName,
	s.PersonalNumber,
	c.ClassName
FROM Student s
INNER JOIN Class c ON s.ClassID = c.ClassID

INSERT INTO Grade (GradeValue, DateGiven, StudentID, CourseID, TeacherID) VALUES
('A', CAST(GETDATE() AS DATE), '3', 8, 5)

SELECT 
	GradeValue,
	DateGiven,
	(s.FirstName + ' ' + s.LastName) as Teacher
FROM Grade g
INNER JOIN Staff s ON s.StaffID = TeacherID

--Hur mycket betalar respektive avdelning ut i lön varje månad? (SQL i SSMS)
SELECT 
	Role,
	SUM(s.Salary) as MonthlySalary
FROM Staff s
GROUP BY s.Role

--Hur mycket är medellönen för de olika avdelningarna? (SQL i SSMS)
SELECT
	Role,
	AVG(s.Salary) as AverageSalary
FROM Staff s
GROUP BY s.Role

--Skapa en Stored Procedure som tar emot ett Id och returnerar viktig information om den student som är registrerad med aktuellt id. (SQL i SSMS)
CREATE PROCEDURE GetStudentById
    @StudentID INT
AS
SELECT
    StudentID,
    FirstName,
    LastName,
    PersonalNumber,
    ClassID
FROM Student
WHERE StudentID = @StudentID
GO