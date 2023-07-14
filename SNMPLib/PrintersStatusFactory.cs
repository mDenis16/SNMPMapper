using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Lextm.SharpSnmpLib;
using System.Reflection;
using System.Text.Unicode;

namespace SNMPLib
{
    
    public class PrintersStatusFactory<T> where T : class, new()
    {
        public bool Initialized { get; set; }
        public PrintersStatusFactory() { }
        public static PrintersStatusFactory<T> Instance { get; private set; }

        private  List<PrintingDevice<T>> printingDevices = new List<PrintingDevice<T>>();
        public void AddDevice(PrintingDevice<T> device) { Console.WriteLine($"Added device for ip {device.endPoint.Address.ToString()}"); printingDevices.Add(device); }
        public List<PropertyInfo> _VariableProps = new List<PropertyInfo>();
        public List<Variable> _watchedVariables = new List<Variable>();
        public List<PrintingDevice<T>> GetDeviceList() { return printingDevices;  }
        public void Initialize()
        {
            GenerateVariablesRequestList();
        }

        public List<Variable> GetWatchedVariables()
        {
            return _watchedVariables;
        }
        public void RemoveDevice(PrintingDevice<T> device) { printingDevices.Remove(device); }
        public async Task<List<T>> GetDevicesStatus(int timeout)
        {
            List<T> devicesData = new List<T>();

                foreach (var device in printingDevices)
                {
                   var result = await device.PoolData();
                     device.Data = result;
                    devicesData.Add(result);
                }
            return devicesData;
          
            
        }

        public async Task LoadDevicesFromJson() {

            try
            {
                var file = await File.ReadAllTextAsync("devices.json");
                var ipAddresses = JsonConvert.DeserializeObject<List<string>>(file);
     
                foreach (var ipAddress in ipAddresses)
                {
                    IPAddress ip;
                    if (IPAddress.TryParse(ipAddress, out ip))
                        AddDevice(new PrintingDevice<T>(this, ip, 161, "public"));
                }
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void GenerateVariablesRequestList()
        {
            _watchedVariables = new List<Variable>();

            foreach(var property in typeof(T).GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(SNMPValueAttribute), false).FirstOrDefault() as SNMPValueAttribute;
                if (attribute != null)
                {
                    var identifier = new ObjectIdentifier(attribute.Path);
                    var variable = new Variable(identifier);
                    _watchedVariables.Add(variable);
                    _VariableProps.Add(property);
                        Console.WriteLine("Pentru " + property.Name + " este " + attribute.Path);
                }
             
            }


        }

        public T GenerateDataFromResponse(IList<Variable> variables)
        {
            T obj = new T();

            
            
            for(int i = 0; i < variables.Count; i++) {

                var variable = variables[i];
                var property = _VariableProps[i];
                if (property == null || variable == null) continue;
                Console.WriteLine("prop " + property.Name + " are val " + variable.Data);

                var valueStr = variable.Data.ToString();
                 if (variable.Data.TypeCode == SnmpType.OctetString)
                {
                    property.SetValue(obj, variable.Data.ToString());
                }else if (variable.Data.TypeCode == SnmpType.Integer32 || variable.Data.TypeCode == SnmpType.Counter32)
                {
                    int Value = 0;

                    if (property.PropertyType == typeof(int))
                    {
                        if (int.TryParse(valueStr, out Value))
                        {
                            property.SetValue(obj, Value);
                        }
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(obj, valueStr);
                    }
                    else
                    {
                        // not supported type for this type of operation
                    }
                   
             
        
                }

            }
            

            return obj;
        }
    }
}
