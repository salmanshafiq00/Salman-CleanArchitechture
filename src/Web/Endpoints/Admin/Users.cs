using Azure.Core;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Common.Extensions;
using CleanArchitechture.Application.Common.Models;
using CleanArchitechture.Application.Features.Admin.AppUsers.Commands;
using CleanArchitechture.Application.Features.Admin.AppUsers.Queries;
using CleanArchitechture.Application.Features.Common.Queries;
using CleanArchitechture.Domain.Shared;

namespace CleanArchitechture.Web.Endpoints.Admin;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this);

        group.MapPost("GetAll", GetAll)
            .WithName("GetUsers")
            .Produces<PaginatedResponse<AppUserModel>>(StatusCodes.Status200OK);

        group.MapGet("Get/{id}", Get)
            .WithName("GetUser")
            .Produces<AppUserModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapGet("GetProfile", GetProfile)
            .WithName("GetProfile")
            .Produces<AppUserModel>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("Create", Create)
            .WithName("CreateUser")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("Upload", Upload)
            .WithName("Upload")
            .Produces<FileResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("RemoveFile", RemoveFile)
            .WithName("RemoveFile")
            .Produces<FileResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        group.MapPut("Update", Update)
            .WithName("UpdateUser")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("UpdateBasic", UpdateBasic)
            .WithName("UpdateBasic")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("AddToRoles", AddToRoles)
            .WithName("AddToRoles")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> GetAll(ISender sender, [FromBody] GetAppUserListQuery query)
    {
        var result = await sender.Send(query);

        if (!query.IsInitialLoaded)
        {
            var roleSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetRoleSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Role_All_SelectList,
                AllowCacheList: false)
            );
            result.Value.OptionsDataSources.Add("roleSelectList", roleSelectList.Value);
            result.Value.OptionsDataSources.Add("statusSelectList", UtilityExtensions.GetActiveInactiveSelectList());
        }

        return TypedResults.Ok(result.Value);
    }

    private async Task<IResult> Get(ISender sender, [FromRoute] string id)
    {
        var result = await sender.Send(new GetAppUserByIdQuery(id));

        var roleSelectList = await sender.Send(new GetSelectListQuery(
                Sql: SelectListSqls.GetRoleSelectListSql,
                Parameters: new { },
                Key: CacheKeys.Role_All_SelectList,
                AllowCacheList: false)
            );
        result.Value.OptionsDataSources.Add("roleSelectList", roleSelectList.Value);

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> GetProfile(ISender sender, IUser user)
    {
        var result = await sender.Send(new GetAppUserProfileQuery(user.Id));

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Create(ISender sender, [FromBody] CreateAppUserCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.Ok(result.Value),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Update(ISender sender, [FromBody] UpdateAppUserCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> UpdateBasic(ISender sender, [FromBody] UpdateAppUserBasicCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: result.ToProblemDetails);
    }

    private async Task<IResult> Upload(IHttpContextAccessor context)
    {
        var files =  context.HttpContext.Request.Form.Files;

        if (files is not null && files.Count == 0) 
        {
            return Results.BadRequest("Files not found");
        }

        var fileResponses = new List<FileResponse>();

        foreach (var file in files)
        {
            if (file is null || file.Length == 0)
            {
                return Results.BadRequest("File is empty");
            }

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "user-photos");

            Directory.CreateDirectory(folderPath);

            var fileNameWithExt = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(folderPath, fileNameWithExt);

            await using var stream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(stream);

            var relativePath = Path.Combine($"{Path.DirectorySeparatorChar}Resources", "uploads", "user-photos", fileNameWithExt).Replace(@"\", "/");

            fileResponses.Add(new FileResponse(relativePath));
        }

        return Results.Ok(fileResponses);

    }

    public async Task<IResult> RemoveFile([FromBody] RemoveFileRequest removeFileReq)
    {
        // Convert the relative path to an absolute path
        string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), removeFileReq.RelativePath.TrimStart(Path.DirectorySeparatorChar, '/'));

        if (File.Exists(absolutePath))
        {
            try
            {
                File.Delete(absolutePath);
                await Task.CompletedTask;
                return Results.Ok("File removed successfully.");
            }
            catch (Exception )
            {
                // Handle exceptions like access issues, etc.
                return Results.Problem(
                   statusCode: StatusCodes.Status500InternalServerError,
                   title: "Internal server error");

            }
        }
        else
        {
            return Results.NotFound("File not found.");
        }
    }


    private async Task<IResult> AddToRoles(ISender sender, [FromBody] AddToRolesCommand command)
    {
        var result = await sender.Send(command);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: result.ToProblemDetails);
    }
}
