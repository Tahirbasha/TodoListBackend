using System.Security.Claims;
using TodoList.DataContext;
using TodoList.Interfaces;
using TodoList.Models;

namespace TodoList.NewFolder
{
    public class TodoService : ITodoService
    {
        private readonly TodoDataContext _todoContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TodoService(TodoDataContext todoContext, IHttpContextAccessor httpContextAccessor)
        {
            _todoContext = todoContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<TodoItem> GetTodos()
        {
            return _todoContext.TodoList.Where(todo => todo.UserId == GetUserId() && todo.Status != "Deleted").ToList();
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<List<TodoItem>> AddTodos(List<TodoItem> Todos)
        {

            DateTime dateTime = DateTime.Now;
            foreach (var item in Todos)
            {
                if (item.Id is null || item.Id == 0)
                {
                    TodoItem todoItem = new TodoItem()
                    {
                        Todo = item.Todo,
                        Status = item.Status,
                        CreatedAt = dateTime.ToString(),
                        UserId = GetUserId()
                    };
                    _todoContext.TodoList.Add(todoItem);
                    await _todoContext.SaveChangesAsync();
                } else if (item.Id is not null && item.Status == "Completed")
                {
                    TodoItem todoItem = _todoContext.TodoList.FirstOrDefault(t => t.Id == item.Id && t.UserId == GetUserId());
                    if (todoItem != null)
                    {
                        todoItem.Todo = item.Todo;
                        todoItem.Status = "Completed";
                        todoItem.CreatedAt = item.CreatedAt;
                    }
                    await _todoContext.SaveChangesAsync();
                }
                else if (item.Id is not null && (item.Status == "Updated" || item.Status == "Deleted"))
                {
                    TodoItem todoItem = _todoContext.TodoList.FirstOrDefault(t => t.Id == item.Id && t.UserId == GetUserId());
                    if (todoItem != null)
                    {
                        todoItem.Todo = item.Todo;
                        todoItem.Status = item.Status;
                        todoItem.CreatedAt = item.CreatedAt;
                    }
                    await _todoContext.SaveChangesAsync();
                }
            }
                return _todoContext.TodoList.Where(todo => todo.UserId == GetUserId() && todo.Status != "Deleted").ToList();
            }

      /*  public async Task<List<TodoItem>> UpdateTodo(List<UpdateTodo> Todos)
        {
            var existingTodos = _todoContext.TodoList;
            foreach (var updatedTodo in Todos)
            {
                TodoItem todoItem = _todoContext.TodoList.FirstOrDefault(t => t.Id == updatedTodo.Id && t.UserId == GetUserId());
                DateTime dateTime = DateTime.Now;
                if (todoItem != null)
                {
                    todoItem.Todo = updatedTodo.Todo;
                    todoItem.Status = "Updated";
                    todoItem.CreatedAt = todoItem.CreatedAt;
                    existingTodos.Update(todoItem);
                }
            }
            await _todoContext.SaveChangesAsync();
            return _todoContext.TodoList.Where(u => u.UserId == GetUserId()).ToList();

        }

        public async Task<List<TodoItem>> DeleteTodo(List<int> Ids)
        {
            var todoList = _todoContext.TodoList;
            var todos = new List<TodoItem>();
            foreach (var todoId in Ids)
            {
                var deletedTodo = todoList.FirstOrDefault(t => t.Id == todoId && t.UserId == GetUserId());
                if (deletedTodo != null)
                {
                    todos.Add(deletedTodo);
                }
            }
            todoList.RemoveRange(todos);
            await _todoContext.SaveChangesAsync();

            return _todoContext.TodoList.Where(u => u.UserId == GetUserId()).ToList();

        }*/
    }
}
