using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Exceptions;
using SchoolManagement.Domain.Interfaces;

namespace SchoolManagement.Domain.Entities;

public class LeadSource : AggregateRoot
{
    public Guid BranchId { get; protected set; }
    public virtual Branch Branch { get; private set; } = null!;
    public virtual ICollection<Intake> Intakes { get; private set; } = new List<Intake>();

    protected LeadSource() { }

    public void UpdateBranchId(Guid branchId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        BranchId = branchId;
    }
}

public class OpcLeadSource : LeadSource
{
    public Guid OpcId { get; private set; }
    public virtual Opc Opc { get; private set; } = null!;

    private OpcLeadSource() { }

    public static OpcLeadSource Create(Guid branchId, Guid opcId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (opcId == Guid.Empty)
            throw new DomainException("Opc ID must not be empty.");

        return new OpcLeadSource
        {
            BranchId = branchId,
            OpcId = opcId
        };
    }

    public void UpdateOpcId(Guid opcId)
    {
        if (opcId == Guid.Empty)
            throw new DomainException("Opc ID must not be empty.");
        OpcId = opcId;
    }
}

public class AdLeadSource : LeadSource
{
    public Guid AdId { get; private set; }
    public virtual Ad Ad { get; private set; } = null!;

    private AdLeadSource() { }

    public static AdLeadSource Create(Guid branchId, Guid adId)
    {
        if (branchId == Guid.Empty)
            throw new DomainException("Branch ID must not be empty.");
        if (adId == Guid.Empty)
            throw new DomainException("Ad ID must not be empty.");

        return new AdLeadSource
        {
            BranchId = branchId,
            AdId = adId
        };
    }

    public void UpdateAdId(Guid adId)
    {
        if (adId == Guid.Empty)
            throw new DomainException("Ad ID must not be empty.");
        AdId = adId;
    }
}




