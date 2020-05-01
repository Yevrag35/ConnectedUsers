using MG.Posh.Extensions.Bound;
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
        public string[] ComputerName = new string[1] { Environment.GetEnvironmentVariable(COMPUTERNAME) };

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

        public List<QUserResult> GetMultiQUserOutput(string[] computerNames, IQUserHelper helper)
        {
            var list = new List<QUserResult>(computerNames.Length);
            foreach (string computerName in computerNames)
            {
                list.Add(GetQUserOutput(computerName, helper));
            }
            return list;
        }

        public QUserResult GetQUserOutput(string computerName, IQUserHelper helper)
        {
            //List<IQUserObject> retList = null;
            QUserResult result = null;
            try
            {
                result = helper.RunQuery(computerName);
            }
            catch (Exception e)
            {
                base.WriteError(new ErrorRecord(e, e.GetType().FullName, ErrorCategory.InvalidResult, computerName));
                result = QUserResult.FromException(e);
            }
            return result;
        }

        public async Task<QUserResult> GetQUserOutputAsync(string computerName, IQUserHelper helper)
        {
            //List<IQUserObject> retList = null;
            QUserResult result = null;
            try
            {
                result = await helper.RunQueryAsync(computerName);
            }
            catch (Exception e)
            {
                base.WriteError(new ErrorRecord(e, e.GetType().FullName, ErrorCategory.InvalidResult, computerName));
                result = QUserResult.FromException(e);
            }
            return result;
        }

        protected private void VerifyParameters()
        {
            //bool check = (this.MyInvocation.BoundParameters.ContainsKey("UserName") &&
            //    this.MyInvocation.BoundParameters.ContainsKey("SessionName")) || (
            //    this.MyInvocation.BoundParameters.ContainsKey("UserName") &&
            //    this.MyInvocation.BoundParameters.ContainsKey("SessionId")) ||
            //    (this.MyInvocation.BoundParameters.ContainsKey("SessionName") &&
            //    this.MyInvocation.BoundParameters.ContainsKey("SessionId"));

            if ((this.ContainsParameter(x => x.UserName) &&
                this.ContainsAnyParameters(x => x.SessionName, x => x.SessionId))
                || this.ContainsAllParameters(x => x.SessionId, x => x.SessionName))
            {
                throw new ArgumentException("You must only specify a UserName, SessionName, or SessionId!");
            }
        }
    }
}
