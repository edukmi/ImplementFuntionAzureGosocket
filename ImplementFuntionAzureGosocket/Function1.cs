using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ImplementFuntionAzureGosocket
{
    public class Function1
    {
        [FunctionName("ReadXML")]
        public async Task Run([TimerTrigger("0 */30 * * * *")] TimerInfo myTimer, ILogger log)
        {
            #region Variables
            XmlDocument doc = new XmlDocument();
            Dictionary<string, Tuple<string, string>> result = new Dictionary<string, Tuple<string, string>>();
            CultureInfo cultura = new CultureInfo("es-CO");

            doc.Load(Environment.GetEnvironmentVariable("FileLocation"));
            XmlNodeList xmlNodeList = doc.GetElementsByTagName("area");
            #endregion
            #region xml reading
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                string name = xmlNodeList[i].ChildNodes[0].InnerText.ToString();
                for (int j = 0; j < xmlNodeList[i].ChildNodes.Count; j++)
                {
                    if (xmlNodeList[i].ChildNodes[j].Name == "employees")
                    {
                        decimal totalSalary = 0;
                        for (int a = 0; a < xmlNodeList[i].ChildNodes[j].ChildNodes.Count; a++)
                        {
                            if (xmlNodeList[i].ChildNodes[j].ChildNodes[a].Name == "employee")
                            {
                                decimal salary = Convert.ToDecimal(xmlNodeList[i].ChildNodes[j].ChildNodes[a].Attributes.Item(1).Value, cultura);
                                totalSalary += salary;
                            }
                        }
                        result.Add(name, Tuple.Create(xmlNodeList[i].ChildNodes[j].SelectNodes("employee").Count.ToString(), totalSalary.ToString()));
                    }
                }
            }
            #endregion
            #region Print output
            log.LogInformation(string.Format("Total Nodos Tipo <area>: {0}", xmlNodeList.Count.ToString()));
            log.LogInformation(string.Format("{0}", Environment.NewLine));
            log.LogInformation(string.Format("Los nodos Tipo <area> que tienen mas de 2 empleados son:"));
            foreach (var item in result.Where(w => w.Value.Item1 != "2"))
                log.LogInformation(string.Format("{0} con {1} empleados", item.Key.ToString(), item.Value.Item1));
            log.LogInformation(string.Format("{0}", Environment.NewLine));
            log.LogInformation(string.Format("El total del salario por area es: "));
            foreach (var item in result)
                log.LogInformation(string.Format("Area {0} | {1} total de salario", item.Key.ToString(), item.Value.Item2));
            log.LogInformation(string.Format("{0}", Environment.NewLine));
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}"); 
            #endregion
        }
    }
}
