namespace CleanArchitechture.Application.Common.Models;

public class SelectListModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class SelectListModel<TId> : SelectListModel
{
    public new TId Id { get; set; }
}

public class SelectListModelDynamic : SelectListModel
{
    public dynamic ValueOne { get; set; }
    public dynamic ValueTwo { get; set; }
    public dynamic ValueThree { get; set; }
    public dynamic ValueFour { get; set; }
    public dynamic ValueFive { get; set; }
    public dynamic ValueSix { get; set; }
    public dynamic ValueSeven { get; set; }

}
