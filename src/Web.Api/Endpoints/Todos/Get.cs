using Application.Abstractions.Messaging;
using Application.Todos.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos", (Guid userId) =>
        {
            var mockTodos = new List<TodoResponse>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Description = "Complete project documentation",
                    DueDate = DateTime.Now.AddDays(7),
                    Labels = ["Documentation", "Important"],
                    IsCompleted = false,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    CompletedAt = null
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Description = "Review pull requests",
                    DueDate = DateTime.Now.AddDays(3),
                    Labels = ["Code Review"],
                    IsCompleted = false,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    CompletedAt = null
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Description = "Update dependencies",
                    DueDate = DateTime.Now.AddDays(-1),
                    Labels = ["Maintenance", "Urgent"],
                    IsCompleted = true,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    CompletedAt = DateTime.Now.AddHours(-2)
                }
            };

            var result = Result<List<TodoResponse>>.Success(mockTodos);
            //var query = new GetTodosQuery(userId);

            //Result<List<TodoResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos);
        //.RequireAuthorization();

        //        app.MapGet("todos", async (
        //    Guid userId,
        //    IQueryHandler<GetTodosQuery, List<TodoResponse>> handler,
        //    CancellationToken cancellationToken) =>
        //        {
        //            var query = new GetTodosQuery(userId);

        //            Result<List<TodoResponse>> result = await handler.Handle(query, cancellationToken);

        //            return result.Match(Results.Ok, CustomResults.Problem);
        //        })
        //.WithTags(Tags.Todos)
        //.RequireAuthorization();

    }
}
