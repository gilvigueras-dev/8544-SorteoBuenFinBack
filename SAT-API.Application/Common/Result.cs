using SAT_API.Domain.Common;

namespace SAT_API.Application.Common
{
    /// <summary>
    /// Patrón Result para manejar operaciones que pueden fallar
    /// </summary>
    /// <typeparam name="T">Tipo de dato del resultado</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; private set; }
        public List<ErrorInfo> Errors { get; private set; } = new();
        public string Message { get; private set; }

        private Result(bool isSuccess, T? value, string message, List<ErrorInfo>? errors = null)
        {
            this.IsSuccess = isSuccess;
            this.Value = value;
            this.Message = message;
            this.Errors = errors ?? new List<ErrorInfo>();
        }

        public static Result<T> Success(T value, string message = "Operación exitosa")
        {
            return new Result<T>(true, value, message);
        }

        public static Result<T> Failure(string message, List<ErrorInfo>? errors = null)
        {
            return new Result<T>(false, default, message, errors);
        }

        public static Result<T> Failure(string message, string errorCode, string errorMessage)
        {
            var errors = new List<ErrorInfo>
        {
            new ErrorInfo { Code = errorCode, Message = errorMessage }
        };
            return new Result<T>(false, default, message, errors);
        }
    }

    // Result sin valor de retorno
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public List<ErrorInfo> Errors { get; private set; } = new();
        public string Message { get; private set; }

        private Result(bool isSuccess, string message, List<ErrorInfo>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors ?? new List<ErrorInfo>();
        }

        public static Result Success(string message = "Operación exitosa")
        {
            return new Result(true, message);
        }

        public static Result Failure(string message, List<ErrorInfo>? errors = null)
        {
            return new Result(false, message, errors);
        }

        public static Result Failure(string message, string errorCode, string errorMessage)
        {
            var errors = new List<ErrorInfo>
        {
            new ErrorInfo { Code = errorCode, Message = errorMessage }
        };
            return new Result(false, message, errors);
        }
    }
}
