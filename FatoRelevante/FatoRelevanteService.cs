using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace FatoRelevante
{
    public class FatoRelevanteService
    {
        public async Task GetFatosAsync()
        {
            CookieContainer cookies = new();
            using HttpClientHandler handler = new();

            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;
            
            HttpClient httpClient = new HttpClient(handler, true);
            await httpClient.GetAsync(new Uri("https://sistemas.cvm.gov.br/?fatosrelev"));
            await httpClient.GetAsync(new Uri("https://www.rad.cvm.gov.br/ENET/frmConsultaExternaCVM.aspx"));
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json,text/javascript,*/*;q=0.01");
            var requestPayload = $"{{ dataDe: '{DateTime.Now.AddDays(-2):dd/MM/yyyy}', dataAte: '{DateTime.Now:dd/MM/yyyy}' , empresa: '', setorAtividade: '-1', categoriaEmissor: '-1', situacaoEmissor: '-1', tipoParticipante: '-1', dataReferencia: '', categoria: 'EST_-1,IPE_-1_-1_-1', periodo: '2', horaIni: '00:01', horaFim: '23:59', palavraChave:'',ultimaDtRef:'false', tipoEmpresa:'0', token: '', versaoCaptcha: ''}}";
            //var requestPayload = $"{{ dataDe: '21/10/2022', dataAte: '22/10/2022' , empresa: '', setorAtividade: '-1', categoriaEmissor: '-1', situacaoEmissor: '-1', tipoParticipante: '-1', dataReferencia: '', categoria: 'EST_-1,IPE_-1_-1_-1', periodo: '2', horaIni: '00:01', horaFim: '23:59', palavraChave:'',ultimaDtRef:'false', tipoEmpresa:'0', token: '', versaoCaptcha: ''}}";

            var formDoc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("",requestPayload),
            };

            var content = new StringContent(requestPayload, Encoding.UTF8, "application/json");

            var url = "https://www.rad.cvm.gov.br/ENET/frmConsultaExternaCVM.aspx/ListarDocumentos";
            var page = await httpClient.PostAsync(
                new Uri(url),
                content);

            string strPage = await page.Content.ReadAsStringAsync();

            dynamic json = JObject.Parse(strPage);

            string dados = Regex.Unescape(json.d.dados.ToString());
            
            var fatos = dados.Split('*');

            var regexURLs = new Regex("<i(.*)i>");

            var urls = fatos.Select(a => regexURLs.Match(a)).ToList();

            var fatosRemoveURLs = fatos.Select(a => regexURLs.Replace(a, "")).ToList();

            var fatosSplit = fatosRemoveURLs.Where(a => a != "").Select(a => a.Replace("$&", "#").Split('#').Where(b => b != "" && b != "&" && b != "$").ToList()).ToList();

            var contagensDistintas = fatosSplit.Select(a => a.Count()).Distinct().ToList();

        }
    }
}
