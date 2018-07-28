using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace net91com.Reports.Services.ServicesBaseEntity
{
    public enum OperationResultCode
    {
        unkown = -1,
        successed = 0,
        denied = 1,
        failed = 2
    }

    public class Result
    {
        public Result(OperationResultCode resultCode, string message, string exceptionStack, object obj,
                      bool m_Showstate)
        {
            this.resultCode = (int) resultCode;
            this.message = message;
            this.exceptionStack = exceptionStack;
            data = obj;
            this.m_Showstate = m_Showstate;
        }

        public int resultCode { get; set; }

        public string message { get; set; }

        public string exceptionStack { get; set; }

        public object data { get; set; }


        public bool m_Showstate { get; set; }

        public bool OriginalString { get; set; }


        /// <summary>
        ///     showstate 是直接字符串输出还是封装成result类的一个属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="showstate"></param>
        /// <returns></returns>
        public static Result GetSuccessedResult(object obj, bool showstate)
        {
            return new Result(OperationResultCode.successed, String.Empty, String.Empty, obj, showstate);
        }

        /// <summary>
        ///     showstate 是直接字符串输出还是封装成result类的一个属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="showstate"></param>
        /// <returns></returns>
        public static Result GetSuccessedResult(object obj, bool showstate, bool oriString)
        {
            var m = new Result(OperationResultCode.successed, String.Empty, String.Empty, obj, showstate);
            m.OriginalString = oriString;
            return m;
        }

        public static Result GetSuccessedResult()
        {
            var m = new Result(OperationResultCode.successed, String.Empty, String.Empty, null, false);
            return m;
        }


        public static Result GetSuccessedResult(object obj, string message, bool showstate)
        {
            return new Result(OperationResultCode.successed, message, String.Empty, obj, showstate);
        }


        public static Result GetDeniedResult(string message)
        {
            return new Result(OperationResultCode.denied, message, String.Empty, null, true);
        }

        public static Result GetFailedResult(string message)
        {
            return new Result(OperationResultCode.failed, message, String.Empty, null, true);
        }

        public static Result GetFailedResult(string message, Exception ex)
        {
            //if (ex != null)
            //{
            //    return new OperationResult(OperationResultType.failed, message, ex.StackTrace, null);
            //}
            //else
            //{
            //    return new OperationResult(OperationResultType.failed, message, String.Empty, null);
            //}
            //错误信息不返回给客户端
            return new Result(OperationResultCode.failed, message, String.Empty, null, true);
        }

        public static Result GetFailedResult(Exception ex)
        {
            //if (ex != null)
            //{
            //    return new OperationResult(OperationResultType.failed, ex.Message, ex.StackTrace, null);
            //}
            //else
            //{
            //    return new OperationResult(OperationResultType.failed, String.Empty, String.Empty, null);
            //}
            //错误信息不返回给客户端
            return new Result(OperationResultCode.failed, String.Empty, String.Empty, null, true);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToJson(IsoDateTimeConverter converter)
        {
            return JsonConvert.SerializeObject(this, converter);
        }
    }
}