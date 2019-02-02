using MG.QUserModule.Objects;
using Microsoft.ActiveDirectory.Management;
using System;
using System.Management.Automation;
using System.Reflection;

namespace MG.QUserModule.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "QUser", DefaultParameterSetName = "None")]
    [OutputType(typeof(IQUserObject))]
    [Alias("Get-ConnectedUser", "gcu")]
    public class GetQUser : BaseQCmdlet
    {
        private const string DNS_HOSTNAME = "DNSHostName";

        [Parameter(Mandatory = true, ParameterSetName = "ByADComputerPipeline", DontShow = true,
            ValueFromPipeline = true)]
        public ADComputer InputObject { get; set; }

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            string compStr = ComputerName;
            if (this.ParameterSetName == "ByADComputerPipeline")
            {
                compStr = this.InputObject.DNSHostName;
            }
            var objs = GetQUserOutput(compStr, _helper);
            if (this.MyInvocation.BoundParameters.ContainsKey("UserName"))
                this.WriteObject(this.FilterByUserName(objs, this.UserName, _helper), true);

            else if (this.MyInvocation.BoundParameters.ContainsKey("SessionName"))
                this.WriteObject(this.FilterBySessionName(objs, this.SessionName), true);

            else if (this.MyInvocation.BoundParameters.ContainsKey("SessionId"))
                this.WriteObject(this.FilterBySessionId(objs, this.SessionId), true);

            else
                this.WriteObject(objs, true);
        }

        private string ResolveAD(PSObject adObject)
        {
            var adType = adObject.ImmediateBaseObject.GetType();
            var prop = adType.GetProperty(DNS_HOSTNAME, BindingFlags.Public | BindingFlags.Instance);
            return prop.GetValue(adObject.ImmediateBaseObject) as string;
        }
    }
}
