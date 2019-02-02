using MG.QUserModule.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace MG.QUserModule.Cmdlets
{
    public class BaseQCmdlet : PSCmdlet
    {
        protected private IQUserHelper _helper;
        protected override void BeginProcessing() => _helper = new QUserHelper();
    }
}
