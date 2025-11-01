using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RequestService.Application.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; }

        public static ServiceResult<T?> Success(T? data, string message) => new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };

        public static ServiceResult<T?> Failure(string errors) => new ServiceResult<T> { IsSuccess = false, Message = errors };
    }

}
