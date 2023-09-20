using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Dto;
using TodoList.Interfaces;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;   
        }
        [HttpGet("GetTodos")]
        public ActionResult<List<TodoItem>> GetTodos()
        {
            return Ok(_todoService.GetTodos());
        }
        [HttpPost("SaveTodos")]
        public async Task<ActionResult<List<TodoItem>>> AddTodo([FromBody] List<TodoItem> TodoItems)
        {
            return Ok(await _todoService.AddTodos(TodoItems));
        }
        /*[HttpPost("UpdateTodo")]
        public async Task<ActionResult<List<TodoItem>>> UpdateTodo(List<UpdateTodo> updateTodos)
        {
            return Ok(await _todoService.UpdateTodo(updateTodos));
        }
        [HttpPost("DeleteTodo")]
        public async Task<ActionResult<List<TodoItem>>> DeleteTodo(List<int> Ids)
        {
            return Ok(await _todoService.DeleteTodo(Ids));
        }*/

    }
}
