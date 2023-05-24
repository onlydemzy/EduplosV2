namespace Eduplus.Services.UtilityServices
{
    public class FetchGatewayInfo
    {
        public static void GetGatewayInfomation(out string responseUrl, out string logoUrl, out string notificationurl, out string terminalId, out string key)
        {
            string filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory.ToString(), "UrlPaths.txt");
            responseUrl = "";
            logoUrl = "";
            notificationurl = "";
            terminalId = "";
            key = "";
            var lines = System.IO.File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (line.Contains("ResponseUrl"))
                {

                    responseUrl = line.Replace("ResponseUrl=", "").Trim();
                }
                if (line.Contains("logoUrl"))
                {

                    logoUrl = line.Replace("logoUrl=", "").Trim();
                }
                if (line.Contains("confirmationUrl"))
                {

                    notificationurl = line.Replace("confirmationUrl=", "").Trim();
                }
                if (line.Contains("TerminalId"))
                {

                    terminalId = line.Replace("TerminalId=", "").Trim();
                }
                if (line.Contains("TerminalKEY"))
                {

                    key = line.Replace("TerminalKEY=", "").Trim();
                }
            }
        }
    }
}
