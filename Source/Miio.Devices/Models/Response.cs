using System;
using System.Collections.Generic;
using System.Text;

namespace Miio.Devices.Models
{
    public class Response
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public object[] Result { get; set; }
        public int Code { get; set; }
    }
}
