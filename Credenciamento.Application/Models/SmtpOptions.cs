using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Credenciamento.Application.Models;

public class SmtpOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Sender { get; set; }
}


