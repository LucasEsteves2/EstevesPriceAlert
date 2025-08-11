using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstevesPriceAlert.Application.Dtos
{
    public class UserOffertInput
    {
        public Guid UserId { get; set; }
        public OffertListDto Notifications { get; set; } = default!;

        public UserOffertInput()
        {
                
        }
    }
}
