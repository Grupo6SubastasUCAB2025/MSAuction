using FluentValidation;
using MSAuction.Commons.DTOs;

namespace MSAuction.Application.Validators;

public class AuctionDtoValidator : AbstractValidator<AuctionDto>
{
    public AuctionDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.InitialPrice).GreaterThan(0);
        RuleFor(x => x.MinIncrement).GreaterThan(0);
        RuleFor(x => x.StartTime).LessThan(x => x.EndTime);
    }
}