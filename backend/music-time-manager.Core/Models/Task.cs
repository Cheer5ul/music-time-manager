using music_time_manager.Core.Errors;
using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class Task
{
    public const int MAX_TITLE_LENGTH = 100; 
    public const int MIN_TITLE_LENGTH = 2;
    public const int MAX_DESCRIPTION_LENGTH = 1000;
    private Task(
        Guid id, string title, DateTime createdAt, Status status,
        Guid createdBy, string? description, Guid? recreatedFromTaskId)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        Status = status;
        CreatedBy = createdBy;
        RecreatedFromTaskId = recreatedFromTaskId;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Status Status { get; private set; }
    public Guid CreatedBy { get; private set; }
    public Guid? RecreatedFromTaskId { get; private set; }

    public ResultT<Task> Create(string title, 
        Guid createdBy, string? description, Guid? recreatedFromTaskId)
    {
        List<Error> errors = [];
        
        if (string.IsNullOrWhiteSpace(title) || title.Length > MAX_TITLE_LENGTH
            || title.Length < MIN_TITLE_LENGTH)
        {
            errors.Add(TaskErrors.InvalidTitle(title));
        }

        if (description != null && string.IsNullOrWhiteSpace(description) ||
            description != null && description.Length > MAX_DESCRIPTION_LENGTH)
        {
            errors.Add(TaskErrors.InvalidDescription(description));
        }

        if (errors.Count > 0)
        {
            return ResultT<Task>.Failures(errors);
        }

        var task = new Task(
            id: Guid.NewGuid(),
            title: title,
            createdAt: DateTime.Now,
            status: Status.ToDo,
            createdBy: createdBy,
            description: description,
            recreatedFromTaskId: recreatedFromTaskId);
        
        return ResultT<Task>.Success(task);
    }

    public static Task Reconstitute(Guid id, string title, DateTime createdAt, Status status,
        Guid createdBy, string? description, Guid? recreatedFromTaskId)
    {
        return new Task(
            id, title, createdAt, status, createdBy, description, recreatedFromTaskId);
    }
}