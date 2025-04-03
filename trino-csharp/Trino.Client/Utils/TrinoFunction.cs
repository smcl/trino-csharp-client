using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trino.Client.Utils
{
    public class TrinoFunction
    {
        private readonly string catalog;
        private readonly string functionName;
        private readonly IList<object> Parameters;

        public TrinoFunction(string catalog, string functionName, IList<object> parameters)
        {
            this.catalog = catalog;
            this.functionName = functionName;
            this.Parameters = parameters;
        }

        public virtual Task<RecordExecutor> ExecuteAsync(ClientSession session)
        {
            string statement = BuildFunctionStatement();
            return RecordExecutor.Execute(session, statement);
        }

        protected virtual string BuildFunctionStatement()
        {
            var fullFunctionName = BuildFullFunctionName();
            var parameterString = string.Join(", ", FormatParameters());

            return $"{fullFunctionName}({parameterString})";
        }

        private string BuildFullFunctionName()
        {
            if (!string.IsNullOrEmpty(catalog))
            {
                return $"{catalog}.{functionName}";
            }

            return functionName;
        }

        private IEnumerable<string> FormatParameters()
        {
            foreach (var parameter in Parameters)
            {
                if (IsNumeric(parameter))
                {
                    yield return parameter.ToString();
                }
                else if (parameter is bool)
                {
                    yield return (bool)parameter ? "1" : "0";
                }
                else if (parameter is DateTime)
                {
                    yield return QuoteParameter(((DateTime)parameter).ToString("s"));
                }
                else if (parameter is DateTimeOffset)
                {
                    yield return QuoteParameter(((DateTimeOffset)parameter).ToString("s"));
                }
                else
                {
                    yield return QuoteParameter(parameter.ToString());
                }
            }
        }

        private static bool IsNumeric(object parameter)
        {
            return parameter is byte || parameter is sbyte
                || parameter is short || parameter is ushort
                || parameter is int || parameter is uint
                || parameter is long || parameter is ulong
                || parameter is IntPtr || parameter is UIntPtr
                || parameter is float || parameter is double
                || parameter is decimal;
        }

        private static string QuoteParameter(string formattedParameter)
        {
            return $"'{formattedParameter}'";
        }
    }
}
