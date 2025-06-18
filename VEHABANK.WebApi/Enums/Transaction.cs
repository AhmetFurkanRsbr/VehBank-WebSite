namespace VEHABANK.WebApi.Enums
{
    public enum Transaction //İşlemler
    {
        Withdraw=0,  //Para çekme
        Deposit=1,   //Para yatırma
        Transfer = 2, //Para transferi 
        Remittance = 3, //Para transferi Havale
        ExternalTransfer = 4, //Para transferi EFT
        UpdatePassword =5, //Parola güncelleme
        GetBranchDetails = 6, //Şube Bilgisi Getirme
        ViewAccounts = 7, //Hesapları Görüntüleme
        Payment = 8, //Fatura-Yurt Ödemesi
    }
}
