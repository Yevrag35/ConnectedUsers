﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MG.PowerShell.QUser.Executor
{
    public class LogoffParameter : IParameterBuilder
    {

        private StringBuilder _builder;

        public string ServerName { get; set; }
        public string SessionName { get; internal set; }
        public int? SessionId { get; internal set; }
        public bool Verbose { get; set; }

        internal LogoffParameter()
        {
            _builder = new StringBuilder();
        }

        public string Build()
        {
            if (!string.IsNullOrEmpty(this.SessionName))
                _builder.Append(this.SessionName);

            else if (this.SessionId.HasValue)
                _builder.Append(this.SessionId.Value);

            if (!string.IsNullOrEmpty(this.ServerName))
            {
                if (_builder.Length > 0)
                    _builder.Append(Constants.SPACE);

                _builder.Append(Constants.SERVER_PARAM);
                _builder.Append(this.ServerName);
            }

            if (this.Verbose)
            {
                if (_builder.Length > 0)
                    _builder.Append(Constants.SPACE);

                _builder.Append(Constants.VERBOSE);
            }

            string result = _builder.ToString();
            _builder.Clear();
            return result;
        }
    }
}
