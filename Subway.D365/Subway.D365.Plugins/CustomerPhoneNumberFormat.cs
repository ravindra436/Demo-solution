using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Subway.D365.Plugins
{
    public class CustomerPhoneNumberFormat :IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IOrganizationService __service;
            IPluginExecutionContext __context;

            ITracingService __ts = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            __context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            __service = serviceFactory.CreateOrganizationService(__context.UserId);

            Entity __e = (Entity)__context.InputParameters["Target"];

            if (__e.Contains("mobilephone"))
            {

                String __mobilePhone = FormatPhone(__e.Attributes["mobilephone"].ToString());
                __e.Attributes["mobilephone"] = __mobilePhone;
            }

        }


        public string FormatPhone(String phone)
        {
            // remove special char from phone 
            string _phone = Regex.Replace(phone, @"[^0-9a-zA-Z]+", "").ToLower();

            int count = 0;
            if (_phone.Length > 10)
            {

                for (int i = 0; i < _phone.Length; i++)
                {
                    if (Char.IsDigit(_phone[i]))
                    {
                        count = count + 1;
                        if (count == 10)//&& !Char.IsDigit(_phone[i+1])
                        {
                            string prePhone = _phone.Substring(0, i + 1);
                            string postPhone = _phone.Substring(i + 1);
                            string _prePhone = Regex.Replace(prePhone, @"[^0-9]", "").ToLower();
                            _phone = String.Concat(_prePhone, postPhone);
                            break;
                        }
                    }
                }
            }

            string pattern = @"(?<=[a-zA-Z])(?=\d)";
            string[] arr = Regex.Split(_phone, pattern);

            Regex regex = new Regex("[a-zA-Z]");
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = regex.Replace(arr[i], "");
            }

            if (_phone.Length >= 10)
            {
                if (_phone.Length == 10)
                {
                    return String.Format("{0:(0##) ###-####}", Convert.ToInt64(_phone));
                }
                else if (arr[0].Length == 10)
                {
                    //Format of arr[0]
                    arr[0] = String.Format("{0:(0##) ###-####}", Convert.ToInt64(arr[0]));
                    //Concat arr[0],x,arr[1],....;
                    string concat = arr[0];
                    if (arr.Length > 1)
                    {
                        concat = String.Concat(arr[0], " x", arr[1]);
                        for (int i = 2; i < arr.Length; i++)
                        {
                            concat = String.Concat(concat, " x", arr[i]);
                        }
                    }
                    return concat;
                }
                else
                {
                    return phone;
                }
            }
            else
            {
                return phone;
            }

        }
       
    }
}
