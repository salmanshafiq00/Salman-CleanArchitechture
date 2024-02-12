using CleanArchitechture.Domain.Entities;

namespace CleanArchitechture.Application.Features.LookupDetails.Queries;

public record LookupDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public Guid LookupId { get; set; }
    public string LookupName { get; set; } = string.Empty;

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<LookupDetail, LookupDetailResponse>().ReverseMap();
        }
    }
}
