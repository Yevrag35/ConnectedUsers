using MG.Posh.Extensions.Bound;
using MG.Posh.Extensions.Writes;
using MG.PowerShell.QUser.Executor;
using MG.PowerShell.QUser.Models;
using MG.PowerShell.QUser.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.PowerShell.QUser.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "ConnectedUser", DefaultParameterSetName = "None")]
    [Alias("Get-QUser")]
    public class GetConnectedUser : PSCmdlet
    {
        #region FIELDS/CONSTANTS
        private QUserParameter _parameter = new QUserParameter();

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public string ComputerName
        {
            get => _parameter.ServerName;
            set => _parameter.ServerName = value;
        }

        [Parameter(Mandatory = true, ParameterSetName = "ByUserName")]
        public string UserName
        {
            get => _parameter.UserName;
            set => _parameter.UserName = value;
        }

        [Parameter(Mandatory = true, ParameterSetName = "BySessionName")]
        public string SessionName
        {
            get => _parameter.ServerName;
            set => _parameter.SessionName = value;
        }

        [Parameter(Mandatory = true, ParameterSetName = "BySessionId")]
        public int SessionId
        {
            get => _parameter.SessionId.GetValueOrDefault();
            set => _parameter.SessionId = value;
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            var executor = new QUserExecutor();
            if (executor.Execute(_parameter))
            {
                var lineParser = new LineParser();
                lineParser.ParseHeaderLine(executor.UserLines[0]);

                for (int i = 1; i < executor.UserLines.Count; i++)
                {
                    string line = executor.UserLines[i];
                    bool isCurrent = !line.StartsWith(">") ? false : true;

                    UserSession session = lineParser.ParseUserLine(line, isCurrent);
                    if (!this.ContainsParameter(x => x.ComputerName))
                        this.ComputerName = this.GetLocalHostName();

                    session.HostName = this.ComputerName;

                    base.WriteObject(session);
                }
            }
            else if (executor.NoUserExists)
            {
                base.WriteWarning(
                    string.Format("{0} - {1}",
                    this.ComputerName,
                    string.Join(Environment.NewLine, executor.ErrorLines)
                ));
            }
            else
            {
                string msg = string.Join(Environment.NewLine, executor.ErrorLines);
                if (this.ContainsParameter(x => x.ComputerName))
                    msg = string.Format("{0} - {1}", this.ComputerName, msg);

                this.WriteError<InvalidOperationException, GetConnectedUser>(msg, ErrorCategory.InvalidResult, this.ComputerName);
            }
        }

        #endregion

        #region BACKEND METHODS
        private string GetLocalHostName()
        {
            return Environment.GetEnvironmentVariable("COMPUTERNAME");
        }

        #endregion
    }
}