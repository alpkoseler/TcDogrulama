using Microsoft.AspNetCore.Mvc;
using TCValidate.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TCValidate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCController : ControllerBase
    {
        // GET api/<TCController>/5
        [HttpGet("{TCNO}")]
        public async Task<Message> Get(long TCNO, string Ad, string Soyad, int DogumYili)
        {
            var returnResult = new Message();
            if (!TcDogrulaV2(TCNO.ToString()))
            {
                returnResult.Mesaj = "T.C. No Formatı Yanlış!";
                returnResult.DogrulamaSonucu = false;

                return returnResult;
            }

            var client = new MValidate.KPSPublicSoapClient(MValidate.KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);

            var response = await client.TCKimlikNoDogrulaAsync(TCNO, Ad, Soyad, DogumYili);

            var result = response.Body.TCKimlikNoDogrulaResult;

            returnResult.DogrulamaSonucu = result;
            returnResult.Mesaj = "T.C. No doğru!";

            if (result == false)
            {
                returnResult.Mesaj = "Girilen bilgilerde yanlışlık vardır!";
                returnResult.DogrulamaSonucu = false;
            }

            return returnResult;
        }
        public static bool TcDogrulaV2(string tcKimlikNo)
        {
            bool returnvalue = false;
            if (tcKimlikNo.Length == 11)
            {
                Int64 ATCNO, BTCNO, TcNo;
                long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

                TcNo = Int64.Parse(tcKimlikNo);

                ATCNO = TcNo / 100;
                BTCNO = TcNo / 100;

                C1 = ATCNO % 10; ATCNO = ATCNO / 10;
                C2 = ATCNO % 10; ATCNO = ATCNO / 10;
                C3 = ATCNO % 10; ATCNO = ATCNO / 10;
                C4 = ATCNO % 10; ATCNO = ATCNO / 10;
                C5 = ATCNO % 10; ATCNO = ATCNO / 10;
                C6 = ATCNO % 10; ATCNO = ATCNO / 10;
                C7 = ATCNO % 10; ATCNO = ATCNO / 10;
                C8 = ATCNO % 10; ATCNO = ATCNO / 10;
                C9 = ATCNO % 10; ATCNO = ATCNO / 10;
                Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
                Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

                returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
            }
            return returnvalue;
        }
    }
}
