using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Notification.Domain.ValueObject;
using FluentAssertions;
using System;
using Xunit;

namespace Fcg.Notification.Domain.Tests.ValueObject
{
    public class EmailAddressTests
    {
        [Fact]
        public void Create_DeveRetornarInstanciaValida_QuandoEmailForValido()
        {
            // Arrange
            var emailValido = "usuario@teste.com";

            // Act
            var emailAddress = EmailAddress.Create(emailValido);

            // Assert
            emailAddress.Value.Should().Be(emailValido);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Create_DeveLancarDomainException_QuandoEmailForVazioOuNulo(string emailInvalido)
        {
            // Arrange & Act
            Action act = () => EmailAddress.Create(emailInvalido);

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Theory]
        [InlineData("emailinvalido.com")]
        [InlineData("usuario@")]
        [InlineData("@dominio.com")]
        [InlineData("usuario@dominio")]
        public void Create_DeveLancarDomainException_QuandoEmailEstiverEmFormatoInvalido(string emailInvalido)
        {
            // Arrange & Act
            Action act = () => EmailAddress.Create(emailInvalido);

            // Assert
            act.Should().Throw<DomainException>();
        }
    }
}
