﻿namespace CleanArchitechture.Application.Features.LookupDetails.Commands;

public class CreateLookupDetailCommandValidator : AbstractValidator<CreateLookupDetailCommand>
{
    private readonly ICommonQueryService _commonQuery;

    public CreateLookupDetailCommandValidator(ICommonQueryService commonQuery)
    {
        _commonQuery = commonQuery;

        RuleFor(v => v.Code)
            .NotEmpty()
            .MaximumLength(10)
            .MinimumLength(3)
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
        return !await _commonQuery.IsExist("dbo.LookupDetails", ["Name"], new { Name = name });
    }
    public async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return !await _commonQuery.IsExist("dbo.LookupDetails", ["Code"], new { Code = code });
    }

}
