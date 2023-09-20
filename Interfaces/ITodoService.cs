using TodoList.Dto;
using TodoList.Models;

namespace TodoList.Interfaces
{
    public interface ITodoService
    {
        List<TodoItem> GetTodos();

        Task<List<TodoItem>> AddTodos(List<TodoItem> TodoItems);

       /* Task<List<TodoItem>> UpdateTodo(List<UpdateTodo> Todo);

        Task<List<TodoItem>> DeleteTodo(List<int> Id);*/
    }
}
