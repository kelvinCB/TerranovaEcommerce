using FluentValidation;

namespace Application.Users.Queries.GetUsers;

/// <summary>
/// Represents a validator for the <see cref="GetUsersQuery"/> class.
/// </summary>
/// <remarks>FluentValidation is used to validate the query parameters.</remarks>
public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUsersQueryValidator"/> class.
    /// </summary>
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);

        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);

        RuleFor(x => x.Search).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Search));
    }
}