using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib;
using System;
using System.Net;

class GFG
{

    // Main Method
    static public void Main(String[] args)
    {
        var result = new List<Variable>();
        Messenger.Walk(VersionCode.V1,
                       new IPEndPoint(IPAddress.Parse("192.168.88.245"), 161),
                       new OctetString("public"),
                       new ObjectIdentifier("1.3.6.1.2.1.43.11.1"),
                       result,
                       60000,
                       WalkMode.WithinSubtree);

        Console.WriteLine("mearsa");
    }
}