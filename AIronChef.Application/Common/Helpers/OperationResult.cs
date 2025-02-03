using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Common.Helpers
{
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }

        public OperationResult()
        {
            Success = false;
            Data = default;
            ErrorMessage = null;
        }
        
        public static OperationResult<T> Successfull(T data)
        {
            return new OperationResult<T> { Success = true, Data = data };
        }

        public static OperationResult<T> Failure(string errorMessage)
        {
            return new OperationResult<T> { Success = false, ErrorMessage = errorMessage };
        }
    }
}
