using IGConnect;
using Lightstreamer.DotNet.Client;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using IGService3.DTOs;
using System.Net;
using System.Collections;


namespace IGService3
{
    public class IGLogin
    {
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();
        public bool LoggedIn { get; set; }
  

        public async void Login(IgRestApiClient igRestApiClient, IGStreamingApiClient igStreamApiClient,string env, string userName, string password, string apiKey)
        {
            _Logger.Info("Attempting login");
            IGService3.Accounts = new ObservableCollection<AccountModel>();
            //    string env = "demo";
            //   string userName = "fdeambrosis";
            //   string password = "Be.25080";
            //   string apiKey = "ebb774096f7f20fe7e8c62aec66b165582d3e94e";
            _Logger.Info("User=" + userName + " is attempting to login to environment=" + env);

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(apiKey))
            {
                _Logger.Info("Please enter a valid username / password / ApiKey combination in ApplicationViewModel ( Login method )");
                return;
            }

            var ar = new AuthenticationRequest { identifier = userName, password = password, encryptedPassword = true };

            try
            {
                var response = await igRestApiClient.SecureAuthenticate(ar, apiKey);
                if (response && (response.Response != null) && (response.Response.accounts.Count > 0))
                {
                   IGService3.Accounts.Clear();

                    foreach (var account in response.Response.accounts)
                    {
                        var igAccount = new AccountModel
                        {
                            ClientId = response.Response.clientId,
                            ProfitLoss = response.Response.accountInfo.profitLoss,
                            AvailableCash = response.Response.accountInfo.available,
                            Deposit = response.Response.accountInfo.deposit,
                            Balance = response.Response.accountInfo.balance,
                            LsEndpoint = response.Response.lightstreamerEndpoint,
                            AccountId = account.accountId,
                            AccountName = account.accountName,
                            AccountType = account.accountType
                        };
                        IGService3.Accounts.Add(igAccount);
                    }
                    _Logger.Info("----------------------------------");
                    _Logger.Info("AccountId: " + response.Response.currentAccountId.ToString());
                    _Logger.Info("Available: " + response.Response.accountInfo.available.ToString());
                    _Logger.Info("Balance :" + response.Response.accountInfo.balance.ToString());
                    _Logger.Info("Deposit :" + response.Response.accountInfo.deposit.ToString());
                    _Logger.Info("ProfitLoss :" + response.Response.accountInfo.profitLoss.ToString());
                    _Logger.Info("Endpoint: " + response.Response.lightstreamerEndpoint);
                    _Logger.Info("----------------------------------");

                    LoggedIn = true;

                    _Logger.Info("Logged in, current account: " + response.Response.currentAccountId);

                    ConversationContext context = igRestApiClient.GetConversationContext();

                    _Logger.Info("establishing datastream connection");


                    if ((context != null) && (response.Response.lightstreamerEndpoint != null) &&
                        (context.apiKey != null) && (context.xSecurityToken != null) && (context.cst != null))
                    {
                        try
                        {
                            IGService3.CurrentAccountId = response.Response.currentAccountId;

                            var connectionEstablished =
                                igStreamApiClient.Connect(response.Response.currentAccountId,
                                                          context.cst,
                                                          context.xSecurityToken, context.apiKey,
                                                            response.Response.lightstreamerEndpoint);
                            if (connectionEstablished)
                            {
                                _Logger.Info(String.Format("Connecting to Lightstreamer. Endpoint ={0}",
                                                    response.Response.lightstreamerEndpoint));
                            }
                            else
                            {
                                igStreamApiClient = null;
                                _Logger.Info((String.Format("Could NOT connect to Lightstreamer. Endpoint ={0}", response.Response.lightstreamerEndpoint)));
                            }
                        }
                        catch (Exception ex)
                        {
                            _Logger.Info(ex.Message);
                        }
                    }
                }
                else
                {
                    _Logger.Info("Failed to login.HttpResponse StatusCode = " + response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                _Logger.Info("ApplicationViewModel exception : " + ex.Message);
            }
        }

    }
}
