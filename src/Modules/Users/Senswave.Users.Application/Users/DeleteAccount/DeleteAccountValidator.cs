using Senswave.Abstractions.Entities;

namespace Senswave.Users.Application.Users.DeleteAccount;

public class DeleteAccountValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(AllowedLengths.Descriptions.MaxLength);
    }
}
