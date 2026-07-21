

using SchoolManagement.Domain.Enums;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Domain.Entities;
public class Intake : Person
{
    public Email? Email { get; private set; } = null!;
    public string? Phone {get;private set;} = string.Empty;
    public DateOnly? DateOfBirth { get; private set; }

    public DateTime IntakeDate { get; private set; }
    public IntakeStatus Status {get;private set;} 
    public DateTime? FollowUpDate { get; private set; } 
    public string? Notes { get; private set; }   
    //fk
    public Guid? CommercialAgentId { get; private set; } 
    public Guid? LeadSourceId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid BranchId {get;private set;}
    public bool IsIndependent { get; private set; } = false;
    public decimal TotalFees { get; private set; }
    public decimal AmountPaid { get; private set; }
    // navigation
    public virtual LeadSource? LeadSource { get; private set; }
    public virtual CommercialAgent? CommercialAgent { get; private set; }
    public virtual Subject Subject {get;private set;} = null!;
    public virtual Branch Branch {get;private set;} = null!;
    public virtual Student? ConvertedToStudent { get; private set; }


    public static Intake Register(string firstName, string lastName, string slug  ,  Guid? genderId, string? email, string? phone, DateOnly? dateOfBirth, DateTime intakeDate, IntakeStatus status, DateTime? followUpDate, string? notes, Guid? commercialAgentId, Guid? leadSourceId, Guid subjectId, Guid branchId, bool isIndependent, decimal totalFees, decimal amountPaid)
    {
        if (subjectId == Guid.Empty)
        {
            throw new DomainException("Subject ID must not be empty.");
        }
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        if (totalFees < 0)
        {
            throw new DomainException("Total fees cannot be negative.");
        }
        if (amountPaid < 0)
        {
            throw new DomainException("Amount paid cannot be negative.");
        }

        var intake = new Intake
        {
            Email = email != null ? new Email(email) : null,
            Phone = phone,
            DateOfBirth = dateOfBirth,
            IntakeDate = intakeDate,
            Status = status,
            FollowUpDate = followUpDate,
            Notes = notes,
            CommercialAgentId = commercialAgentId,
            LeadSourceId = leadSourceId,
            SubjectId = subjectId,
            BranchId = branchId,
            IsIndependent = isIndependent,
            TotalFees = totalFees,
            AmountPaid = amountPaid
        };
        
        intake.RegisterPerson(firstName, lastName, slug  , genderId);
        return intake;
    }

    public void UpdateEmail(string? email)
    {
        Email = new Email(email);
    }

    public void UpdatePhone(string? phone)
    {
        Phone = phone;
    }

    public void UpdateDateOfBirth(DateOnly? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public void UpdateIntakeDate(DateTime intakeDate)
    {
        IntakeDate = intakeDate;
    }

    public void UpdateStatus(IntakeStatus status)
    {
        Status = status;
    }

    public void MarkAsNew()
    {
        Status = IntakeStatus.New;
    }

    public void MarkAsContacted()
    {
        Status = IntakeStatus.Contacted;
    }

    public void MarkAsInterested()
    {
        Status = IntakeStatus.Interested;
    }

    public void MarkAsEnrolled()
    {
        Status = IntakeStatus.Enrolled;
    }

    public void MarkAsNotInterested()
    {
        Status = IntakeStatus.NotInterested;
    }

    public void UpdateFollowUpDate(DateTime? followUpDate)
    {
        FollowUpDate = followUpDate;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    public void UpdateCommercialAgentId(Guid? commercialAgentId)
    {
        CommercialAgentId = commercialAgentId;
    }

    public void UpdateLeadSourceId(Guid? leadSourceId)
    {
        LeadSourceId = leadSourceId;
    }

    public void UpdateSubjectId(Guid subjectId)
    {
        if (subjectId == Guid.Empty)
        {
            throw new DomainException("Subject ID must not be empty.");
        }
        SubjectId = subjectId;
    }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
        {
            throw new DomainException("Branch ID must not be empty.");
        }
        BranchId = branchId;
    }

    public void UpdateIsIndependent(bool isIndependent)
    {
        IsIndependent = isIndependent;
    }

    public void UpdateTotalFees(decimal totalFees)
    {
        if (totalFees < 0)
        {
            throw new DomainException("Total fees cannot be negative.");
        }
        TotalFees = totalFees;
    }

    public void UpdateAmountPaid(decimal amountPaid)
    {
        if (amountPaid < 0)
        {
            throw new DomainException("Amount paid cannot be negative.");
        }
        AmountPaid = amountPaid;
    }
    


}


