using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using System.Net.Mail;

namespace Fcg.Notification.Domain.ValueObject
{
    public record EmailAddress
    {
        public string Address { get; }

        private EmailAddress(string value) => Address = value;

        public static EmailAddress Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new DomainException(DomainMessages.EmailRequired);

            AssertionConcern.AssertArgumentEmailFormat(value, DomainMessages.EmailInvalid);
            return new EmailAddress(value.ToLowerInvariant());
        }
                
    }
}
