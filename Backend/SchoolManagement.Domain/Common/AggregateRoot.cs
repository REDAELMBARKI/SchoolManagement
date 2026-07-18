using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Common
{
    public class AggregateRoot : BaseEntity
    {
        private readonly IList<INotification>  _domainEvents = new List<INotification>();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();
        protected void AddDomainEvent(INotification DomainEvent)
        {
            _domainEvents.Add(DomainEvent);
        }

        protected void RemoveDomainEvent(INotification DomainEvent)
        {
            _domainEvents.Remove(DomainEvent);
        }

        public  void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
