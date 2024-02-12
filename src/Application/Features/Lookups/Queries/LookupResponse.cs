using CleanArchitechture.Domain.Entities;

namespace CleanArchitechture.Application.Features.Lookups.Queries;

public record LookupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Lookup, LookupResponse>().ReverseMap();
        }
    }
}
