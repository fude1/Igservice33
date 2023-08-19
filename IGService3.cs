﻿using IGConnect;
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
using YamlDotNet.Serialization;
using IGConfig;
using dto.endpoint.prices.v2;
using IGWebApi.DTOs;

namespace IGService3
{
    public partial class IGService3 : ServiceBase
    {
        public class OpenPositionData
        {
            public decimal? gainLost { get; set; }
            public string createdDate { get; set; }
            public string dealId { get; set; }
            public decimal? size { get; set; }
            public string direction { get; set; }
            public decimal? level { get; set; }
        }

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();
    	private ChartCandleTableListerner _chartSubscriptionOneMin;
        private SubscribedTableKey _chartSubscribedTableKeyOneMin;
        private bool CandleOneMinDetailsSubscribed = false;
        private SubscribedTableKey _browseSubscriptionTableKey;
        private MarketDetailsTableListerner _l1BrowsePricesSubscription;
        private DateTime lastTime = DateTime.Now;
        private bool MarketDetailsSubscribed = false;
        private bool AccountDetailsSubscribed = false;
        private AccountDetailsTableListerner _accountBalanceSubscription;
        private SubscribedTableKey _accountBalanceStk;
        private CTimerSW _timerStartConnession = new CTimerSW();
        private CTimerSW _timerStartConnession1 = new CTimerSW();
        private CTimerSW _stockUpdate = new CTimerSW();
        private CTimerSW _realdataUpdate = new CTimerSW();
        private CTimerSW _ChartCandelDataTimer = new CTimerSW();
        
        private bool bFirstTime = true;
        private bool bFirstTime1 = true;
        private Thread _workerThreadClientData;
        private Thread _workerThreadClientData1;
        private bool _bContinueClientData = true;
        private bool _bContinueClientData1 = true;
        public IGStreamingApiClient igStreamApiClient;
        public IgRestApiClient igRestApiClient;
        private TelegramMsg TMsg = new TelegramMsg();
        public static string CurrentAccountId = "";
        public static ObservableCollection<AccountModel> Accounts { get; set; }
        private IGLogin _IGLogin = new IGLogin();
        private Datalogger _Datalogger = new Datalogger();
        private ChartCandelData _ChartCandelData = new ChartCandelData();
        private bool _bChartCandelData = false;
        public StreamingAccountData lastAccount = new StreamingAccountData();
        private int _bTotaleOperazioniAttive = 0;
        private ChartCandelData _realCandelData = new ChartCandelData();
        private string _foglio = "Foglio1";
        private double _size = 0;
        decimal? _gainLost = 0;
        private bool _bStopRunning = false;
        private bool _bFirstRunning = true;
        private double spreadPrice = 0;
        private int actualAccount = 0;
        MyConfig configYaml = new MyConfig();
        int NextPriceByDate = 0;
        string apiTokentrading = "";
        string chatIdtrading = "";
        decimal? size = 0;
        decimal? stopLevel = 0;
        decimal? limitLevel = 0;
        string currencyCode = "";
        string epic = "";
        double volume;
        int FattoreMoltiplicatore = 0;

        string ApiTokenMonitor = "6294115371:AAEtz_twC2-VF0NR9QtGpbKlphmBS-EE0OM";
        string ChatIdMonitor= "@aicodemonitor";
        //--------------------------------------------------------------------------------------------
        //COSTRUTTORE
        //--------------------------------------------------------------------------------------------
        public IGService3()
        {
            _Logger.Info("Avvio");
            InitializeComponent();
            _timerStartConnession.Start(60000);
            _timerStartConnession1.Start(60000);
            _stockUpdate.Start(5000);
            _Logger.Info("_timerStartConnession avviato");
            RegisterPricesSubscription();
     		RegisterCandleOneMinSubscriptions();
            RegisterAccountBalanceSubscriptions();
            _bStopRunning = false;
            var deserializer = new DeserializerBuilder().Build();
            _Logger.Info("leggo file configYaml");
            using (var reader = new StreamReader(@"C:\inetpub\IGService3\config.yaml"))
            {
                _Logger.Info("deserializer configYaml");
                configYaml = deserializer.Deserialize<MyConfig>(reader);
            }
            size = Decimal.Parse(configYaml.size);
            stopLevel = Decimal.Parse(configYaml.stopLevel);
            limitLevel = Decimal.Parse(configYaml.limitLevel);
            epic = configYaml.Epic;
            currencyCode = configYaml.currencyCode;
            apiTokentrading = configYaml.ApiTokenTrading;
            chatIdtrading = configYaml.ChatIdTrading;
            FattoreMoltiplicatore = int.Parse(configYaml.FattoreMoltiplicatore);
            ServiceName = "IGService3";
            _Logger.Info("Avvio servizio raccolta dati storici ");
            TMsg.sendTelegramMessage("Avvio servizio raccolta dati storici");
        }

        protected override void OnStart(string[] args)
        {
            _Logger.Info("OnStart");
            _workerThreadClientData = new Thread(this.ExecuteThreadClientData);
            _workerThreadClientData.Start();
            _workerThreadClientData1 = new Thread(this.ExecuteThreadClientData1);
            _workerThreadClientData1.Start();
        }

        private void RegisterPricesSubscription()
        {
            _Logger.Info("RegisterPricesSubscription");
            _l1BrowsePricesSubscription = new MarketDetailsTableListerner();
            _l1BrowsePricesSubscription.Update += OnMarketUpdate;
        }

        private void RegisterAccountBalanceSubscriptions()
        {
            _Logger.Info("RegisterAccountBalanceSubscriptions");
            _accountBalanceSubscription = new AccountDetailsTableListerner();
            _accountBalanceSubscription.Update += OnAccountUpdate;
        }

        private void RegisterCandleOneMinSubscriptions()
        {
            _chartSubscriptionOneMin = new ChartCandleTableListerner();
            _chartSubscriptionOneMin.Update += OnChartCandleDataUpdateOneMin;
        }


        private void SubscribeToChartsOneMin(string[] chartEpics, ChartScale scaleChart)
        {
            try
            {
                _chartSubscribedTableKeyOneMin = igStreamApiClient.SubscribeToChartCandleData(chartEpics, scaleChart, _chartSubscriptionOneMin);
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception when trying to subscribe to Chart Candle Data: " + chartEpics[0] + ex.Message);
            }
        }

