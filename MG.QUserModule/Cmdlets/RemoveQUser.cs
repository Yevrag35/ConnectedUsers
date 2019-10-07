using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.QUserModule.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "QUser", DefaultParameterSetName = "SpecifyComputerName", SupportsShouldProcess = true,
        ConfirmImpact = ConfirmImpact.High)]
    [OutputType(typeof(void))]
    [Alias("Remove-ConnectedUser", "rcu")]
    public class RemoveQUser : BaseQCmdlet
    {
        private const string ACTIVITY = "Remote User Logoff";
        private const string COMPLETED = "Completed!";
        private const string STATUS_FORMAT = "Logging off user {0}/{1}... {2}";
        private const string MULTI_STATUS_FORMAT = "{0} - {1}";
        private const string LH = "localhost";
        private QUserRemover Remover { get; set; }
        private IWildcardMatcher Matcher => this.Remover;
        private List<IQUserObject> _list;
        private int TotalCount => _list.Count;


        #region PARAMETERS

        [Parameter(Mandatory = true, ParameterSetName = "ViaPipeline", DontShow = true, ValueFromPipeline = true)]
        public IQUserObject RemoteUserObject { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        [Parameter(Mandatory = false)]
        public int TimeoutInMs = 3000;

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            if (this.MyInvocation.BoundParameters.ContainsKey("Force") &&
                ((SwitchParameter)this.MyInvocation.BoundParameters["Force"]).ToBool() &&
                this.MyInvocation.BoundParameters.ContainsKey("WhatIf") &&
                ((SwitchParameter)this.MyInvocation.BoundParameters["WhatIf"]).ToBool())
            {
                throw new ArgumentException("'-WhatIf' cannot be combined with '-Force'!");
            }
            this.Remover = new QUserRemover();
            _list = new List<IQUserObject>();
        }

        protected override void ProcessRecord()
        {
            if (!this.ParameterSetName.Contains("Pipeline"))
            {
                IList<IQUserObject> objs = GetQUserOutput(ComputerName, this.TimeoutInMs, _helper);
                if (this.MyInvocation.BoundParameters.ContainsKey("UserName"))
                    _list.AddRange(base.FilterByUserName(objs, this.UserName, this.Matcher));

                if (this.MyInvocation.BoundParameters.ContainsKey("SessionName"))
                    _list.AddRange(base.FilterBySessionName(objs, this.SessionName));

                if (this.MyInvocation.BoundParameters.ContainsKey("SessionId"))
                    _list.AddRange(base.FilterBySessionId(objs, this.SessionId));
            }
            else
            {
                _list.Add(this.RemoteUserObject);
            }
        }

        protected override void EndProcessing()
        {
            for (int i = 1; i <= _list.Count; i++)
            {
                IQUserObject sid = _list[i-1];
                if (this.Force || this.ShouldProcess(sid.UserName + " - " + sid.HostName, "Logging off"))
                {
                    this.UpdateProgress(0, i, sid);
                    this.WriteVerbose("Executing remote log off of '" + sid.UserName + "' on '" + sid.HostName + "'...");
                    this.Remover.LogoffRemoteUser(sid);
                }
            }
            this.UpdateProgress(0);
        }

        private void UpdateProgress(int id, int on, IQUserObject sid)
        {
            string status;
            if (sid.HostName == LH || sid.HostName == Environment.GetEnvironmentVariable(COMPUTERNAME))
                status = string.Format(STATUS_FORMAT, on, this.TotalCount, sid.UserName);
            else
                status = string.Format(MULTI_STATUS_FORMAT, sid.HostName, string.Format(STATUS_FORMAT, on, this.TotalCount, sid.UserName));

            var pr = new ProgressRecord(id, ACTIVITY, status);

            double num = Math.Round((((double)on / (double)this.TotalCount) * 100), 2, MidpointRounding.ToEven);
            pr.PercentComplete = Convert.ToInt32(num);
            this.WriteProgress(pr);
        }

        private void UpdateProgress(int id)     // Complete it!
        {
            var pr = new ProgressRecord(id, ACTIVITY, COMPLETED)
            {
                RecordType = ProgressRecordType.Completed
            };
            this.WriteProgress(pr);
        }
    }
}
