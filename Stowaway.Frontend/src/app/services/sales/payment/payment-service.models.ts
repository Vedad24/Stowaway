export interface CreatePaymentCommand
{
    orderId : number
}
export interface CreatePaymentResponse
{
    checkoutUrl : string,
    externalPaymentId : string,
    status : string
}

export interface UpdateStripePaymentCommand
{
    
}