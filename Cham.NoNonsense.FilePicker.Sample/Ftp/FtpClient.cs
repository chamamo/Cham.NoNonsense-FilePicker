//
// Copyright (c) 2015 Mourad Chama
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Cham.NoNonsense.FilePicker.Sample.Ftp
{
    public class FtpClient
	{
        private static readonly string[] ParseFormats = new string[] { 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<dir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})(\\s+)(?<size>(\\d+))(\\s+)(?<ctbit>(\\w+\\s\\w+))(\\s+)(?<size2>(\\d+))\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{2}:\\d{2})\\s+(?<name>.+)", 
            "(?<timestamp>\\d{2}\\-\\d{2}\\-\\d{2}\\s+\\d{2}:\\d{2}[Aa|Pp][mM])\\s+(?<dir>\\<\\w+\\>){0,1}(?<size>\\d+){0,1}\\s+(?<name>.+)" };


		public FtpClient()
		{
		    Proxy = null;
		    KeepAlive = false;
		    EnableSsl = false;
		}

		public FtpClient(string hostname)
        {
            Proxy = null;
            KeepAlive = false;
            EnableSsl = false;
            _hostname = hostname;
        }

		public FtpClient(string hostname, string username, string password)
		{
            Proxy = null;
            KeepAlive = false;
            EnableSsl = false;
            _hostname = hostname;
			_username = username;
			Password = password;
		}

		public FtpClient(string hostname, string username, string password, bool KeepAlive)
		{
		    Proxy = null;
		    EnableSsl = false;
		    _hostname = hostname;
			_username = username;
			Password = password;
			this.KeepAlive = KeepAlive;
		}
        
		public IEnumerable<FtpFile> ListFiles(FtpFile parent = null)
		{
		    var parentPath = parent == null ? "" : parent.Path;
			var ftp = GetRequest(GetDirectory(parent.Path));
			// Set request to do simple list
			ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails;

			string str = GetStringResponse(ftp);
			// replace CRLF to CR, remove last instance
			str = str.Replace("\r\n", "\r").TrimEnd('\r');

		    foreach (var line in str.Replace("\n", "").Split(System.Convert.ToChar('\r')))
		    {
		        //parse
		        if (line != "")
		        {
		            var m = GetMatchingRegex(line);
		            if (m == null)
		            {
		                //failed
		                throw (new ApplicationException("Unable to parse line: " + line));
		            }
		            else
		            {
		                var filename = m.Groups["name"].Value;
		                var path = parentPath + filename;
		                var dir = m.Groups["dir"].Value;
		                FtpFile file;
		                if (dir != "" && dir != "-")
		                {
		                    file = new FtpDir(parentPath, filename);
		                }
		                else
		                {
		                    file = new FtpFile(parentPath, filename);
		                }
		                yield return file;
		            }
		        }
		    }
		}

        private Match GetMatchingRegex(string line)
		{
			Regex rx;
			Match m;
			for (int i = 0; i <= ParseFormats.Length - 1; i++)
			{
				rx = new Regex(ParseFormats[i]);
				m = rx.Match(line);
				if (m.Success)
				{
					return m;
				}
			}
			return null;
		}

        public string Hostname
        {
            get
            {
                if (_hostname.StartsWith("ftp://"))
                {
                    return _hostname;
                }
                else
                {
                    return "ftp://" + _hostname;
                }
            }
            set { _hostname = value; }
        }
        private string _hostname;

        public string Username
        {
            get { return (_username == "" ? "anonymous" : _username); }
            set { _username = value; }
        }
        private string _username;

        public string Password { get; set; }

        public string CurrentDirectory
        {
            get
            {
                return _currentDirectory + ((_currentDirectory.EndsWith("/")) ? "" : "/").ToString();
            }
            set
            {
                if (!value.StartsWith("/"))
                {
                    throw (new ApplicationException("Directory should start with /"));
                }
                _currentDirectory = value;
            }
        }
        private string _currentDirectory = "/";

        public bool EnableSsl { get; set; }

        public bool KeepAlive { get; set; }

        public bool UsePassive { get; set; }

        public IWebProxy Proxy { get; set; }

		private FtpWebRequest GetRequest(string URI)
		{
			//create request
			FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
			//Set the login details
			result.Credentials = GetCredentials();
			// support for EnableSSL
			result.EnableSsl = EnableSsl;
			//keep alive? (stateless mode)
			result.KeepAlive = KeepAlive;
			// support for passive connections 
			result.UsePassive = UsePassive;
			// support for proxy settings
			result.Proxy = Proxy;

			return result;
		}

		private System.Net.ICredentials GetCredentials()
		{
			return new System.Net.NetworkCredential(Username, Password);
		}

		private string GetFullPath(string file)
		{
			if (file.Contains("/"))
			{
				return AdjustDir(file);
			}
			else
			{
				return this.CurrentDirectory + file;
			}
		}

		private string AdjustDir(string path)
		{
			return ((path.StartsWith("/")) ? "" : "/").ToString() + path;
		}

		private string GetDirectory(string directory)
		{
			string URI;
			if (directory == "")
			{
				//build from current
				URI = Hostname + this.CurrentDirectory;
				_lastDirectory = this.CurrentDirectory;
			}
			else
			{
				if (!directory.StartsWith("/"))
				{
					throw (new ApplicationException("Directory should start with /"));
				}
				URI = this.Hostname + directory;
				_lastDirectory = directory;
			}
			return URI;
		}

		private string _lastDirectory = "";

		private string GetStringResponse(FtpWebRequest ftp)
		{
			//Get the result, streaming to a string
			string result = "";
			using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
			{
				long size = response.ContentLength;
				using (Stream datastream = response.GetResponseStream())
				{
					using (StreamReader sr = new StreamReader(datastream, System.Text.Encoding.UTF8 ))
					{
						result = sr.ReadToEnd();
						sr.Close();
					}

					datastream.Close();
				}

				response.Close();
			}

			return result;
		}

	}
}