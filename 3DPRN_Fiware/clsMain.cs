using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using FIWARE;
using FIWARE.Orion.Client;
using FIWARE.Orion.Client.Models;
using _3DPRN_Fiware;
using TIPS;
using System.Windows.Forms;

namespace _3DPRN_Fiware
{
    internal class clsMain
    {
        
        public TCPIP_Client _onBoardClient;
        Fiware_Orion_Client _orionClient;
        string _prn_ip = "192.168.0.116";
        string _prn_port = "13000";
        public string _prn_SerialNumber = "";
        public  ContextResponse _prn_Attributes;
        string _orionClientConfig_BaseUrl = "http://192.168.0.33:1026/";
        string _orionClientConfig_Token = "2b4ba603987c4421957693e9bf14d1dbc846f473259a65e1d5bfaa21df6616ec";
        private int _retryperiod=5000;
        private readonly object _syncTimer = new object();

        public static Thread ThreadManagePrinters;

        public void MainTimer()
        {
               do
               {
               //if (STATUS.Program_Ending) break;
               try
               {
                  _onBoardClient.Write("~:STATUS_EXPOSE_GET" + VBComp.vbCrLf);  //richiesta stato
                  System.Diagnostics.Debug.Print("STATUS_EXPOSE_GET");
                  if (_prn_SerialNumber != "" )
                  {
                     _orionClient.GetStatus(_prn_SerialNumber);
                     _prn_Attributes=_orionClient._attributes;
                  }
                    
                    
                  }
                  catch (Exception ex)
                  {
                     System.Diagnostics.Debug.Print(ex.ToString());
                  }

                  Thread.Sleep(_retryperiod);
               }
               while (true);
        }

        //internal static async Task Main(frmMain frmMain)
        public  void Main(frmMain frmMain)
        {
            //var mclsMain = new clsMain();
            System.Diagnostics.Debug.Print($"Started.");

            //connessione stampante            
            _onBoardClient = new TCPIP_Client(_prn_ip, int.Parse(_prn_port));
            _onBoardClient.DataArrivedEvent += new System.EventHandler(DataArrived);
            _onBoardClient.LineCommand_End = "|";
            _onBoardClient.Connect();

            if (_onBoardClient.Connected())
            {
                _onBoardClient.Write("~:ATI SW=99,TYPECLI=CONTROLBASE,NAMECLI=HOUSTON" + VBComp.vbCrLf);

                //Istanzia Orion Context Broker
                _orionClient = new Fiware_Orion_Client(_orionClientConfig_BaseUrl, _orionClientConfig_Token);


                //Timer Lettura/scrittura

                //Printers
                ThreadManagePrinters = new Thread(MainTimer);
                ThreadManagePrinters.Priority = ThreadPriority.Normal;
                ThreadManagePrinters.IsBackground = true;
                ThreadManagePrinters.Start();


                //Timer generalTimer = new Timer((s) =>
                //{
                //    if (Monitor.TryEnter(_syncTimer))
                //    {
                //        try
                //        {
                //            _onBoardClient.Write("~:STATUS_EXPOSE_GET" + VBComp.vbCrLf);  //richiesta stato
                //            System.Diagnostics.Debug.Print("STATUS_EXPOSE_GET");
                //        }
                //        catch (Exception ex)
                //        {
                //            System.Diagnostics.Debug.Print(ex.ToString());
                //        }
                //        finally
                //        {
                //            Monitor.Exit(_syncTimer);
                //        }
                //    }
                //    else
                //        return;
                //}, null, 0, this._retryperiod);


            }
            else
            {
            //errore connessione stampante -> chiudere app?
            DialogResult result = MessageBox.Show("Can't connect with printer at " + _prn_ip, "CONNECTION ERROR", MessageBoxButtons.OK);
            System.Environment.Exit(0);
         }
    }
        public  void DataArrived(object sender, EventArgs e)
        {
            string data = ((TCPIP_Client)sender).DataRead;
            System.Diagnostics.Debug.Print("Evento Arrivato da Server:{0}", data);

            //parsing data
            string[] subs = data.Split(' ', (char)2);
            switch (subs[0])
            {
                case "~:STATUS_EXPOSE":
                    //deserializzazione
                    _3DPRN_Fiware.STATUS_EXPOSE printerstatus = new _3DPRN_Fiware.STATUS_EXPOSE();
                    var serializer = new XmlSerializer(typeof(_3DPRN_Fiware.STATUS_EXPOSE), new XmlRootAttribute("STATUS_EXPOSE"));
                    data = data.Substring(data.IndexOf(" ")).Trim();

                    using (var reader = new System.IO.StringReader(data))
                    {
                        try
                        {
                            printerstatus = (_3DPRN_Fiware.STATUS_EXPOSE)serializer.Deserialize(reader);
                              _prn_SerialNumber = printerstatus.Printer.PRN_SerialNUmber;

                              //_orionClient.UpdateStatus(printerstatus);
                              var threadOrion = new Thread(() =>
                                          _orionClient.UpdateStatus(printerstatus)
                                     );
                                     threadOrion.IsBackground = true;
                                     threadOrion.Start();


                            System.Diagnostics.Debug.Print($"DataArrived End Call");

                        }
                        catch (InvalidCastException exc)
                        {
                            // recover from exception
                            System.Diagnostics.Debug.Print($"DataArrived InvalidCastException");
                        }

                    }
                    break;

                default:
                    System.Diagnostics.Debug.Print($"Response {subs[0]}.");
                    break;
            }

            


        }
    }
}
