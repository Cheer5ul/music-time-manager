using music_time_manager.Core.Errors;
using music_time_manager.Core.Result;

namespace music_time_manager.Core.Models;

public class SubTask
{
    private SubTask(Guid id, string title, Status status, Guid taskId)
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

    public static ResultT<SubTask> Create(string title, Status status, Guid taskId)
    {
        List<Error> errors = [];
        
        if (string.IsNullOrWhiteSpace(title) || title.Length > Task.MAX_TITLE_LENGTH ||
            title.Length < Task.MIN_TITLE_LENGTH)
        {
            errors.Add(TaskErrors.InvalidTitle(title));
        }

        if (errors.Count > 0)
        {
            return ResultT<SubTask>.Failures(errors);
        }

        var subTask = new SubTask(
            Guid.NewGuid(),
            title, 
            Models.Status.ToDo,
            taskId);
        
        return ResultT<SubTask>.Success(subTask);
    }

    public static SubTask Reconstitute(Guid id, string title, Status status, Guid taskId)
    {
        return new SubTask(id, title, status, taskId);
    }
}