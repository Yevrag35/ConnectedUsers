using System;
using System.Reflection;

namespace MG.QUserModule
{
    public class QUserException : InvalidOperationException
    {
        private static readonly string defMsg = "An occurred while executing 'quser.exe' against {0}!" +
            Environment.NewLine + "{1}";

        public string OffendingServer { get; }

        public QUserException(string cmdArg) { }

        private static ParseServer(string cmdArg)
        {

        }
    }

    public class GenericMethodException : TargetException
    {
        private const string defMsg = "{0} threw an exception when searching for attribute values!";

        public Type AttributeType { get; }
        public string OffendingMethod { get; }
        public Enum OffendingEnum { get; }

        public GenericMethodException(Type attType, string methodName, Enum e)
            : base(string.Format(defMsg, methodName))
        {
            AttributeType = attType;
            OffendingMethod = methodName;
            OffendingEnum = e;
        }

        public GenericMethodException(Type attType, string methodName, Enum e, Exception innerException)
            : base(string.Format(defMsg, methodName), innerException)
        {
            AttributeType = attType;
            OffendingMethod = methodName;
            OffendingEnum = e;
        }
    }
}
