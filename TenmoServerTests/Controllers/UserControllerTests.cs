using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoServer.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TenmoServer.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Net.Http.Headers;

namespace TenmoServer.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests : TenmoServerTests.TenmoDaoTests
    {
        protected HttpClient _client;

        [TestInitialize]
        public override void Setup()
        {
            var builder = new WebHostBuilder()
                .UseStartup<TenmoServer.Startup>()
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build());
            var server = new TestServer(builder);
            _client = server.CreateClient();
            base.Setup();
        }
        [TestMethod()]
        public async Task AllMethodsExpectUnauthorized()
        {
            Account input = new Account() { AccountId = 0, UserId = 0, Balance = 1000};

            var responseGetDefaultAccount = await _client.GetAsync("user/1/default");
            var responseGetAccounts = await _client.GetAsync("user/1");
            var responseGetUsers = await _client.GetAsync("user");
            var responsePostCreateAccount = await _client.PostAsJsonAsync("user/1", input);

            Assert.IsTrue(responseGetDefaultAccount.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            Assert.IsTrue(responseGetAccounts.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            Assert.IsTrue(responseGetUsers.StatusCode == System.Net.HttpStatusCode.Unauthorized);
            Assert.IsTrue(responsePostCreateAccount.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public async Task GetAccountsExpectOk()
        {
            string userToken = await GetLogin();

            var requestViewer = new HttpRequestMessage() { RequestUri = new Uri(_client.BaseAddress + "user/1"), Method = HttpMethod.Get };
            requestViewer.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            var responseViewer = await _client.SendAsync(requestViewer);

            var responseGetAccounts1 = await _client.GetAsync("user/1");
            var responseGetAccounts2 = await _client.GetAsync("user/2");

            Assert.IsTrue(responseGetAccounts1.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.IsTrue(responseGetAccounts2.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

        //[TestMethod]
        //public async Task GetUsersExpectOk()
        //{
        //    string viewerToken = await GetLogin();

        //    var requestViewer = new HttpRequestMessage() { RequestUri = new Uri(_client.BaseAddress + "user/1"), Method = HttpMethod.Get };
        //    requestViewer.Headers.Authorization = new AuthenticationHeaderValue("Bearer", viewerToken);
        //    var responseViewer = await _client.SendAsync(requestViewer);

        //    var responseGetUsers = await _client.GetAsync("user");

        //    Assert.IsTrue(responseGetUsers.StatusCode == System.Net.HttpStatusCode.OK);
        //}

        //[TestMethod]
        //public async Task GetDefaultAccountExpectOk()
        //{
        //    string userToken = await GetLogin();

        //    var requestViewer = new HttpRequestMessage() { RequestUri = new Uri(_client.BaseAddress + "user/1"), Method = HttpMethod.Get };
        //    requestViewer.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        //    var responseViewer = await _client.SendAsync(requestViewer);

        //    var responseGetDefaultAccount1 = await _client.GetAsync("user/1/default");
        //    var responseGetDefaultAccount2 = await _client.GetAsync("user/2/default");

        //    Assert.IsTrue(responseGetDefaultAccount1.StatusCode == System.Net.HttpStatusCode.OK);
        //    Assert.IsTrue(responseGetDefaultAccount2.StatusCode == System.Net.HttpStatusCode.OK);
        //}

        private async Task<string> GetLogin()
        {
            var viewerResponse = await _client.PostAsJsonAsync("login", new { username = "test", password = "test" });

            string viewerResponseContent = await viewerResponse.Content.ReadAsStringAsync();
            ReturnUser viewer = JsonConvert.DeserializeObject<ReturnUser>(viewerResponseContent);
            return viewer.Token;
        }
    }
}