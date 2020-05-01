using MG.Posh.Extensions.Bound;
//using Microsoft.ActiveDirectory.Management;
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
        //private List<string> comps;
        private ComputerCollection comps;
        private int _tot;
        protected override string Activity => "Logged On User Query";
        protected override ICollection<string> Items => comps;
        protected override string StatusFormat => "Querying machine {0}/{1}...";
        protected override int TotalCount => _tot;

        [Parameter(Mandatory = true, ParameterSetName = "ByADComputerPipeline", DontShow = true,
            ValueFromPipeline = true)]
        public PSObject InputObject { get; set; }
        //public ADComputer InputObject { get; set; }

        //[Parameter(Mandatory = false)]
        //public int TimeoutInMs = 3000;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            comps = new ComputerCollection();
        }

        protected override void ProcessRecord()
        {
            if (this.ContainsParameter(x => x.ComputerName))
            {
                comps.UnionWith(this.ComputerName);
            }

            if (this.ContainsParameter(x => x.InputObject))
            {
                string compStr = null;
                foreach (PSPropertyInfo psProp in this.InputObject.Properties)
                {
                    if (psProp.Name.Equals("DnsHostName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        compStr = psProp.Value as string;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(compStr))
                {
                    comps.Add(compStr);
                }
            }
            
        }

        protected override void EndProcessing()
        {
            _tot = comps.Count;
            List<IQUserObject> list = null;
            if (comps.Count == 1)
            {
                try
                {
                    list = GetQUserOutput(comps[0], _helper);
                }
                catch (Exception e)
                {
                    base.WriteError(new ErrorRecord(e, e.GetType().FullName, ErrorCategory.NotSpecified, comps[0]));
                }
            }
            else if (comps.Count > 1)
            {
                list = this.Execute();
            }
            
            WriteObject(list, true);
        }

        protected private List<IQUserObject> Execute(bool noProgress = false)
        {
            var taskList = new List<Task<IEnumerable<IQUserObject>>>();
            var final = new List<IQUserObject>();

            for (int n = 0; n < comps.Count; n++)
            {
                taskList.Add(this.ProcessAsync(comps[n]));
            }

            while (taskList.Count > 0)
            {
                //if (!noProgress)
                //{
                    this.UpdateProgress(0, taskList.Count);
                //}
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
                Thread.Sleep(100);
            }
            //if (!noProgress)
            //{
                this.UpdateProgress(0);
            //}
            return final;
        }

        private async Task<IEnumerable<IQUserObject>> ProcessAsync(string computerName)
        {
            IList<IQUserObject> objs = null;
            try
            {
                objs = await GetQUserOutputAsync(computerName, _helper);
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
