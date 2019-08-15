using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GeradorDeArquivoCnab240.Console.Validadores;

namespace GeradorDeArquivoCnab240.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            GerarArquivoCnab240();
        }

        private static void GerarArquivoCnab240()
        {
            var arquivo = LerArquivo();
            System.Console.ReadLine();
        }

        private static StringBuilder LerArquivo()
        {
            StringBuilder arquivo = new StringBuilder();

            try
            {
                string line;
                int counter = 0;
                decimal totalPgto = 0;
                string path = Path.Combine(Environment.CurrentDirectory,
                    @"Arquivos\ArquivoReferencia\PG000##CODIGOCLIENTE##_##DATAHORA##-##INDICE##.REM");

                using (StreamReader sr = new StreamReader(path))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
//                        System.Console.WriteLine(line);
                        counter++;
                        var nwLine = SubstituirValores(line, ref totalPgto);
                        arquivo.AppendLine(nwLine);
                    }
                }

                using (var tw = new StreamWriter(Path.Combine(Environment.CurrentDirectory,
                    @"Arquivos\ARQUIVO_GERADO.REM"), true))
                {
                    tw.WriteLine(arquivo);
                }

                System.Console.WriteLine("{0} linhas.", counter);
                return arquivo;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("não foi possível ler arquivo.");
                throw;
                return arquivo;
            }
        }

        private static string SubstituirValores(string linha, ref decimal totalPgto)
        {
            if (string.IsNullOrEmpty(linha))
                return string.Empty;

            linha = SetarDadosHeader(linha);
            linha = SetarDadosSegmentoA(linha, ref totalPgto);

            return linha;
        }

        private static string SetarDadosHeader(string linha)
        {
            var dadosHeaderArquivo = new DadosHeaderArquivo("10640319000135");
            var dadosHeaderLote = new DadosHeaderLote("10640319000135");

            if (!dadosHeaderArquivo.IsValid)
                throw new Exception("Header arquivo inválido.");
            if (!dadosHeaderLote.IsValid)
                throw new Exception("Header do lote inválido.");

            linha = linha.Replace("##CNPJ_CLIENTE##", dadosHeaderArquivo.CnpjCliente);
            linha = linha.Replace("##DATA_OPERACAO##",
                dadosHeaderArquivo.DataOperacao.ToString(CultureInfo.InvariantCulture));

            return linha;
        }

        private static string SetarDadosSegmentoA(string linha, ref decimal totalPgto)
        {
            
            var dadosSegmentoA = new DetalheSegmentoA("TESTE DE FORNECEDOR", "1234567891", 1566, "12345678965448",
                DateTime.Now.AddDays(5), totalPgto);

            if (!dadosSegmentoA.IsValid)
                throw new Exception("Segmento A inválido.");

            linha = linha.Replace("##NOME_FORNECEDOR##", dadosSegmentoA.NomeFornecedor);
            linha = linha.Replace("##NUM_NOTA##", dadosSegmentoA.NumeroNota);
            linha = linha.Replace("##DATA_VENCIMENTO##",
                dadosSegmentoA.DataVencimento.ToString(CultureInfo.InvariantCulture));
            linha = linha.Replace("##VALOR_PGTO##", dadosSegmentoA.ValorPgto.ToString(CultureInfo.InvariantCulture));
            linha = linha.Replace("##CNPJ_FORNECEDOR##", dadosSegmentoA.CnpjFornecedor);

            linha = linha.Replace("##TOTAL_PGTO##",
                dadosSegmentoA.ValorTotalPgto.ToString(CultureInfo.InvariantCulture));
            totalPgto = dadosSegmentoA.ValorTotalPgto;

            return linha;
        }
    }

    public class DetalheSegmentoA
    {
        public string NomeFornecedor { get; set; }
        public string NumeroNota { get; set; }
        public decimal ValorPgto { get; set; }
        public decimal ValorTotalPgto { get; set; }
        public string CnpjFornecedor { get; set; }
        public string DataVencimento { get; set; }

        private const int LengthNomeFornecedor = 30;
        private const int LengthNumeroNota = 20;
        private const int LengthValorPgto = 15;

        public bool IsValid
        {
            get => true;
            private set { }
        }

        public DetalheSegmentoA(string nomeFornecedor, string numeroNota, decimal valorPgto, string cnpjFornecedor,
            DateTime dataVencimento, decimal totalPgto)
        {
            NomeFornecedor = nomeFornecedor;
            NumeroNota = numeroNota;
            ValorPgto = valorPgto;
            CnpjFornecedor = cnpjFornecedor;
            DataVencimento = dataVencimento.Date.ToString("MMddyyyy");
            ValorTotalPgto = totalPgto += ValorPgto;
            ComplementarNomeFornecedor();
            ComplementarNumNota();
            ComplementarValorPgto();
            ComplementarValorTotalPgto();

            Validar();
        }

        private void ComplementarNomeFornecedor()
        {
            if (string.IsNullOrEmpty(NomeFornecedor)) return;

            var padding = System.Math.Abs(NomeFornecedor.Length - LengthNomeFornecedor);
//            NomeFornecedor = NomeFornecedor.PadRight(padding, ' ');
            NomeFornecedor = string.Empty.PadRight(padding, ' ') + NomeFornecedor;
        }


        private void ComplementarNumNota()
        {
            if (string.IsNullOrEmpty(NumeroNota)) return;

            var padding = System.Math.Abs(NomeFornecedor.Length - LengthNomeFornecedor);
            NumeroNota = string.Empty.PadRight(padding, '0') + NumeroNota;
        }

        private void ComplementarValorPgto()
        {
            var vlTotal = ValorPgto.ToString(CultureInfo.InvariantCulture);
            var padding = System.Math.Abs(vlTotal.Length - LengthNomeFornecedor);
            ValorPgto = Convert.ToDecimal(string.Empty.PadRight(padding, '0') + ValorPgto);
        }

        private void ComplementarValorTotalPgto()
        {
            var vlTotal = ValorTotalPgto.ToString(CultureInfo.InvariantCulture);
            var padding = System.Math.Abs(vlTotal.Length - LengthNomeFornecedor);
            ValorTotalPgto = Convert.ToDecimal(string.Empty.PadRight(padding, '0') + ValorTotalPgto);
        }

        private void Validar()
        {
            if (NomeFornecedor.Length != LengthNomeFornecedor)
            {
                IsValid = false;
                return;
            }

            if (NumeroNota.Length != LengthNumeroNota)
            {
                IsValid = false;
                return;
            }

            if (ValorPgto.ToString(CultureInfo.CurrentCulture).Length != LengthValorPgto)
            {
                IsValid = false;
                return;
            }

            if (CnpjValidador.Validar(CnpjFornecedor))
            {
                IsValid = false;
            }
        }
    }

    public class DadosHeaderArquivo
    {
        public string CnpjCliente { get; }
        public DateTime DataOperacao => DateTime.Now.Date;

        public bool IsValid
        {
            get => true;
            private set { }
        }

        public DadosHeaderArquivo(string cnpjCliente)
        {
            CnpjCliente = cnpjCliente;
            Validar();
        }

        private void Validar()
        {
            IsValid = CnpjValidador.Validar(CnpjCliente);
        }
    }

    public class DadosHeaderLote
    {
        public bool IsValid
        {
            get => true;
            private set { }
        }

        public string CnpjCliente { get; private set; }


        public DadosHeaderLote(string cnpjCliente)
        {
            CnpjCliente = cnpjCliente;
            Validar();
        }

        private void Validar()
        {
            IsValid = CnpjValidador.Validar(CnpjCliente);
        }
    }
}