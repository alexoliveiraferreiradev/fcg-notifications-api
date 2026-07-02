using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using System.Net.Mail;

namespace Fcg.Notification.Domain.ValueObject
{
    public record EmailAddress
    {
        public string Value { get; }

        private EmailAddress(string value) => Value = value;

        public static EmailAddress Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new DomainException(MensagensDominio.EmailObrigatorio);

            AssertionConcern.AssertArgumentEmailFormat(value, MensagensDominio.EmailInvalido);
            return new EmailAddress(value.ToLowerInvariant());
        }
                
    }
}
