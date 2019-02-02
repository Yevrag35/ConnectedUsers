using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.QUserModule.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "QUser", DefaultParameterSetName = "None")]
    [OutputType(typeof(IQUserObject))]
    public class GetQUser : BaseQCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ByADComputerPipeline", DontShow = true,
            ValueFromPipeline = true)]
        public PSObject InputObject { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByADComputerPipeline")]
        public string Property = "Name";

        [Parameter(Mandatory = false, ParameterSetName = "FindUserNameAndComputer")]
        [Parameter(Mandatory = false, ParameterSetName = "FindSessionNameAndComputer")]
        [Parameter(Mandatory = false, ParameterSetName = "FindSessionIdAndComputer")]
        public string ComputerName = Environment.GetEnvironmentVariable("COMPUTERNAME");

        [Parameter(Mandatory = false, ParameterSetName = "FindUserNameAndComputer")]
        public string UserName { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "FindSessionNameAndComputer")]
        public string SessionName { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "FindSessionIdAndComputer")]
        public int SessionId { get; set; }

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            if (ParameterSetName != "ByADComputerPipeline")
            {
                IList<IQUserObject> objs = GetQUserOutput(ComputerName, _helper);
                if (this.MyInvocation.BoundParameters.ContainsKey("UserName"))
                    WriteObject(this.FilterByUserName(objs, this.UserName), true);

                else if (this.MyInvocation.BoundParameters.ContainsKey("SessionName"))
                    WriteObject(this.FilterBySessionName(objs, this.SessionName), true);

                else if (this.MyInvocation.BoundParameters.ContainsKey("SessionId"))
                    WriteObject(this.FilterBySessionId(objs, this.SessionId), true);

                else
                    WriteObject(objs, true);
            }
            else
            {
                var compStr = this.GetADName(InputObject);
                IList<IQUserObject> objs = GetQUserOutput(compStr, _helper);
                WriteObject(objs, true);
            }
        }

        private IEnumerable<IQUserObject> FilterBySessionName(IList<IQUserObject> list, string sessionName)
        {
            return _helper.PerformWildcardMatch(list, "SessionName", sessionName);
        }

        private IEnumerable<IQUserObject> FilterByUserName(IList<IQUserObject> list, string userName) =>
            _helper.PerformWildcardMatch(list, "UserName", userName);

        private IEnumerable<IQUserObject> FilterBySessionId(IList<IQUserObject> list, int sessionId) =>
            list.Where(x => x.Id == sessionId);

        public static IList<IQUserObject> GetQUserOutput(string computerName, IQUserHelper helper) => 
            helper.RunQuery(computerName);

        private string GetADName(PSObject psObj)
        {
            var prop = psObj.Properties.Single(x => x.Name.Equals(Property, StringComparison.InvariantCultureIgnoreCase));
            return prop.Value as string;
        }
    }
}
