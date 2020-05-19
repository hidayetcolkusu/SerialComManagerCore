using BaseComManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialComManager
{
    public class SerialInitInfo : BaseInitInfo, IBaseInitInfo
    {
        public string Port  { get; set; }
        public int BaudRate { get; set; }
    }
}
