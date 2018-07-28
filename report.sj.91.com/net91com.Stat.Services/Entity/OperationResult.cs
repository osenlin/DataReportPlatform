using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace net91com.Stat.Services.Entity
{
    public enum OperationResultType
    {
        unkown,
        successed,
        denied,
        failed
    }

    public class OperationResult
    {
        public OperationResultType resultType
        {
            get { return m_resultType; }
            set { m_resultType = value; }
        }

        private OperationResultType m_resultType;


        public string result
        {
            get { return Enum.GetName(typeof (OperationResultType), m_resultType); }
        }

        public string message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        private string m_message;

        public string exceptionStack
        {
            get { return m_exceptionStack; }
            set { m_exceptionStack = value; }
        }

        private string m_exceptionStack;

        public object obj
        {
            get { return m_object; }
            set { m_object = value; }
        }

        private object m_object;

        public OperationResult(OperationResultType resultType, string message, string exceptionStack, object obj)
        {
            m_resultType = resultType;
            m_message = message;
            m_exceptionStack = exceptionStack;
            m_object = obj;
        }

        public static OperationResult GetSuccessedResult()
        {
            return GetSuccessedResult(null);
        }

        public static OperationResult GetSuccessedResult(object obj)
        {
            return new OperationResult(OperationResultType.successed, String.Empty, String.Empty, obj);
        }

        public static OperationResult GetSuccessedResult(object obj, string message)
        {
            return new OperationResult(OperationResultType.successed, message, String.Empty, obj);
        }

        public static OperationResult GetDeniedResult(string message)
        {
            return new OperationResult(OperationResultType.denied, message, String.Empty, null);
        }

        public static OperationResult GetFailedResult(string message)
        {
            return GetFailedResult(message, null);
        }

        public static OperationResult GetFailedResult(string message, Exception ex)
        {
            if (ex != null)
            {
                return new OperationResult(OperationResultType.failed, message, ex.StackTrace, null);
            }
            else
            {
                return new OperationResult(OperationResultType.failed, message, String.Empty, null);
            }
        }

        public static OperationResult GetFailedResult(Exception ex)
        {
            if (ex != null)
            {
                return new OperationResult(OperationResultType.failed, ex.Message, ex.StackTrace, null);
            }
            else
            {
                return new OperationResult(OperationResultType.failed, String.Empty, String.Empty, null);
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string ToJson(Newtonsoft.Json.Converters.IsoDateTimeConverter converter)
        {
            return JsonConvert.SerializeObject(this, converter);
        }
    }
}