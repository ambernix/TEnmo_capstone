using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44320/";
        private readonly IRestClient client = new RestClient();

        // returns account numbers and balances for all accounts for the user
        public IList<ApiAccount> GetAccounts(int userId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + $"user/{userId}");
            IRestResponse<IList<ApiAccount>> response = client.Get<IList<ApiAccount>>(request);
            return response.Data;
        }
        public int AddAccount(int userId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            int output = 0;
            RestRequest request = new RestRequest(API_BASE_URL + $"user/{userId}");
            IRestResponse<int> response = client.Post<int>(request);
            if (response.IsSuccessful)
            {
                output = response.Data;
            }
            return output;
        }

        // gives list of all users with their user ids except the one making the request
        public IList<ApiUser> GetUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + $"user");
            IRestResponse<IList<ApiUser>> response = client.Get<IList<ApiUser>>(request);
            return response.Data;
        }

        // handles transfers that are both sending money and approving a request for money
        public int Transfer(int accountId, int userId, decimal transferAmount, string transferType)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            // Get Id for default account of given userId. This allows one user to transfer to another users
            // account without exposing the account id number.
            RestRequest accountIdRequest = new RestRequest(API_BASE_URL + $"user/{userId}/default");
            IRestResponse<int> accountResponse = client.Get<int>(accountIdRequest);

            int toAccountId, fromAccountId;
            if (transferType == "Send")
            {
                toAccountId = accountResponse.Data;
                fromAccountId = accountId;
            }
            else
            {
                toAccountId = accountId;
                fromAccountId = accountResponse.Data;
            }
            ApiTransfer transfer = new ApiTransfer
            {
                AccountFrom = fromAccountId,
                AccountTo = toAccountId,
                Amount = transferAmount,
                TransferType = transferType
            };
            RestRequest transferRequest = new RestRequest(API_BASE_URL + $"transfer/{UserService.GetUsername()}");
            transferRequest.AddJsonBody(transfer);
            IRestResponse<int> transferResponse = client.Post<int>(transferRequest);

            return transferResponse.Data;

        }

        // returns all transfers (approved or pending depending on input) for all accounts for the user
        public IList<ApiTransfer> GetTransfers(string username, string pending = "")
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + $"transfer/{username}/{pending}");
            IRestResponse<IList<ApiTransfer>> response = client.Get<IList<ApiTransfer>>(request);
            return response.Data;
        }

        // returns details on an individual transfer for the user
        public ApiTransfer GetTransfer(string username, int transferId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            RestRequest request = new RestRequest(API_BASE_URL + $"transfer/{username}/{transferId}");
            IRestResponse<ApiTransfer> response = client.Get<ApiTransfer>(request);
            ApiTransfer output = response.Data;

            return output;
        }
    
        // depending on user input, approves or rejects a pending transfer
        public bool ActionTransfer(ApiTransfer transfer, int actionId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            transfer.UsernameFrom = UserService.GetUsername();
            RestRequest request = new RestRequest(API_BASE_URL + $"transfer/{transfer.UsernameFrom}/{actionId}");
            request.AddJsonBody(transfer);
            IRestResponse response = client.Put(request);
            if (response.IsSuccessful)
            {
                return true;
            }
            return false;
        }
    }
}