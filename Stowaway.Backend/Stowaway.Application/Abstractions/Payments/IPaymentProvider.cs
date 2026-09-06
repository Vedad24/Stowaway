using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stowaway.Application.Abstractions.Payments
{
    public interface IPaymentProvider
    {
        Task<CreatePaymentResult> MakePaymentAsync(CreatePaymentRequest request);
    }
}