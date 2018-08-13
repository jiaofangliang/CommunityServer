/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Net;

namespace ASC.Mail.Aggregator.Common
{
    public class Pop3ConnectionException : Exception
    {
        public Pop3ConnectionException(string message) : base(message) { }
    }

    public class Pop3ConnectionTimeoutException : Pop3ConnectionException
    {
        public Pop3ConnectionTimeoutException() : base("pop3 connection timeout exceeded") { }
    }

    public class ImapConnectionException : Exception
    {
        public ImapConnectionException(string message) : base(message) { }
    }

    public class ImapConnectionTimeoutException : ImapConnectionException
    {
        public ImapConnectionTimeoutException() : base("imap connection timeout exceeded") { }
    }

    public class SmtpConnectionException : Exception
    {
        public SmtpConnectionException(string message) : base(message) { }
    }

    public class SmtpConnectionTimeoutException : SmtpConnectionException
    {
        public SmtpConnectionTimeoutException() : base("smtp connection timeout exceeded") { }
    }

    public class ApiHelperException : Exception
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _response;

        public HttpStatusCode StatusCode {
            get { return _statusCode; }
        }

        public string Response
        {
            get { return _response; }
        }

        public ApiHelperException(string message, HttpStatusCode statusCode, string response)
            : base(message)
        {
            _statusCode = statusCode;
            _response = response;
        }

        public override string ToString()
        {
            return string.Format("Api error has been occurred: Message = '{0}' StatusCode = '{1}' Response = '{2}'",
                Message, StatusCode, Response);
        }
    }
}
