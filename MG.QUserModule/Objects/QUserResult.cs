using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MG.QUserModule
{
    public class QUserResult
    {
        private static readonly string[] LINE_BREAKS = new string[3]
        {
            "\r\n", "\r", "\n"
        };
        private const StringSplitOptions SPLIT_OPTION = StringSplitOptions.RemoveEmptyEntries;

        private List<string> _rawStandardOutput { get; } = new List<string>(2);
        private string _rawErrorString;

        public bool HasOutput => this.StandardOutput.Count > 0;
        public Exception Exception { get; }
        public bool IsFaulted => this.Exception != null;
        public IReadOnlyList<string> StandardOutput => _rawStandardOutput;
        public List<IQUserObject> Users { get; set; }

        private QUserResult(Exception ex)
        {
            this.Exception = ex;
        }
        private QUserResult(StreamReader standard, StreamReader error)
        {
            using (standard)
            {
                while (!standard.EndOfStream)
                {
                    _rawStandardOutput.Add(standard.ReadLine());
                }
                if (_rawStandardOutput.Count > 1)
                    _rawStandardOutput.RemoveAt(0);
            }
            using (error)
            {
                _rawErrorString = error.ReadToEnd();
            }

            if (!string.IsNullOrWhiteSpace(_rawErrorString))
            {
                this.Exception = new InvalidOperationException(_rawErrorString);
            }
        }

        public static QUserResult FromException(Exception error) => new QUserResult(error);
        public static QUserResult ReadProcess(Process finishedProcess)
        {
            if (!finishedProcess.HasExited)
                return null;

            return new QUserResult(finishedProcess.StandardOutput, finishedProcess.StandardError);
        }
    }
}
