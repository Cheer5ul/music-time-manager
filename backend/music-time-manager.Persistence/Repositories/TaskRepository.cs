// using music_time_manager.Core.Models;
// using music_time_manager.Persistence.Entities;
// using Task = music_time_manager.Core.Models.Task;
//
// namespace music_time_manager.Persistence.Repositories;
//
// public class TaskRepository
// {
//     private readonly MusicTimeManagerDbContext _dbContext;
//     
//     public TaskRepository(MusicTimeManagerDbContext dbContext)
//     {
//         _dbContext = dbContext;
//     }
//
//     public async Task<Guid> CreateTask(Task task)
//     {
//         var taskAssigneeEntity = new SubtaskAssigneeEntity()
//         {
//             
//         };
//
//         var subtaskAssigneeEntity = new SubtaskAssigneeEntity()
//         {
//             
//         };
//         
//         var subtaskEntities = task.SubTasks
//             .Select(st => new SubtaskEntity()
//             {
//                 Id = st.Id,
//                 Title = st.Title,
//                 Status = st.Status,
//                 TaskId = st.TaskId
//             })
//             .ToList();
//
//         var taskEntity = new TaskEntity()
//         {
//             Id = task.Id,
//             Title = task.Title,
//             Description = task.Description,
//             DueDate = task.DueDate,
//             CreatedAt = task.CreatedAt,
//             Status = task.Status,
//             CreatedBy = task.CreatedBy,
//             RecreatedFromTask = null,
//             RecreatedFromTaskId = null,
//             RecreatedTasks = [],
//             SubtaskEntities = subtaskEntities,
//             TaskAssignees = []
//         };
//         
//         var taskAssigneeEntities = new List<TaskAssigneeEntity>();
//
//         foreach (var subtaskEntity in subtaskEntities)
//         {
//             taskAssigneeEntities.Add(new TaskAssigneeEntity()
//             {
//                 TaskId = taskEntity.Id,
//                 UserId = taskEntity.
//             });
//         }
//         
//
//     }
// }