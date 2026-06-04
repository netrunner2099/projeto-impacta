using System;

namespace Credenciamento.Web.Models
{
    public class ErrorViewModel : LocalBaseViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
