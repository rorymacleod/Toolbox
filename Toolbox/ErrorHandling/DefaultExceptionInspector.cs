using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Provides analysis of exceptions of type <see cref="Exception"/>.
    /// </summary>
    public class DefaultExceptionInspector : IExceptionInspector
    {
        public virtual void GetData(Exception exception, IDictionary<string, string> data)
        {
            foreach (string key in exception.Data.Keys)
            {
                data[key] = Convert.ToString(exception.Data[key]) ?? string.Empty;
            }

            if (exception is ISupportsMessageData dataEx)
            {
                dataEx.GetMessageData(data);
            }
        }

        public virtual string GetMessage(Exception exception, IDictionary<string, string>? data,
            ExceptionFormatting formatting)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            var sb = new StringBuilder();
            switch (formatting)
            {
                case ExceptionFormatting.SingleLine:
                    this.AppendMessage(sb, exception);
                    break;

                case ExceptionFormatting.Multiline:
                    this.AppendExceptionName(sb, exception);
                    this.AppendMessage(sb, exception);
                    break;

                default:
                    this.AppendExceptionName(sb, exception);
                    this.AppendMessage(sb, exception);
                    if (data?.Count > 0)
                    {
                        sb.AppendLine();
                        this.AppendAllData(sb, data);
                    }
                    break;
            }

            return sb.ToString();
        }

        public virtual bool IsSignificant(Exception exception)
        {
            return exception is not AggregateException;
        }

        public virtual bool IsTransient(Exception exception)
        {
            if (exception is ISupportsTransient transientEx)
            {
                return transientEx.IsTransient;
            }

            // Some exception types are always transient.
            return exception is ThreadAbortException or OperationCanceledException or TimeoutException ||
                exception.InnerException != null && exception.InnerException.IsTransient();
        }

        private void AppendAllData(StringBuilder sb, IDictionary<string, string> data)
        {
            int i = 0;
            foreach (var (key, value) in data)
            {
                sb.Append($"- {key}: {value}");
                if (++i < data.Count)
                {
                    sb.AppendLine();
                }
            }
        }

        private void AppendExceptionName(StringBuilder sb, Exception exception)
        {
            var exType = exception.GetType();
            var name = exType == typeof(Exception) ? exType.Name : exType.Name.TrimEnd("Exception",
                StringComparison.InvariantCultureIgnoreCase);
            sb.AppendFormat("({0}) ", name);
        }

        protected virtual void AppendMessage(StringBuilder sb, Exception exception)
        {
            sb.Append(exception.Message);
        }
    }
}
