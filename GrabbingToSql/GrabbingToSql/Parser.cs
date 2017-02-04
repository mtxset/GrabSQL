using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GrabbingToSql
{
    class Parser
    {
        public StringBuilder GetHtmlFromCompany()
        {
            StringBuilder sb = new StringBuilder();

            HTTPConnector webConn = new HTTPConnector("https://beta.companieshouse.gov.uk/company/10581927");

            sb = webConn.GetLastPage;

            return sb;
        }

        private class HTTPConnector
        {
            private HttpWebRequest  httpRequest;
            private HttpWebResponse httpRespone;

            private StringBuilder lastPageContent;

            public StringBuilder GetLastPage
            {
                get
                {
                    return lastPageContent;
                }
            }

            private void Init()
            {
                lastPageContent = new StringBuilder();
            }

            public HTTPConnector( string initialRequest = "https://beta.companieshouse.gov.uk/company/")
            {
                Init();

                try
                { 
                    httpRequest = (HttpWebRequest)WebRequest.Create( initialRequest );
                    httpRequest.KeepAlive = false;
                    httpRespone = (HttpWebResponse)httpRequest.GetResponse();
                    
                    Stream streamResponse       = httpRespone.GetResponseStream();
                    StreamReader streamReader   = new StreamReader( streamResponse );

                    Char[] buffer = new char[256];

                    int count = streamReader.Read( buffer, 0, 256 );
                    while (count > 0)
                    {
                        String tempData = new string( buffer, 0, count );
                        lastPageContent.Append( tempData );
                        count = streamReader.Read( buffer, 0, 256 );
                    }
                }
                catch (ArgumentException e)
                {

                }
                catch (WebException e)
                {

                }
                catch (Exception e)
                {

                }
            }

            
            
        }


    }
}
