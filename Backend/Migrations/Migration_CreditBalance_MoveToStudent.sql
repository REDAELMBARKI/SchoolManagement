-- ============================================================================
-- Migration: Move CreditBalance from Enrollment to Student
-- Date: 2026-08-01
-- Story: Story 16 - Refactor CreditBalance to Student Entity
-- ============================================================================

BEGIN TRANSACTION;

-- ============================================================================
-- Step 1: Add CreditBalance column to Students table
-- ============================================================================
ALTER TABLE Students
ADD CreditBalance DECIMAL(18, 2) NOT NULL DEFAULT 0;

-- ============================================================================
-- Step 2: Migrate existing credit balances from Enrollments to Students
-- Sum all credit balances per student and update the student's credit balance
-- ============================================================================
UPDATE s
SET s.CreditBalance = ISNULL(e_totals.TotalCredit, 0)
FROM Students s
LEFT JOIN (
    SELECT StudentId, SUM(CreditBalance) AS TotalCredit
    FROM Enrollments
    GROUP BY StudentId
) e_totals ON s.Id = e_totals.StudentId;

-- ============================================================================
-- Step 3: Remove CreditBalance column from Enrollments table
-- ============================================================================
ALTER TABLE Enrollments
DROP COLUMN CreditBalance;

-- ============================================================================
-- Step 4: Verification queries (run these manually after migration)
-- ============================================================================
-- SELECT Id, FirstName, LastName, CreditBalance FROM Students WHERE CreditBalance > 0;
-- SELECT COUNT(*) FROM Students WHERE CreditBalance > 0;

COMMIT TRANSACTION;

-- ============================================================================
-- Rollback Script (Run only if migration needs to be reverted)
-- ============================================================================
/*
BEGIN TRANSACTION;

-- Add CreditBalance back to Enrollments
ALTER TABLE Enrollments
ADD CreditBalance DECIMAL(18, 2) NOT NULL DEFAULT 0;

-- Note: Original per-enrollment credit distribution cannot be restored
-- All credits will remain at the student level

-- Remove CreditBalance from Students
ALTER TABLE Students
DROP COLUMN CreditBalance;

COMMIT TRANSACTION;
*/
