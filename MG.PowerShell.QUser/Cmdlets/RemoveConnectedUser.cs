using MG.Posh.Extensions.Bound;
using MG.Posh.Extensions.Shoulds;
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
    [Cmdlet(VerbsCommon.Remove, "ConnectedUser", DefaultParameterSetName = "ByInputObject",
        SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    [Alias("Logoff-User", "Disconnect-User")]
    [CmdletBinding(PositionalBinding = false)]
    public class RemoveConnectedUser : PSCmdlet
    {
        private List<LogoffParameter> _parameters;
        private LogoffParameter _parameter;
        private bool _force;

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByInputObject")]
        public UserSession[] InputObject { get; set; } 

        [Parameter(Mandatory = false, ParameterSetName = "BySessionName")]
        [Parameter(Mandatory = false, ParameterSetName = "BySessionId")]
        public string ComputerName
        {
            get => _parameter.ServerName;
            set => this.SetLogoffParameter(p => p.ServerName = value);
        }

        [Parameter(Mandatory = true, ParameterSetName = "BySessionName")]
        public string SessionName
        {
            get => _parameter.SessionName;
            set => this.SetLogoffParameter(p => p.SessionName = value);
        }

        [Parameter(Mandatory = true, ParameterSetName = "BySessionId", Position = 0)]
        public int SessionId
        {
            get => _parameter.SessionId.GetValueOrDefault();
            set => this.SetLogoffParameter(p => p.SessionId = value);
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force
        {
            get => _force;
            set => _force = value;
        }

        protected override void BeginProcessing()
        {
            _parameters = new List<LogoffParameter>(1);
            if (this.ContainsAnyParameters(x => x.ComputerName, x => x.SessionName, x => x.SessionId))
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
            var cmdExecutor = new LogoffExecutor();
            this.SetVerboseArgument();
            this.ExecuteCommandsFromArguments(cmdExecutor, _parameters, _force);
        }

        private void ExecuteCommandsFromArguments(IExecutor cmdExecutor, List<LogoffParameter> parameterList, bool forceWanted)
        {
            for (int i = 0; i < parameterList.Count; i++)
            {
                LogoffParameter singleParameter = parameterList[i];
                (string, object) msgTgt = this.GetShouldProcessTarget(singleParameter);

                if (forceWanted || this.ShouldProcessFormat("Remove", msgTgt.Item1, msgTgt.Item2))
                {
                    this.ExecuteSingleParameter(cmdExecutor, singleParameter);
                }
            }
        }
        private void ExecuteSingleParameter(IExecutor executor, IParameterBuilder parameter)
        {
            if (!executor.Execute(parameter))
            {
                this.WriteError<InvalidOperationException, RemoveConnectedUser>(
                    string.Join(Environment.NewLine, executor.ErrorLines),
                    ErrorCategory.InvalidResult);
            }
            else if (executor.StandardLines.Count > 0)
            {
                this.WriteVerbose(string.Join(Environment.NewLine, executor.StandardLines));
            }
            executor.Reset();
        }
        private (string, object) GetShouldProcessTarget(LogoffParameter parameter)
        {
            string target = "The current user (i.e. - YOU!)";
            object arg = null;
            if (!string.IsNullOrEmpty(parameter.SessionName))
            {
                target = "Session Name: {0}";
                arg = parameter.SessionName;
            }
            else if (parameter.SessionId.HasValue)
            {
                target = "Session ID: {0}";
                arg = parameter.SessionId.Value;
            }

            if (!string.IsNullOrEmpty(parameter.ServerName))
                target = string.Format("({0}) - {1}", parameter.ServerName, target);

            return (target, arg);
        }
        private void SetLogoffParameter(Action<LogoffParameter> action)
        {
            if (_parameter == null)
                _parameter = new LogoffParameter();

            action.Invoke(_parameter);
        }
        private void SetVerboseArgument()
        {
            if (this.ContainsBuiltinParameter(BuiltInParameter.Verbose))
            {
                _parameters.ForEach(x =>
                {
                    x.Verbose = true;
                });
            }
        }
    }
}
