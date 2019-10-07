using MG.QUserModule.Objects;
using Microsoft.ActiveDirectory.Management;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MG.QUserModule.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "QUser", DefaultParameterSetName = "None")]
    [OutputType(typeof(IQUserObject))]
    [Alias("Get-ConnectedUser", "gcu")]
    public class GetQUser : ProgressCmdlet
    {
        private const string DNS_HOSTNAME = "DNSHostName";
        private List<string> comps;
        private int _tot;
        protected override string Activity => "Logged On User Query";
        protected override ICollection<string> Items => comps;
        protected override string StatusFormat => "Querying machine {0}/{1}...";
        protected override int TotalCount => _tot;

        [Parameter(Mandatory = true, ParameterSetName = "ByADComputerPipeline", DontShow = true,
            ValueFromPipeline = true)]
        public ADComputer InputObject { get; set; }

        [Parameter(Mandatory = false)]
        public int TimeoutInMs = 3000;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            comps = new List<string>();
        }

        protected override void ProcessRecord()
        {
            string compStr = ComputerName;
            if (this.ParameterSetName == "ByADComputerPipeline")
            {
                compStr = this.InputObject.DNSHostName;
            }
            comps.Add(compStr);
        }

        protected override void EndProcessing()
        {
            _tot = comps.Count;
            List<IQUserObject> list = null;
            if (_tot == 1)
            {
                list = GetQUserOutput(comps[0], this.TimeoutInMs, _helper);
            }
            else
            {
                list = this.Execute();
            }
            WriteObject(list, true);
        }

        protected private List<IQUserObject> Execute()
        {
            var taskList = new List<Task<IEnumerable<IQUserObject>>>();
            var final = new List<IQUserObject>();

            for (int n = 0; n < comps.Count; n++)
            {
                taskList.Add(this.ProcessAsync(comps[n]));
            }

            while (taskList.Count > 0)
            {
                this.UpdateProgress(0, taskList.Count);
                for (int i = taskList.Count - 1; i >= 0; i--)
                {
                    Task<IEnumerable<IQUserObject>> t = taskList[i];

                    if (t.IsCompleted)
                    {
                        if (!t.IsFaulted && !t.IsCanceled && t.Result != null)
                        {
                            final.AddRange(t.Result);
                        }
                        taskList.Remove(t);
                    }
                }
                Thread.Sleep(1000);
            }
            this.UpdateProgress(0);
            return final;
        }

        private async Task<IEnumerable<IQUserObject>> ProcessAsync(string computerName)
        {
            IList<IQUserObject> objs = null;
            try
            {
                objs = await GetQUserOutputAsync(computerName, this.TimeoutInMs, _helper);
            }
            catch
            {
                return null;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey("UserName"))
                return this.FilterByUserName(objs, this.UserName, _helper);
            //this.WriteObject(this.FilterByUserName(objs, this.UserName, _helper), true);

            else if (this.MyInvocation.BoundParameters.ContainsKey("SessionName"))
                return this.FilterBySessionName(objs, this.SessionName);
            //this.WriteObject(this.FilterBySessionName(objs, this.SessionName), true);

            else if (this.MyInvocation.BoundParameters.ContainsKey("SessionId"))
                return this.FilterBySessionId(objs, this.SessionId);
            //this.WriteObject(this.FilterBySessionId(objs, this.SessionId), true);

            else
                return objs;
            //this.WriteObject(objs, true);
        }

        private string ResolveAD(PSObject adObject)
        {
            Type adType = adObject.ImmediateBaseObject.GetType();
            PropertyInfo prop = adType.GetProperty(DNS_HOSTNAME, BindingFlags.Public | BindingFlags.Instance);
            return prop.GetValue(adObject.ImmediateBaseObject) as string;
        }
    }
}
