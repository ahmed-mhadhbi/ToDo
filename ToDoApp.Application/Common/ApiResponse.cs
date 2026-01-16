using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ToDoApp.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }       // Indicates if the operation succeeded
        public string? Message { get; set; }    // Optional message (error or info)
        public T? Data { get; set; }            // Actual payload (Todo, list, etc.)

        public ApiResponse(T? data, bool success = true, string? message = null)
        {
            Data = data;
            Success = success;
            Message = message;
        }

        // Convenience methods
        public static ApiResponse<T> Ok(T? data, string? message = null) => new(data, true, message);
        public static ApiResponse<T> Fail(string message) => new(default, false, message);
    }
}
