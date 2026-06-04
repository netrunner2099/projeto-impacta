using Credenciamento.Application.Models;

namespace Credenciamento.Web.Areas.Admin.Models;

public class AdminUserViewModel : LocalBaseViewModel
{
    public string? FilterName { get; set; }
    public byte? FilterRole { get; set; }
    public IEnumerable<UserModel> Users { get; set; } = [];
}
