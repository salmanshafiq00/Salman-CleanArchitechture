namespace CleanArchitechture.Application.Common.Models;

public record FileResponse(string Path)
{

}

public record RemoveFileRequest(string RelativePath)
{

}
