namespace CleanArchitechture.Application.Common.Models;

public record FileResponse(string FilePath)
{

}

public record RemoveFileRequest(string RelativePath)
{

}
