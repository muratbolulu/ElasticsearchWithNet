using System.Net;

namespace Elasticsearch.Api.DTOs
{
    public record ResponseDto<T>
    {
        public T? Data { get; set; }
        public List<String>? Errors { get; set; }
        public HttpStatusCode Status { get; set; }

        //static Factory Method
        public static ResponseDto<T> Success(T data, HttpStatusCode status)
        {
            return new ResponseDto<T> { Data = data, Status = status };
        }

        //static Factory Method
        public static ResponseDto<T> Fail(List<string> errors, HttpStatusCode status)
        {
            return new ResponseDto<T> { Errors = errors, Status = status };
        }

        //static Factory Method
        public static ResponseDto<T> Fail(string error, HttpStatusCode status)
        {
            return new ResponseDto<T> { Errors = new List<string> { error }, Status = status };
        }
    }
}
