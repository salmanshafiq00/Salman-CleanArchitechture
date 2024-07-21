using CleanArchitechture.Domain.Admin;
using static CleanArchitechture.Application.Common.DapperQueries.Constants;

namespace CleanArchitechture.Application.Features.Admin.AppPages.Queries;

public record AppPageModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string RouterLink { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Active { get; set; } = string.Empty;
    public string AppPageLayout { get; set; } = string.Empty;

    public List<AppPageFieldModel> AppPageFields { get; set; } = [];

    public Dictionary<string, object> OptionsDataSources { get; set; } = [];

    public static IReadOnlyCollection<DataFieldModel> DataFields = [
        new DataFieldModel{ FieldName = "id", Caption = "Id", FieldType = TField.TString, DSName = string.Empty, DbField = "ap.Id", IsSortable = false, IsGlobalFilterable = false, IsFilterable = false, IsVisible = false, SortOrder = 0 },
        new DataFieldModel{ FieldName = "name", Caption = "Name", FieldType = TField.TString, DSName = string.Empty, DbField = "ap.Name", IsSortable = true, IsGlobalFilterable = true, IsFilterable = true, IsVisible = true,  SortOrder = 1 },
        new DataFieldModel{ FieldName = "title", Caption = "Title", FieldType = TField.TString, DSName = string.Empty, DbField = "ap.Title", IsSortable = false, IsGlobalFilterable = false,  IsFilterable = false, IsVisible = true, SortOrder = 2 }
    ];


    private class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<AppPage, AppPageModel>().ReverseMap();
        }
    }
}

public record AppPageLayout
{
    public List<AppPageFieldModel> AppPageFields { get; set; } = [];
}

public record AppPageFieldModel 
{
    public Guid Id { get; set; }
    public Guid AppPageId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string DbField { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string TextAlign { get; set; } = string.Empty;
    public bool IsSortable { get; set; } = true;
    public bool IsFilterable { get; set; } = false;
    public string DSName { get; set; } = string.Empty;
    public bool IsGlobalFilterable { get; set; } = false;
    public string FilterType { get; set; } = string.Empty;
    public bool EnableLink { get; set; } = false;
    public string LinkBaseUrl { get; set; } = string.Empty;
    public string LinkValueFieldName { get; set; } = string.Empty;
    public string BgColor { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    private class Mapper: Profile 
    {
        public Mapper()
        {
            CreateMap<AppPageFieldModel, AppPageField>().ReverseMap();
        }
    }
}

public record AppPageActionModel 
{
    public Guid Id { get; set; }
    public Guid AppPageId { get; set; }
    public int? ActionTypeId { get; set; }
    public string ActionName { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string FunctionName { get; set; } = string.Empty;   
    public string NavigationUrl { get; set; } = string.Empty;
    public bool ShowCaptionDevice { get; set; }
    public bool ShowCaptionWeb { get; set; }
    public string Position { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    private class Mapper : Profile 
    {
        public Mapper()
        {
            CreateMap<AppPageAction, AppPageActionModel>().ReverseMap();    
        }
    }
}
