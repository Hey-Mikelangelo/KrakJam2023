using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;



namespace Utilities
{
    public class MethodCallGenerator
    {
        private StringBuilder mainSb = new StringBuilder();
        private StringBuilder sb = new StringBuilder();
        public override string ToString()
        {
            return mainSb.ToString();
        }
        public MethodCallGenerator WithReturnedValue(string returnVariableName, Type returnType)
        {
            sb.Length = 0;
            if (returnType != null)
            {
                sb.Append(returnType.FullName);
                sb.Space();
            }
            sb.Append(returnVariableName).Space().Append('=').Space();
            mainSb.Insert(0, sb.ToString());
            return this;
        }
        public MethodCallGenerator CreateMethod(MethodBase method, IReadOnlyList<string> methodArguments,
            string callerName)
        {
            sb.Length = 0;
            mainSb.Length = 0;
            sb.Append(callerName);
            sb.Append(".");
            string methodName = method.GetInCodeMethodName();
            sb.Append(methodName);

            var isGetter = method.IsGetter();
            var isSetter = method.IsSetter();
            if (isSetter)
            {
                sb.Append(" = (");
            }
            else if (isGetter == false)
            {
                sb.Append('(');
            }

            for (int i = 0; i < methodArguments.Count; i++)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }
                var methodArgument = methodArguments[i];
                sb.Append(methodArgument);

            }
            if (isGetter == false)
            {
                sb.Append(')');
            }
            sb.Append(';');
            mainSb.Append(sb);
            return this;
        }

        
    }

    
}



