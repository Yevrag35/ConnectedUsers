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

namespace MG.PowerShell.QUser
{
    [Cmdlet(VerbsCommon.Remove, "ConnectedUser", DefaultParameterSetName = "ByInputObject")]
    [Alias("Logoff-User", "Disconnect-User")]
    [CmdletBinding(PositionalBinding = false)]
    public class RemoveConnectedUser : PSCmdlet
    {
        private List<LogoffParameter> _parameters;
        private LogoffParameter _parameter;

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByInputObject")]
        public UserSession[] InputObject { get; set; } 

        [Parameter(Mandatory = false, ParameterSetName = "BySessionName")]
        [Parameter(Mandatory = false, ParameterSetName = "BySessionId")]
        public string ComputerName
        {
            get => _parameter.ServerName;
            set => _parameter.ServerName = value;
        }

        [Parameter(Mandatory = true, ParameterSetName = "BySessionName")]
        public string SessionName
        {
            get => _parameter.SessionName;
            set => _parameter.SessionName = value;
        }

        [Parameter(Mandatory = true, ParameterSetName = "BySessionId", Position = 0)]
        public int SessionId
        {
            get => _parameter.SessionId.GetValueOrDefault();
            set => _parameter.SessionId = value;
        }

        protected override void BeginProcessing()
        {
            _parameters = new List<LogoffParameter>(1);
            if (_parameter != null)
            {
                _parameters.Add(_parameter);
            }
        }
        protected override void ProcessRecord()
        {
            if (this.ContainsParameter(x => x.InputObject))
                _parameters.AddRange(UserSession.ToLogoff(this.InputObject));
        }

        protected override void EndProcessing()
        {
            if (this.ContainsBuiltinParameter(BuiltInParameter.Verbose))
            {
                _parameters.ForEach((x) =>
                {
                    x.Verbose = true;
                });
            }

            var ex = new LogoffExecutor();
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (!ex.Execute(_parameters[i]))
                {
                    this.WriteError<InvalidOperationException, RemoveConnectedUser>(
                        string.Join(Environment.NewLine, ex.ErrorLines),
                        ErrorCategory.InvalidResult);
                }
                else if (ex.StandardLines.Count > 0)
                {
                    this.WriteVerbose(string.Join(Environment.NewLine, ex.StandardLines));
                }
            }
        }
    }
}
