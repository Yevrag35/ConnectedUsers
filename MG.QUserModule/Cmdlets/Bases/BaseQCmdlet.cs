using MG.Posh.Extensions.Bound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace MG.QUserModule.Cmdlets
{
    public abstract class BaseQCmdlet : ProgressCmdlet
    {
        protected private const string COMPUTERNAME = "COMPUTERNAME";
        protected const string ERROR_FORMAT = "{0} - {1}";
        protected ComputerCollection comps;

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
            QUserResult result = null;
            try
            {
                result = await helper.RunQueryAsync(computerName);
            }
            catch (Exception e)
            {
                //base.WriteError(new ErrorRecord(e, e.GetType().FullName, ErrorCategory.InvalidResult, computerName));
                result = QUserResult.FromException(e);
            }
            return result;
        }

        protected private void VerifyParameters()
        {
            if ((this.ContainsParameter(x => x.UserName) &&
                this.ContainsAnyParameters(x => x.SessionName, x => x.SessionId))
                || this.ContainsAllParameters(x => x.SessionId, x => x.SessionName))
            {
                throw new ArgumentException("You must only specify a UserName, SessionName, or SessionId!");
            }
        }

        protected IEnumerable<IQUserObject> Execute(bool noProgress = false)
        {
            var taskList = new List<Task<QUserResult>>();
            var final = new List<QUserResult>();

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
                    Task<QUserResult> t = taskList[i];

                    if (t.IsCompleted)
                    {
                        if (!t.IsFaulted && !t.IsCanceled && t.Result != null)
                        {
                            if (t.Result.IsFaulted)
                                base.WriteError(new ErrorRecord(t.Result.Exception, t.Result.Exception.GetType().FullName, ErrorCategory.InvalidResult, t.Result));

                            else
                                final.Add(t.Result);
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
            return this.Filter(final);   
        }

        private IEnumerable<IQUserObject> Filter(IEnumerable<QUserResult> results)
        {
            IEnumerable<IQUserObject> many = results.SelectMany(x => x.Users);

            if (this.ContainsParameter(x => x.SessionId))
                many = many.Where(x => x.Id.Equals(this.SessionId));

            else if (this.ContainsParameter(x => x.SessionName))
                many = many.Where(x => x.SessionName.Equals(this.SessionName, StringComparison.InvariantCultureIgnoreCase));

            else if (this.ContainsParameter(x => x.UserName))
                many = many.Where(x => x.UserName.Equals(this.UserName, StringComparison.InvariantCultureIgnoreCase));

            return many;
        }

        protected Task<QUserResult> ProcessAsync(string computerName)
        {
            return GetQUserOutputAsync(computerName, _helper);
        }
    }
}
