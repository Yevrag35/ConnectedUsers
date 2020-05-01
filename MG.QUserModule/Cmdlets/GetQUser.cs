using MG.Posh.Extensions.Bound;
//using Microsoft.ActiveDirectory.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MG.QUserModule.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "QUser", DefaultParameterSetName = "None")]
    [OutputType(typeof(IQUserObject))]
    [Alias("Get-ConnectedUser", "gcu")]
    public class GetQUser : BaseQCmdlet
    {
        private const string DNS_HOSTNAME = "DNSHostName";
        //private List<string> comps;
        
        private int _tot;
        protected override string Activity => "Logged On User Query";
        protected override ICollection<string> Items => base.comps;
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
            else if (this.ContainsParameter(x => x.InputObject))
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
            else
                comps.Add("localhost");
        }

        protected override void EndProcessing()
        {
            _tot = comps.Count;
            List<IQUserObject> list = new List<IQUserObject>(_tot);
            if (comps.Count == 1)
            {
                try
                {
                    list.AddRange(GetQUserOutput(comps[0], _helper).Users);
                }
                catch (Exception e)
                {
                    base.WriteError(new ErrorRecord(e, e.GetType().FullName, ErrorCategory.NotSpecified, comps[0]));
                }
            }
            else if (comps.Count > 1)
            {
                list.AddRange(base.Execute());
            }
            
            WriteObject(list, true);
        }
    }
}
