using MG.QUserModule.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace MG.QUserModule.Cmdlets
{
    public class BaseQCmdlet : PSCmdlet
    {
        protected private const string COMPUTERNAME = "COMPUTERNAME";

        [Parameter(Mandatory = false, ParameterSetName = "SpecifyComputerName", ValueFromPipeline = true)]
        public string ComputerName = Environment.GetEnvironmentVariable(COMPUTERNAME);

        [Parameter(Mandatory = false)]
        public string UserName { get; set; }

        [Parameter(Mandatory = false)]
        public string SessionName { get; set; }

        [Parameter(Mandatory = false)]
        public int SessionId { get; set; }

        protected private IQUserHelper _helper;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            _helper = new QUserHelper();
            this.VerifyParameters();
        }

        protected private IEnumerable<IQUserObject> FilterBySessionName(IList<IQUserObject> list, string sessionName)
        {
            return _helper.PerformWildcardMatch(list, "SessionName", sessionName);
        }

        protected private IEnumerable<IQUserObject> FilterByUserName(IList<IQUserObject> list, string userName, IWildcardMatcher matcher) =>
            matcher.PerformWildcardMatch(list, "UserName", userName);

        protected private IEnumerable<IQUserObject> FilterBySessionId(IList<IQUserObject> list, int sessionId) =>
            list.Where(x => x.Id == sessionId);

        public static IList<IQUserObject> GetQUserOutput(string computerName, IQUserHelper helper) =>
            helper.RunQuery(computerName);

        public static async Task<IList<IQUserObject>> GetQUserOutputAsync(string computerName, IQUserHelper helper) =>
            await helper.RunQueryAsync(computerName);

        protected private void VerifyParameters()
        {
            var check = (this.MyInvocation.BoundParameters.ContainsKey("UserName") &&
                this.MyInvocation.BoundParameters.ContainsKey("SessionName")) || (
                this.MyInvocation.BoundParameters.ContainsKey("UserName") &&
                this.MyInvocation.BoundParameters.ContainsKey("SessionId")) ||
                (this.MyInvocation.BoundParameters.ContainsKey("SessionName") &&
                this.MyInvocation.BoundParameters.ContainsKey("SessionId"));
            if (check)
                throw new ArgumentException("You must only specify a UserName, SessionName, or SessionId!");
        }
    }
}
