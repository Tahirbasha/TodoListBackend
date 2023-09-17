using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoList.DataContext;
using TodoList.Dto;
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
            return _todoContext.TodoList.Where(todo => todo.UserId == GetUserId()).ToList();
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<List<TodoItem>> AddTodos(AddTodoDto Todos)
        {

            DateTime dateTime = DateTime.Now;
            List<TodoItem> TodoList = new List<TodoItem>();
            foreach (var item in Todos.Todos)
            {
                TodoItem todoItem = new TodoItem()
                {
                    Todo = item,
                    Status = "Created",
                    CreatedAt = dateTime.ToString(),
                    UserId = GetUserId()
                };
                TodoList.Add(todoItem);
            }
            await _todoContext.TodoList.AddRangeAsync(TodoList);
            await _todoContext.SaveChangesAsync();
            return _todoContext.TodoList.Where(u => u.UserId == GetUserId()).ToList();

        }

        public async Task<List<TodoItem>> UpdateTodo(List<UpdateTodo> Todos)
        {
            var existingTodos = _todoContext.TodoList;
            foreach(var updatedTodo in Todos)
            {
                TodoItem todoItem = _todoContext.TodoList.FirstOrDefault(t => t.Id == updatedTodo.Id && t.UserId == GetUserId());
                DateTime dateTime = DateTime.Now;
                if (todoItem != null)
                {
                    todoItem.Todo = updatedTodo.Todo;
                    todoItem.Status = "Updated";
                    todoItem.CreatedAt = dateTime.ToString();
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

        }
    }
}
