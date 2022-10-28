using FatoRelevante.Context;
using FatoRelevante.Entidades;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FatoRelevante
{
    public class FatoRelevanteService
    {
         
        public async Task<string> GetJsonFatos()
        {
            CookieContainer cookies = new();
            using HttpClientHandler handler = new();

            handler.CookieContainer = cookies;
            handler.AllowAutoRedirect = true;
            handler.UseCookies = true;

            HttpClient httpClient = new(handler, true);
            await httpClient.GetAsync(new Uri("https://sistemas.cvm.gov.br/?fatosrelev"));
            await httpClient.GetAsync(new Uri("https://www.rad.cvm.gov.br/ENET/frmConsultaExternaCVM.aspx"));

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json,text/javascript,*/*;q=0.01");
            var requestPayload = $"{{ dataDe: '{DateTime.Now.AddDays(-2):dd/MM/yyyy}', dataAte: '{DateTime.Now:dd/MM/yyyy}' , empresa: '', setorAtividade: '-1', categoriaEmissor: '-1', situacaoEmissor: '-1', tipoParticipante: '-1', dataReferencia: '', categoria: 'EST_-1,IPE_-1_-1_-1', periodo: '2', horaIni: '00:01', horaFim: '23:59', palavraChave:'',ultimaDtRef:'false', tipoEmpresa:'0', token: '', versaoCaptcha: ''}}";
            
            var formDoc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("",requestPayload),
            };

            var content = new StringContent(requestPayload, Encoding.UTF8, "application/json");

            var url = "https://www.rad.cvm.gov.br/ENET/frmConsultaExternaCVM.aspx/ListarDocumentos";
            var page = await httpClient.PostAsync(
                new Uri(url),
                content);

            return await page.Content.ReadAsStringAsync();
        }
        public async Task GetFatosAsync()
        {

            FatosRelevantesContext _context = new();

            string strPage = await GetJsonFatos();

            dynamic json = JObject.Parse(strPage);

            string dados = Regex.Unescape(json.d.dados.ToString());
            
            var fatos = dados.Split('*');

            var regexURLs = new Regex("<i(.*)i>");

            var urls = fatos.Select(a => regexURLs.Match(a).ToString()).ToList();

            var fatosRemoveURLs = fatos.Select(a => regexURLs.Replace(a, "")).ToList();

            var fatosSplit = fatosRemoveURLs.Where(a => a != "").Select(a => a.Replace("$&", "#").Split('#').Where(b => b != "" && b != "&" && b != "$").ToList()).ToList();

            var contagensDistintas = fatosSplit.Select(a => a.Count()).Distinct().ToList();


            List<RegistroFatoRelevante> fatosRelevantes = new();

            for(int i = 0; i < fatosSplit.Count; i++)
            {
                var fato = fatosSplit[i];
                var urlFato = urls[i];
                string assunto = string.Empty;
                if (fato.Count == 11)
                    assunto = fato[10];

                var dataEntregaString = fato[6].Substring(fato[6].Length - 16, 16);
                var dataEntrega = DateTime.ParseExact(dataEntregaString, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var dataReferenciaString = fato[5].Substring(11, 8);
                var dataReferencia = DateTime.ParseExact(dataReferenciaString, "yyyyMMdd", CultureInfo.InvariantCulture);

                var regexNumeroSequencial = new Regex("(?<=NumeroSequencialDocumento)(.*)(?=&)");
                string numeroSequencial = regexNumeroSequencial.Match(urlFato).ToString();
                
                var regexCodigoTipoInstituicao= new Regex("(?<=CodigoTipoInstituicao=)(.*?)(?=\\'\\))");
                string codigoTipoInstituicao = regexCodigoTipoInstituicao.Match(urlFato).ToString();

                var regexDownloadParameters = new Regex("(?<=OpenDownloadDocumentos\\()(.*?)(?=\\))");
                string downloadParameters = regexDownloadParameters.Match(urlFato).ToString();

                var parameters = downloadParameters.Replace("\'", "").Split(",");

                var regexTipo = new Regex("(?<=\")(.*?)(?=\"\\))");
                string tipo = regexTipo.Match(urlFato).ToString();

                var fatoObject = new RegistroFatoRelevante()
                {
                    Assunto = assunto,
                    Categoria = fato[2],
                    DataEntrega = dataEntrega,
                    DataReferencia = dataReferencia,
                    Tipo = tipo,
                    CodigoTipoInstituicao = codigoTipoInstituicao,
                    Empresa = new Empresa()
                    {
                        CodigoCvm = fato[0],
                        Nome = fato[1]
                    },
                    NumeroProtocolo = parameters[3],
                    Modalidade = fato[9],
                    NumeroProtocoloEntrega = parameters[2],
                    NumeroSequencia = numeroSequencial,
                    Status = fato[7]
                };



                fatosRelevantes.Add(fatoObject);

            }

            Console.WriteLine("Boa");

        }
    }
}
