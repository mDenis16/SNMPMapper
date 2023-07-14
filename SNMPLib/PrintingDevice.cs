using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SNMPLib
{
  
    public class PrintingDevice<DataClass> where DataClass : class, new()
    {
        private OctetString Community { get; set; }
        public IPEndPoint endPoint { get; set; }
        public DataClass Data { get; set; }
        public PrintersStatusFactory<DataClass> Factory { get; set; } // Make it public

        public PrintingDevice(PrintersStatusFactory<DataClass> factory, IPAddress _IpAddress, int port, string _Community)
        {
            endPoint = new IPEndPoint(_IpAddress, port);
            Community = new OctetString(_Community);
            Factory = factory;
            Data = new DataClass();
        }

        public PrintingDevice() { }

      
        public async Task<DataClass> PoolData()
        {
           

            GetBulkRequestMessage message = new GetBulkRequestMessage(0,
                                                               VersionCode.V2,
                                                               Community,
                                                               0,
                                                                1,
                                                                Factory.GetWatchedVariables());

            ISnmpMessage response = await message.GetResponseAsync(endPoint, new CancellationTokenSource(millisecondsDelay: 10000).Token);
            if (response.Pdu().ErrorStatus.ToInt32() != 0)
            {
                Console.WriteLine("Error in response");
            }

            var vars = response.Pdu().Variables;
            return Factory.GenerateDataFromResponse(response.Pdu().Variables); 
        }
    }
}