        public void SubscribeToBrowsePrices(string[] epics)
        {
          //  _Logger.Info("SubscribeToBrowsePrices");
            try
            {
                _browseSubscriptionTableKey = igStreamApiClient.SubscribeToMarketDetails(epics, _l1BrowsePricesSubscription);
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception SubscribeToBrowsePrices: " + ex.Message);
            }
        }

        public void SubscribeToAccountDetails()
        {
         //   _Logger.Info("SubscribeToAccountDetails");
            try
            {
                if (CurrentAccountId != null)
                {
                    _accountBalanceStk = igStreamApiClient.SubscribeToAccountDetails(CurrentAccountId, _accountBalanceSubscription);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - SubscribeToAccountDetails" + ex.Message);
            }

        }

        private void OnMarketUpdate(object sender, UpdateArgs<L1LsPriceData> e)
        {
          //  _Logger.Info("OnMarketUpdate");
            try
            {
                var wlmUpdate = e.UpdateData;
                if (e.ItemPosition == 1)
                {
                }
                if (e.ItemPosition == 2)
                {
                }

            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - OnMarketUpdate" + ex.Message);
            }
        }

        private void OnAccountUpdate(object sender, UpdateArgs<StreamingAccountData> e)
        {
         //   _Logger.Info("OnAccountUpdate");
            if ((e.ItemPosition == 0) || (e.ItemPosition > Accounts.Count))
            {
                return;
            }
            var index = e.ItemPosition - 1; // we are subscription to the current account ( which will be account index 0 ).
            lastAccount = e.UpdateData;
            //_bUltimoAccount = true;
        }

    
      
        private void OnChartCandleDataUpdateOneMin(object sender, UpdateArgs<ChartCandelData> e)
        {
             var candleUpdate = e.UpdateData;

     
            if (candleUpdate.EndOfConsolidation.ToString() == "True")
            {
                if (e.ItemPosition == 1 && candleUpdate.Offer.Close != null)
                {
                     _ChartCandelData = candleUpdate;
                     _bChartCandelData = true;
                }
            }
            if (e.ItemPosition == 1 && candleUpdate.Offer.Close != null)
            {
                _realCandelData = candleUpdate;
            }

        }

        protected override void OnStop()
        {
            _Logger.Info("OnStop");
            _bContinueClientData = false;
        }
        //--------------------------------------------------------------------------------------------
        //Eseguo fino allo STOP
        //--------------------------------------------------------------------------------------------

        private void ExecuteThreadClientData1()
        {
            double priceask = 0;
            double pricebid = 0;
            double _volume = 0;
            while (_bContinueClientData1)
            {
                if (_timerStartConnession1.TimerScaduto() && bFirstTime1)
                {
                    bFirstTime1 = false;
                }
                if (!bFirstTime1 && !CandleOneMinDetailsSubscribed)
                {
                    CandleOneMinDetailsSubscribed = true;

                    SubscribeToChartsOneMin(new String[] { configYaml.Epic }, ChartScale.OneMinute);
                }

                if ( !bFirstTime1 && ( _ChartCandelDataTimer.TimerScaduto() || _bChartCandelData) )
                {
                    _ChartCandelDataTimer.Start(90000);

                    if(_bChartCandelData == false)
                    {
                        CandleOneMinDetailsSubscribed = false;
                        TMsg.sendTelegramMessage("Flusso dati interrotto!!!");
                    }

                    _bChartCandelData = false;
                    if (_realCandelData.Offer != null && _realCandelData.Offer.Close != null && _realCandelData.Bid != null && _realCandelData.Bid.Close != null && _realCandelData.LastTradedVolume != null)
                    {
                        _Datalogger.UpdateDataEURUSD(DateTime.Now, (decimal)_ChartCandelData.Offer.Open, (decimal)_ChartCandelData.Offer.Close, (decimal)_ChartCandelData.Offer.High, (decimal)_ChartCandelData.Offer.Low, (decimal)_ChartCandelData.LastTradedVolume);
                        WriteLucaCandle((decimal)_ChartCandelData.Offer.Open, (decimal)_ChartCandelData.Offer.Close, (decimal)_ChartCandelData.Offer.High, (decimal)_ChartCandelData.Offer.Low);
                    }
                    else
                    {
                        _Datalogger.getLastDataEURUSD(out priceask, out pricebid, out _volume);
                        _Datalogger.UpdateDataEURUSD(DateTime.Now, (decimal)priceask, (decimal)priceask, (decimal)priceask, (decimal)priceask, (decimal)priceask);
                        WriteLucaCandle((decimal)priceask, (decimal)priceask, (decimal)priceask, (decimal)priceask);
                     }

                }
                Thread.Sleep(5000);
            }
        }
        //--------------------------------------------------------------------------------------------
        //Eseguo fino allo STOP
        //--------------------------------------------------------------------------------------------
        private void ExecuteThreadClientData()
          {
              _Logger.Info("ExecuteThreadClientData");
              while (_bContinueClientData)
              {
                    if (!bFirstTime && _realdataUpdate.TimerScaduto() )
                    {
                        _realdataUpdate.Start(1000);
                    
                        int gapLong = 0;
                        int gapShort = 0;
                        if (_realCandelData.Offer != null && _realCandelData.Offer.Close != null && _realCandelData.Bid != null && _realCandelData.Bid.Close != null && _realCandelData.LastTradedVolume != null)
                        {
                            if (_realCandelData.Offer.Open > _realCandelData.Offer.High)
                                gapLong = 1;
                            if (_realCandelData.Offer.Open < _realCandelData.Offer.Low)
                                gapShort = 1;
                            _Datalogger.UpdateLastEURUSD(DateTime.Now, (decimal)_realCandelData.Offer.Close, (decimal)_realCandelData.Bid.Close, (decimal)_realCandelData.LastTradedVolume, gapLong, gapShort, (decimal)_realCandelData.Offer.Open, (decimal)_realCandelData.Offer.Close, (decimal)_realCandelData.Offer.High, (decimal)_realCandelData.Offer.Low);
                        }
                     
                        string path1 = configYaml.Monitor;
                        System.IO.StreamWriter sw1 = new System.IO.StreamWriter(path1, true);
                        string date = DateTime.Now.ToString("dd-MMM-yy HH:mm");
                        sw1.Write(date + "\r\n");
                        sw1.Close();
	                  }

                    if (_timerStartConnession.TimerScaduto() && bFirstTime)
                    {
                        bFirstTime = false;
                        igRestApiClient = new IgRestApiClient(configYaml.env);
                        igStreamApiClient = new IGStreamingApiClient();
                        _IGLogin.Login(igRestApiClient, igStreamApiClient, configYaml.env, configYaml.userName, configYaml.password, configYaml.apiKey);
                    }

                  if (!bFirstTime && !MarketDetailsSubscribed)
                  {
                     MarketDetailsSubscribed = true;
                     SubscribeToBrowsePrices(new String[] { configYaml.Epic });
                  }
                  if (!bFirstTime && !AccountDetailsSubscribed)
                  {
                      AccountDetailsSubscribed = true;
                      SubscribeToAccountDetails(); 
                  }
             
                  if (!bFirstTime && _stockUpdate.TimerScaduto())
                  {
                    try
                    {
                        string fromDate = DateTime.Now.AddDays(-1).ToString("yyyy.MM.dd");
                        string toDate = DateTime.Now.ToString("yyyy.MM.dd");

                        if ((DateTime.Now.Hour >= 1 && DateTime.Now.Hour <= 4) && (NextPriceByDate==1|| NextPriceByDate == 0))
                        {
                            GetHistoricalPriceByDate(configYaml.Epic, "HOUR_4");
                            NextPriceByDate = 5;
                        }
                        if ((DateTime.Now.Hour >= 5 && DateTime.Now.Hour <= 8) && (NextPriceByDate == 5 || NextPriceByDate == 0))
                        {
                            GetHistoricalPriceByDate(configYaml.Epic, "HOUR_4");
                            NextPriceByDate = 9;

                        }
                        if ((DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 12) && (NextPriceByDate == 9 || NextPriceByDate == 0))
                        {
                            GetHistoricalPriceByDate(configYaml.Epic, "HOUR_4");
                            NextPriceByDate = 13;


                        }
                        if ((DateTime.Now.Hour >= 13 && DateTime.Now.Hour <= 16) && (NextPriceByDate == 13 || NextPriceByDate == 0))
                        {
                            GetHistoricalPriceByDate(configYaml.Epic, "HOUR_4");
                            NextPriceByDate = 17;


                        }
                        if ((DateTime.Now.Hour >= 17 && DateTime.Now.Hour <= 20) && (NextPriceByDate == 17 || NextPriceByDate == 0))
                        {
                            GetHistoricalPriceByDate(configYaml.Epic, "HOUR_4");
                            NextPriceByDate = 21;

                        }
                        if ((DateTime.Now.Hour >= 21 && DateTime.Now.Hour <= 24) && (NextPriceByDate == 21 || NextPriceByDate == 0))
                        {
                            GetHistoricalPriceByDate(configYaml.Epic, "HOUR_4");
                            NextPriceByDate = 1;
                        }


                        if (actualAccount!=(int)(lastAccount.AvailableCash+ lastAccount.Deposit))
                        {
                            actualAccount = (int)(lastAccount.AvailableCash + lastAccount.Deposit);
							string apiTokentrading = configYaml.ApiTokenTrading;
                            string chatIdtrading = configYaml.ChatIdTrading;

                            TMsg.sendTelegramMessage("actualAccount 3:" + actualAccount.ToString()+ " ProfitAndLoss:" + lastAccount.ProfitAndLoss.ToString()+ " UsedMargin:" + lastAccount.UsedMargin.ToString(), ChatIdMonitor, ApiTokenMonitor);
                            _Datalogger.UpdateAccount(lastAccount);
                        }
                        if (lastAccount.ProfitAndLoss<0)
                            TMsg.sendTelegramMessage( " ProfitAndLoss:" + lastAccount.ProfitAndLoss.ToString() );
                    }
                    catch (Exception ex)
                    {
                        _Logger.Error("Exception - UpdateAccount" + ex.Message);
                    }
   
                    GetRestPositions(configYaml.Epic);
                    lastTransactionUpdate();
               

                    MarketDetailsSubscribed = false;
                   
                    // se volumi alti stoppo i servizi
                    AccountDetailsSubscribed = false;
                     if (_realCandelData.Offer != null)
                        spreadPrice = ((double)_realCandelData.Offer.Close-(double)_realCandelData.Bid.Close)* double.Parse(configYaml.FattoreMoltiplicatore);

                    if ( !_bStopRunning  &&
                          ( (DateTime.Now.Hour < double.Parse(configYaml.OraInizioGiornata) || DateTime.Now.Hour > double.Parse(configYaml.OraFineGiornata)) ||
                         //   (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ||
                            (spreadPrice > double.Parse(configYaml.SpreadSoglia) || _Datalogger.getVolume() > double.Parse(configYaml.VolumeSoglia)) ) )
                    {
                        _bFirstRunning = false;
                        _bStopRunning = true;
                        _Datalogger.updateActivePeriod(0, _Datalogger.getVolume(), spreadPrice);
                        string path2 = configYaml.Stopmonitor;
                        System.IO.StreamWriter sw2 = new System.IO.StreamWriter(path2, true);
                        string date = DateTime.Now.ToString("dd-MMM-yy HH:mm");
                        sw2.Write(date + "\r\n");
                        sw2.Close();
                        TMsg.sendTelegramMessage("Stop All Service 3 Volume:" + _Datalogger.getVolume() + " spreadPrice:" + spreadPrice);
                        if(_Datalogger.getVolume() > double.Parse(configYaml.VolumeSoglia))
                        {
                          //  CloseAllPosition("CS.D.EURUSD.MINI.IP", "BUY", "Volume() > 100");
                          //  CloseAllPosition("CS.D.EURUSD.MINI.IP", "SELL", "Volume() > 100");
                        }
                        if (DateTime.Now.Hour > double.Parse(configYaml.OraFineGiornata) && lastAccount.ProfitAndLoss>0)
                        {
                           // CloseAllPosition("CS.D.EURUSD.MINI.IP", "BUY", "Hour > 21");
                           // CloseAllPosition("CS.D.EURUSD.MINI.IP", "SELL", "Hour > 21");
                        }
                        if (DateTime.Now.Hour > double.Parse(configYaml.OraFineGiornata) && DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                        {
                         TMsg.sendTelegramMessage("FINE SETTIMANA!!!:" + actualAccount.ToString() + " ProfitAndLoss:" + lastAccount.ProfitAndLoss.ToString() + " UsedMargin:" + lastAccount.UsedMargin.ToString());
                        }
                        if (DateTime.Now.Hour > double.Parse(configYaml.OraFineGiornata) )
                        {
                           TMsg.sendTelegramMessage("FINE GIORNATA!!!:" + actualAccount.ToString() + " ProfitAndLoss:" + lastAccount.ProfitAndLoss.ToString() + " UsedMargin:" + lastAccount.UsedMargin.ToString());
                        }
                    }
                    else if ( (_bStopRunning || _bFirstRunning) &&
                        ( (DateTime.Now.Hour >= double.Parse(configYaml.OraInizioGiornata) && DateTime.Now.Hour <= double.Parse(configYaml.OraFineGiornata)) &&
                   //       !(DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) &&
                          (spreadPrice <= double.Parse(configYaml.SpreadSoglia) && _Datalogger.getVolume() <= double.Parse(configYaml.VolumeSoglia))) )
                    {
                        _bFirstRunning = false;
                        string path4 = configYaml.Stopmonitor;
                
                        _Datalogger.updateActivePeriod(1, _Datalogger.getVolume(), spreadPrice);
                        _bStopRunning = false;
                        File.Delete(path4);
                        TMsg.sendTelegramMessage("Start All Service Volume 3:" + _Datalogger.getVolume() + " spreadPrice:" + spreadPrice);
                        if (DateTime.Now.Hour == double.Parse(configYaml.OraInizioGiornata) && DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                        {
                           TMsg.sendTelegramMessage("INIZIO SETTIMANA!!!:" + actualAccount.ToString() + " ProfitAndLoss:" + lastAccount.ProfitAndLoss.ToString() + " UsedMargin:" + lastAccount.UsedMargin.ToString());
                        }
                        if (DateTime.Now.Hour == double.Parse(configYaml.OraInizioGiornata))
                        {
                           TMsg.sendTelegramMessage("INIZIO GIORNATA!!!:" + actualAccount.ToString() + " ProfitAndLoss:" + lastAccount.ProfitAndLoss.ToString() + " UsedMargin:" + lastAccount.UsedMargin.ToString());
                        }
                    }


                    List<string> epic = new List<string>();
                    List<string> direction = new List<string>();
                    List<double> size = new List<double>();
                    List<double> gainLost = new List<double>();
                    List<string> dealId = new List<string>();
                    List<double> price = new List<double>();
                    List<string> data = null;
                    List<double> s3 = null;
                    List<double> s2 = null;
                    List<double> s1 = null;
                    List<double> Pivot = null;
                    List<double> r3 = null;
                    List<double> r2 = null;
                    List<double> r1 = null;
                    double pAsk = 0;
                    double volume = 0;
                    double volumeM = _Datalogger.getVolume();
                    double rsi = 0;
                    double slope = 0;
                    BackAnalysis backAnalysis = new BackAnalysis();

                    _Datalogger.GetStock(out epic, out size, out direction, out gainLost, out dealId, out price);
                    for(int i=0;i<epic.Count;i++)
                    {
                        pAsk = 0;
                        volume = 0;
                        if (_realCandelData.Offer!=null)
                            pAsk =(double) _realCandelData.Offer.Close;
                        if (_ChartCandelData.LastTradedVolume != null)
                            volume = (double)_ChartCandelData.LastTradedVolume;
                        rsi = 0;
                        slope = 0;
                        _Datalogger.getRsiEURUSD(out rsi);
                        _Datalogger.getSlopeEURUSD(out slope);
                        _Datalogger.GetPivot(out data, out s3, out s2, out s1, out Pivot, out r3, out r2, out r1);
                        backAnalysis = new BackAnalysis(); 
                        backAnalysis.Size = size[i];
                        backAnalysis.foglio = dealId[i];
                        backAnalysis.ValT = pAsk;
                        backAnalysis.Bo = 0;
                        backAnalysis.So = 0;
                        backAnalysis.Tempo = ((DateTime.Now.Hour * 60) + DateTime.Now.Minute);
                        if (s3.Count > 0)
                        {
                            backAnalysis.S3 = s3[0];
                            backAnalysis.S2 = s2[0];
                            backAnalysis.S1 = s1[0];
                            backAnalysis.PV = Pivot[0];
                            backAnalysis.R1 = r3[0];
                            backAnalysis.R2 = r2[0];
                            backAnalysis.R3 = r1[0];
                        }
                        backAnalysis.Slop = slope;
                        backAnalysis.RSI = rsi;
                        backAnalysis.VolV = volume;
                        backAnalysis.VolM = volumeM;
                        backAnalysis.GainPerc = 0;
                        backAnalysis.Gain = gainLost[i];
                        backAnalysis.StartO = 0;
                        backAnalysis.StopO = 0;
                        backAnalysis.Spread = spreadPrice;
                        _Datalogger.UpdateIGBackAnalysis(backAnalysis);
                    }
                    pAsk = 0;
                    volume = 0;
                    if (_realCandelData.Offer != null)
                        pAsk = (double)_realCandelData.Offer.Close;
                    if (_ChartCandelData.LastTradedVolume != null)
                        volume = (double)_ChartCandelData.LastTradedVolume;
                    rsi = 0;
                    slope = 0;
                    _Datalogger.getRsiEURUSD(out rsi);
                    _Datalogger.getSlopeEURUSD(out slope);
                    _Datalogger.GetPivot(out data, out s3, out s2, out s1, out Pivot, out r3, out r2, out r1);
                    backAnalysis = new BackAnalysis();
                    backAnalysis.Size = _size;
                    backAnalysis.foglio = "Foglio1"; 
                    backAnalysis.ValT = pAsk;
                    backAnalysis.Bo = 0;
                    backAnalysis.So = 0;
                    backAnalysis.Tempo = ((DateTime.Now.Hour * 60) + DateTime.Now.Minute);
                    if (s3.Count > 0)
                    {
                        backAnalysis.S3 = s3[0];
                        backAnalysis.S2 = s2[0];
                        backAnalysis.S1 = s1[0];
                        backAnalysis.PV = Pivot[0];
                        backAnalysis.R1 = r3[0];
                        backAnalysis.R2 = r2[0];
                        backAnalysis.R3 = r1[0];
                    }
                    backAnalysis.Slop = slope;
                    backAnalysis.RSI = rsi;
                    backAnalysis.VolV = volume;
                    backAnalysis.VolM = volumeM;
                    backAnalysis.GainPerc = 0;
                    backAnalysis.Gain = (double)_gainLost;
                    backAnalysis.StartO = 0;
                    backAnalysis.StopO = 0;
                    backAnalysis.Spread = spreadPrice;
                    _Datalogger.UpdateIGBackAnalysis(backAnalysis);
     
                    _stockUpdate.Start(60000);
                  }

     

                if (!bFirstTime && _bStopRunning == false)
                {
                    ManageOpenPosition();
                    ManageCloseAllPosition();
                    ManageClosePosition();
                    ManageModifyPosition();
                }
                else if (!bFirstTime && _bStopRunning == true)
                {
                    ManageModifyPosition();
                    string fileName = configYaml.OpenPosition;
                    File.Delete(fileName);
                }

                Thread.Sleep(5000);
              }
           }


          private async void CloseAllPosition(string epic)
          {
              var positionsResponse = await igRestApiClient.getOTCOpenPositionsV2();
              if (positionsResponse && (positionsResponse.Response != null))
              {
                  if (positionsResponse.Response.positions.Count != 0)
                  {
                      foreach (var position in positionsResponse.Response.positions)
                      {
                        if( position.market.epic== epic)
                        {
                            closePosition(position.position.size, epic, position.position.direction, position.position.dealId, "");
                            decimal? gainLost = 0;
                            double BC = 0;
                            double SC = 0;
                            if (position.market.offer != null)
                            {
                                if (position.position.direction == "BUY")
                                {
                                    BC = 1; 
                                    gainLost = (position.position.size * position.market.bid) - (position.position.size * position.position.level);
                                }
                                else
                                {
                                    SC = 1;
                                    gainLost = (position.position.size * position.position.level) - (position.position.size * position.market.offer);
                                }
                            }
                            gainLost = gainLost * Decimal.Parse(configYaml.FattoreMoltiplicatore);

                            _gainLost = gainLost;
                            _size =(double) position.position.size;
                            
                             _foglio = position.position.dealId;
                            List<string> data = null;
                            List<double> s3 = null;
                            List<double> s2 = null;
                            List<double> s1 = null;
                            List<double> Pivot = null;
                            List<double> r3 = null;
                            List<double> r2 = null;
                            List<double> r1 = null;
                            double pAsk = 0;
                            double volume = 0;
                            double volumeM = _Datalogger.getVolume();
                            if (_realCandelData.Offer != null)
                                pAsk = (double)_realCandelData.Offer.Close;
                            if (_ChartCandelData.LastTradedVolume != null)
                                volume = (double)_ChartCandelData.LastTradedVolume;
                             double rsi = 0;
                            double slope = 0;
                            _Datalogger.getRsiEURUSD(out rsi);
                            _Datalogger.getSlopeEURUSD(out slope);
                            _Datalogger.GetPivot(out data, out s3, out s2, out s1, out Pivot, out r3, out r2, out r1);
                            BackAnalysis backAnalysis = new BackAnalysis();
                            backAnalysis.Size = _size;
                            backAnalysis.foglio = _foglio;
                            backAnalysis.ValT = pAsk;
                            backAnalysis.BC = BC;
                            backAnalysis.SC = SC;
                            backAnalysis.Bo = 0;
                            backAnalysis.So = 0;
                            backAnalysis.Tempo = ((DateTime.Now.Hour * 60) + DateTime.Now.Minute);
                            if (s3.Count > 0)
                            {
                                backAnalysis.S3 = s3[0];
                                backAnalysis.S2 = s2[0];
                                backAnalysis.S1 = s1[0];
                                backAnalysis.PV = Pivot[0];
                                backAnalysis.R1 = r3[0];
                                backAnalysis.R2 = r2[0];
                                backAnalysis.R3 = r1[0];
                            }
                            backAnalysis.Slop = slope;
                            backAnalysis.RSI = rsi;
                            backAnalysis.VolV = volume;
                            backAnalysis.VolM = volumeM;
                            backAnalysis.GainPerc = 0;
                            backAnalysis.Gain = (double)_gainLost;
                            backAnalysis.StartO = 0;
                            backAnalysis.StopO = 1;
                            backAnalysis.Spread = spreadPrice;
                            _Datalogger.UpdateIGBackAnalysis(backAnalysis);
                            _size = 0;
                            _foglio = "Foglio1";
                        }
                    }
                }
              }
           }

   
          private void ManageOpenPosition()
          {
              string fileName = configYaml.OpenPosition;

              if (!File.Exists(fileName))
                  return;


              try
              {
                System.IO.StreamReader sw2 = new System.IO.StreamReader(fileName, true);
                string strRead = sw2.ReadToEnd();
                string[] arrStrRead = strRead.Split('}');
                if(arrStrRead.Length > 2)
                {
                    sw2.Close();
                    File.Delete(fileName);
                    return;
                }

                List<Position> ReadPosition = JsonConvert.DeserializeObject<List<Position>>(strRead);


                foreach (Position _position in ReadPosition)
                {
                    string[] _epics = _position.epic.Split('.');
                    double Bo = 0;
                    double So = 0;
                    decimal? limitLevel = _position.limitLevel;
             

                    openPosition(_position.size, _position.direction, _position.epic, _position.forceOpen, _position.currencyCode, _position.LimitDistance, _position.StopDistance, null, _position.limitLevel, _position.stopLevel);
                    
                    _size = (double)_position.size;
                    List<string> data = null;
                    List<double> s3 = null;
                    List<double> s2 = null;
                    List<double> s1 = null;
                    List<double> Pivot = null;
                    List<double> r3 = null;
                    List<double> r2 = null;
                    List<double> r1 = null;
                    double pAsk = 0;
                    double volume = 0;
                    double volumeM = _Datalogger.getVolume();
                    if (_realCandelData.Offer != null)
                        pAsk = (double)_realCandelData.Offer.Close;
                    if (_ChartCandelData.LastTradedVolume != null)
                        volume = (double)_ChartCandelData.LastTradedVolume;
                    double rsi = 0;
                    double slope = 0;
                    _Datalogger.getRsiEURUSD(out rsi);
                    _Datalogger.getSlopeEURUSD(out slope);
                    _Datalogger.GetPivot(out data, out s3, out s2, out s1, out Pivot, out r3, out r2, out r1);
                    BackAnalysis backAnalysis = new BackAnalysis();
                    backAnalysis.foglio = _foglio;
                    backAnalysis.Size = _size;
                    backAnalysis.ValT = pAsk;
                    backAnalysis.Bo = Bo;
                    backAnalysis.So = So;
                    backAnalysis.Tempo = ((DateTime.Now.Hour * 60) + DateTime.Now.Minute);
                    if (s3.Count > 0)
                    {
                        backAnalysis.S3 = s3[0];
                        backAnalysis.S2 = s2[0];
                        backAnalysis.S1 = s1[0];
                        backAnalysis.PV = Pivot[0];
                        backAnalysis.R1 = r3[0];
                        backAnalysis.R2 = r2[0];
                        backAnalysis.R3 = r1[0];
                    }
                    backAnalysis.Slop = slope;
                    backAnalysis.RSI = rsi;
                    backAnalysis.VolV = volume;
                    backAnalysis.VolM = volumeM;
                    backAnalysis.GainPerc = 0;
                    backAnalysis.Gain = (double)_gainLost;
                    backAnalysis.StartO = 1;
                    backAnalysis.StopO = 0;
                    backAnalysis.Spread = spreadPrice;
                    _Datalogger.UpdateIGBackAnalysis(backAnalysis);
                }
                sw2.Close();
                File.Delete(fileName);
              }
              catch 
              {

              }
          }

		  private void ManageCloseAllPosition()
          {
				string fileName = configYaml.CloseAllPosition;

				if (!File.Exists(fileName))
                  return;

				try
				{
					// Open the file using a StreamReader
					using (StreamReader sr = new StreamReader(fileName))
					{
						string line;
					//	double tmpData = 0;
						// Read and process each line until the end of the file
						while ((line = sr.ReadLine()) != null)
						{
						  string[] arrStr = line.Split(',');
						  CloseAllPosition(arrStr[0]);          
						}
					}
					File.Delete(fileName);
				}
				catch (IOException e)
				{
					Console.WriteLine("An error occurred while reading the file: " + e.Message);
				}

          }
        private void ManageModifyPosition()
        {
            string fileName = @"C:\inetpub\IGWebApi\Data\modifyPosition3.json";

            if (!File.Exists(fileName))
                return;

            try
            {
                System.IO.StreamReader sw2 = new System.IO.StreamReader(fileName, true);
                string strRead = sw2.ReadToEnd();
                List<Position> ReadPosition = JsonConvert.DeserializeObject<List<Position>>(strRead);
                foreach (Position _position in ReadPosition)
                {
                    modifyPosition(_position.limitLevel, _position.stopLevel, _position.dealId);
                }
                sw2.Close();
                File.Delete(fileName);
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred while ManageModifyPosition: " + e.Message);
            }
        }

        private void ManageClosePosition()
          {
              string fileName = configYaml.ClosePosition;

              if (!File.Exists(fileName))
                  return;

              try
              {
                  System.IO.StreamReader sw2 = new System.IO.StreamReader(fileName, true);
                  string strRead = sw2.ReadToEnd();
                  List<Position> ReadPosition = JsonConvert.DeserializeObject<List<Position>>(strRead);
                  foreach (Position _position in ReadPosition)
                  {
                        closePosition(_position.size, _position.epic, _position.direction, _position.dealId, "FROM ORDER");
                         double BC = 0;
                        double SC = 0;
                   
                        _foglio = _position.dealId;
                        _size = (double)_position.size;
                        List<string> data = null;
                        List<double> s3 = null;
                        List<double> s2 = null;
                        List<double> s1 = null;
                        List<double> Pivot = null;
                        List<double> r3 = null;
                        List<double> r2 = null;
                        List<double> r1 = null;
                        double pAsk = 0;
                        double volume = 0;
                        double volumeM = _Datalogger.getVolume();
                        if (_realCandelData.Offer != null)
                            pAsk = (double)_realCandelData.Offer.Close;
                        if (_ChartCandelData.LastTradedVolume != null)
                            volume = (double)_ChartCandelData.LastTradedVolume;
                        double rsi = 0;
                        double slope = 0;
                        _Datalogger.getRsiEURUSD(out rsi);
                        _Datalogger.getSlopeEURUSD(out slope);
                        _Datalogger.GetPivot(out data, out s3, out s2, out s1, out Pivot, out r3, out r2, out r1);
                        BackAnalysis backAnalysis = new BackAnalysis();
                        backAnalysis.foglio = _foglio;
                        backAnalysis.Size = _size;
                        backAnalysis.ValT = pAsk;
                        backAnalysis.BC = BC;
                        backAnalysis.SC = SC;
                        backAnalysis.Bo = 0;
                        backAnalysis.So = 0;
                        backAnalysis.Tempo = ((DateTime.Now.Hour *60) + DateTime.Now.Minute);
                        if (s3.Count > 0)
                        {
                            backAnalysis.S3 = s3[0];
                            backAnalysis.S2 = s2[0];
                            backAnalysis.S1 = s1[0];
                            backAnalysis.PV = Pivot[0];
                            backAnalysis.R1 = r3[0];
                            backAnalysis.R2 = r2[0];
                            backAnalysis.R3 = r1[0];
                        }
                        backAnalysis.Slop = slope;
                        backAnalysis.RSI = rsi;
                        backAnalysis.VolV = volume;
                        backAnalysis.VolM = volumeM;
                        backAnalysis.GainPerc = 0;
                        backAnalysis.Gain = (double)_gainLost;
                        backAnalysis.StartO = 0;
                        backAnalysis.StopO = 1;
                        backAnalysis.Spread = spreadPrice;
                        backAnalysis.VolM = volumeM;
                        _size = 0;
                        _foglio = "Foglio1";
                        _Datalogger.UpdateIGBackAnalysis(backAnalysis);
                  }
                  sw2.Close();
                  File.Delete(fileName);
              }
              catch 
              {

              }
          }


            public async void GetRestPositions(string epic)
            {
          {
            // _Logger.Info("GetRestPositions");

            if (igRestApiClient != null)
              {
                  try
                  {
                      _Datalogger.DeleteAllStock();
                      _bTotaleOperazioniAttive = 0;
                      var positionsResponse = await igRestApiClient.getOTCOpenPositionsV2();
                      if (positionsResponse && (positionsResponse.Response != null))
                      {
                          if (positionsResponse.Response.positions.Count != 0)
                          {
                              foreach (var position in positionsResponse.Response.positions)
                              {
                                 //   if(position.market.epic == epic)
                                    {
                                          OpenPositionData _openPosition = new OpenPositionData();
                                          _openPosition.createdDate = position.position.createdDate;
                                          _openPosition.dealId = position.position.dealId;
                                          _openPosition.size = position.position.size;
                                          _openPosition.direction = position.position.direction;
                                          _openPosition.level = position.position.level;

                                          string[] _epics = position.market.epic.Split('.');

                                         _bTotaleOperazioniAttive = _bTotaleOperazioniAttive - (int)_openPosition.size;

                          
                                         if (position.market.offer!=null)
                                         {
                                            if (_openPosition.direction=="BUY")
                                                _gainLost = (position.position.size * position.market.bid) -(position.position.size * position.position.level);
                                            else
                                                _gainLost = (position.position.size * position.position.level)- (position.position.size * position.market.offer);
                                         }
                                        //     _gainLost = _gainLost;


                                        TMsg.sendTelegramMessage("GetRestPositions 3 epic: " + position.market.epic + " direction: " + position.position.direction + " dealId: " + position.position.dealId + " _gainLost:" + _gainLost.ToString(), ChatIdMonitor, ApiTokenMonitor);
                                        _Datalogger.UpdateStock(position.market.epic, (double)position.position.size, position.position.direction, (double)_gainLost, position.position.dealId, (double)position.market.offer);
                                        _Datalogger.UpdateLucaOrder(_gainLost.ToString(), position.position.dealId);
                                     }
                                }
                        }
                        else
                        {
                          _Logger.Error("GetRestPositions: no Positions found");
                        }
                    }
                    else
                    {
                        _Logger.Error("GetRestPositions: HttpStatusCode is not ok: " + positionsResponse.StatusCode);
                    }
                }
                catch
                {
                }


                }

            }

        }



        public async void openPosition(decimal? size, string direction, string epic, bool forceOpen, string currencyCode, decimal? limitDistance, decimal? stopDistance, decimal? trailingStep,  decimal? limitLevel, decimal? stopLevel)
        {
           _Logger.Info("openPosition size:" + size + " direction:" + direction + " epic:" + epic + " currencyCode:" + currencyCode + " limitLevel:" + limitLevel + " actual:" + _realCandelData.Offer.Close + " lucaactual:" + stopLevel);
            decimal? offset = 0;
            decimal? IGdelta = 0;
            var position = new dto.endpoint.positions.create.otc.v2.CreatePositionRequest();
            position.epic = epic;
            position.currencyCode = currencyCode;
            position.direction = direction;
            position.expiry = "-";
            position.forceOpen = forceOpen;
            position.guaranteedStop = false;
            position.orderType = "MARKET";
            position.size = size;
            position.stopDistance = null;
            position.trailingStop = false;
            position.trailingStopIncrement = trailingStep;
            position.limitDistance = limitDistance;
            IGdelta = _realCandelData.Offer.Close - stopLevel;
            if (epic== "IX.D.SPTRD.IFE.IP")
            {
                if (direction == "BUY")
                {
                    offset = limitLevel - stopLevel;
                    limitLevel = _realCandelData.Offer.Close + offset;
                }
                else
                {
                   offset = stopLevel - limitLevel;
                   limitLevel = _realCandelData.Offer.Close - offset;
                }
                TMsg.sendTelegramMessage("ordine SP500 modifico TP in :"+ limitLevel);
            }
            else
            {
                if (direction == "BUY")
                {
                    offset = limitLevel - stopLevel;
                 }
                else
                {
                    offset = stopLevel - limitLevel;
                }
            }

            int nRetry = 3;
            position.stopLevel = null;
            position.limitLevel = limitLevel;
            _Logger.Info("openPosition position.stopDistance:" + position.stopDistance.ToString() + " trailingStopIncrement:" + position.trailingStopIncrement.ToString() + " limitDistance:" + position.limitDistance.ToString() + " stopLevel:" + position.stopLevel.ToString() + " limitLevel:" + position.limitLevel);

            while (nRetry > 0)
            {
                var dealRef = await igRestApiClient.createPositionV2(position);

                if (dealRef && (dealRef.Response != null))
                {
                     _Logger.Info("openPosition OK nRetry=" + nRetry.ToString());
                    var positionsRes = await igRestApiClient.retrieveConfirm(dealRef.Response.dealReference);
                    Thread.Sleep(1000);
                    _bTotaleOperazioniAttive = _bTotaleOperazioniAttive + (int)size;
                    _foglio = positionsRes.Response.dealId;
                    _Logger.Info("retrieveConfirm :" + positionsRes.StatusCode.ToString() + " dealStatus :" + positionsRes.Response.dealStatus + " reason :" + positionsRes.Response.reason);
               		TMsg.sendTelegramMessage("openPosition size: " + size + " actual:" + _realCandelData.Offer.Close + " lucaactual:" + stopLevel+ " delta MT5-IG: " + IGdelta + " gain: " + (offset*size).ToString() + " direction: " + direction + " epic: " + epic + " currencyCode: " + currencyCode + " limitLevel: " + position.limitLevel + " stopLevel: " + position.stopLevel + " dealStatus: " + positionsRes.Response.dealStatus + " reason: " + positionsRes.Response.reason);
                    _Datalogger.UpdateLucaOrder(DateTime.Now, epic, (decimal)limitLevel, stopLevel, positionsRes.Response.dealId, positionsRes.Response.level, 0, direction);
                    break;
                }
                else
                {
                    if (dealRef != null)
                        _Logger.Info("openPosition KO StatusCode:" + dealRef.StatusCode.ToString());
                    else
                        _Logger.Info("openPosition KO");
   
                    Thread.Sleep(1000);
                    if (nRetry > 0)
                        nRetry--;
                    TMsg.sendTelegramMessage("openPosition  size:" + size + " direction:" + direction + " epic:" + epic + " dealStatus :" + dealRef.StatusCode.ToString());
                }
            }
        }
        async void lastTransactionUpdate()
        {
            try
            {
                _Datalogger.DeletelastActivity();
                var positionsRes2 = await igRestApiClient.lastActivityTimeRange("01-07-2023", "07-07-2023");
                foreach (var activitie in positionsRes2.Response.activities)
                {
                    _Datalogger.UpdatelastActivity(activitie);

                    string[] results = activitie.result.Split(':');
                    string reference = results[1];

                }
                _Datalogger.DeleteTransactionClosed();
                 var positionsRes = await igRestApiClient.lastTransactionTimeRange("ALL", "01-07-2023", "14-07-2023");
                foreach (var transaction in positionsRes.Response.transactions)
                {
                    _Datalogger.UpdateTransactionClosed(transaction);
                }

            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - TransactionClosed" + ex.Message);
            }
        }

        async void modifyPosition( decimal? limitLevel, decimal? stopLevel, string dealId)
        {
            try
            {
                _Logger.Info("modifyPosition  limitLevel:" + limitLevel + " dealId:" + dealId + " stopLevel:" + stopLevel);
                var editPositionRequest = new dto.endpoint.positions.edit.v2.EditPositionRequest();
                editPositionRequest.limitLevel = limitLevel;
                editPositionRequest.stopLevel = stopLevel;
                editPositionRequest.trailingStop = false;
                editPositionRequest.trailingStopDistance = null;
                editPositionRequest.trailingStopIncrement = null;

                var response =  await igRestApiClient.editPositionV2(dealId, editPositionRequest);
                if (response.Response != null)
                {
                    _Logger.Info("modifyPosition ok");
                     TMsg.sendTelegramMessage("modifyPosition 3 modifyPosition dealId :" + dealId + " limitLevel :" + limitLevel.ToString() + " stopLevel :" + stopLevel.ToString());
                }
                else
                    _Logger.Info("closePosition KO");
            }
            catch (Exception ex)
            {
                _Logger.Error("Exception - TransactionClosed" + ex.Message);
            }
        }

        async void closePosition(decimal? size, string epic, string direction, string dealId, string typeStop)
        {
            _Logger.Info("closePosition size:" + size + "epic:" + epic + " direction:" + direction + " dealId:" + dealId + " typeStop:" + typeStop);
            GetRestPositions(epic);
            var positions = await igRestApiClient.getOTCOpenPositionsV2();

            string dir = "";
            if (direction == "BUY")
                dir = "SELL";
            else
                dir = "BUY";

            var close = new dto.endpoint.positions.close.v1.ClosePositionRequest();
            close.dealId = dealId;
            close.epic = null;
            close.expiry = null;
            close.direction = dir;
            close.size = size;
            close.level = null;
            close.orderType = "MARKET";
            close.quoteId = null;

            var response = await igRestApiClient.closePosition(close);
            if (response.Response.dealReference != null)
            {
                _Logger.Info("closePosition ok");
                var positionsRes = await igRestApiClient.retrieveConfirm(response.Response.dealReference);
                 string apiTokentrading = configYaml.ApiTokenTrading;
				 string chatIdtrading = configYaml.ChatIdTrading;
				 TMsg.sendTelegramMessage("closePosition 3 size:" + size + "epic:" + epic + " direction:" + direction + "retrieveConfirm :" + positionsRes.StatusCode.ToString()+ " typeStop:" + typeStop);
                 _Logger.Info("retrieveConfirm :" + positionsRes.StatusCode.ToString());
                _Datalogger.UpdateLucaOrder3(positionsRes.Response.dealId, positionsRes.Response.level.ToString());
            }
            else
                _Logger.Info("closePosition KO");
        }

        public async void GetHistoricalPriceByDate(string epic, string resolution)
        {
            _Datalogger.DeleteData4H();
            _Logger.Info("getHistoricalPriceByNum");
            var PriceListResponse = await igRestApiClient.priceSearchByNumV2(epic, resolution, "2");
            if (PriceListResponse && (PriceListResponse.Response != null))
            {
                _Logger.Info("remainingAllowance: "+ PriceListResponse.Response.allowance.remainingAllowance.ToString());
                
                if (PriceListResponse.Response.prices.Count != 0)
                {                
                    foreach (var price in PriceListResponse.Response.prices)
                    {
                        if (price.closePrice.ask != null)
                            _Datalogger.UpdateData4H(price.snapshotTime, (decimal)price.openPrice.ask, (decimal)price.closePrice.ask, (decimal)price.highPrice.ask, (decimal)price.lowPrice.ask, (decimal)price.lastTradedVolume, PriceListResponse.Response.allowance.remainingAllowance);
                    }
                }
            }
        }


        private void WriteLucaCandle(Decimal? open, Decimal? close, Decimal? high, Decimal? low)
        {
            string strTmp = "";
            string fileName = "";

            fileName = @"C:\inetpub\IGWebApi\Data\Candle.json";

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false);
            Candle _Candle = new Candle();

            _Candle.date = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _Candle.open = open.ToString();
            _Candle.close = close.ToString();
            _Candle.high = high.ToString();
            _Candle.low = low.ToString();


            strTmp = JsonConvert.SerializeObject(_Candle);
            sw.Write(strTmp + "\r\n");
            sw.Close();

        }
    }

}
