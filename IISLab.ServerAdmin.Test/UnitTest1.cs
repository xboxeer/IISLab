using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IISLab.ServerAdmin.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetCurrentSiteTest()
        {
            var request = @"GET / HTTP/1.1
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
Accept-Language: zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3
Upgrade-Insecure-Requests: 1
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/18.17763
Accept-Encoding: gzip, deflate, peerdist
Host: localhost:81
Connection: Keep-Alive
X-P2P-PeerDist: Version=1.1
X-P2P-PeerDistEx: MinContentInformation=1.0, MaxContentInformation=2.0";
            var config = HostConfig.GetHostConfig();
            var site = config.GetRequestingSite(request);
            Assert.IsNotNull(site);
        }
    }
}
