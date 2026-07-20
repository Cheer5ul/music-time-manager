using music_time_manager.Core.Errors;
using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class Subtask
{
    private Subtask(Guid id, string title, Status status, Guid taskId)
    {
        Id = id;
        Title = title;
        Status = status;
        TaskId = taskId;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public Status Status { get; private set; }
    public Guid TaskId { get; private set; }

    public static ResultT<Subtask> Create(string title, Guid taskId)
    {
        List<Error> errors = [];
        
        if (string.IsNullOrWhiteSpace(title) || title.Length > Task.MAX_TITLE_LENGTH ||
            title.Length < Task.MIN_TITLE_LENGTH)
        {
            errors.Add(SubtaskErrors.InvalidTitle(title));
        }

        if (errors.Count > 0)
        {
            return ResultT<Subtask>.Failures(errors);
        }

        var subtask = new Subtask(
            Guid.NewGuid(),
            title, 
            Models.Status.ToDo,
            taskId);
        
        return ResultT<Subtask>.Success(subtask);
    }

    public static Subtask Reconstitute(Guid id, string title, Status status, Guid taskId)
    {
        return new Subtask(id, title, status, taskId);
    }

    public Result.Result CreateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > Task.MAX_TITLE_LENGTH ||
            title.Length < Task.MIN_TITLE_LENGTH)
        {
            return Result.Result.Failures([SubtaskErrors.InvalidTitle(title)]);
        }
        
        Title =  title;
        
        return Result.Result.Success;
    }
}