﻿namespace CleanArchitechture.Application.Features.Lookups.Commands;

public class CreateLookupCommandValidator : AbstractValidator<CreateLookupCommand>
{
    private readonly ICommonQueryService _commonQuery;

    public CreateLookupCommandValidator(ICommonQueryService commonQuery)
    {
        _commonQuery = commonQuery;

        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(10)
            .MinimumLength(4)
              .WithMessage("'{PropertyName}' must be 4 - 10 characters.");

        RuleFor(v => v.Code)
            .MustAsync(BeUniqueCode)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueName)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");

        RuleFor(v => v.Description)
            .MaximumLength(500)
            .WithMessage("{0} can not exceed max 500 chars.");
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _commonQuery.IsExist("dbo.Lookups", ["Name"], new { Name = name });
    }
    public async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return !await _commonQuery.IsExist("dbo.Lookups", ["Code"], new { Code = code });
    }

}
